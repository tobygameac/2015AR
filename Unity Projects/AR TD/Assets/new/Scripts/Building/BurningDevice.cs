using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterStats))]
[RequireComponent (typeof(SphereCollider))]
public class BurningDevice : MonoBehaviour {

  public GameObject burningFXObject;
  private ParticleSystem burningFXParticleSystem;

  public float additionalBurningFXTime = 3.0f;

  public GameObject muzzleBase;
  private Rotating muzzleBaseRotating;

  private float attackingSpeed;
  private float attackingRange;

  private CharacterStats characterStats;

  private float lastAttackingTime;

  private static Game game;

  void Start() {
    characterStats = GetComponent<CharacterStats>();

    attackingSpeed = characterStats.AttackingSpeed;
    attackingRange = characterStats.AttackingRange;

    GetComponent<SphereCollider>().radius = attackingRange;
    burningFXParticleSystem = burningFXObject.GetComponent<ParticleSystem>();
    burningFXParticleSystem.startSize *= attackingRange;

    muzzleBaseRotating = muzzleBase.GetComponent<Rotating>();

    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }

    lastAttackingTime = Time.time - additionalBurningFXTime - 1;
  }

  void Update() {
    if (Time.time - lastAttackingTime <= additionalBurningFXTime) {
      burningFXParticleSystem.loop = true;
      if (burningFXParticleSystem.isStopped) {
        burningFXParticleSystem.Play();
      }
      muzzleBaseRotating.enabled = true;
    } else {
      burningFXParticleSystem.loop = false;
      muzzleBaseRotating.enabled = false;
    }
  }

  void OnTriggerStay(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      lastAttackingTime = Time.time;

      CharacterStats targetCharacterStats = collider.GetComponent<CharacterStats>();
      targetCharacterStats.CurrentHP -= characterStats.Damage * attackingSpeed * Time.deltaTime;
      if (targetCharacterStats.CurrentHP <= 0) {
        ++characterStats.UnitKilled;
        if (game.HasTechnology(GameConstants.TechnologyID.SELF_LEARNING)) {
          characterStats.DamageModifier += GameConstants.SELF_LEARNING_IMPROVEMENT_PERCENT_PER_KILL;
        } else {
        }
      }
    }
  }
}
