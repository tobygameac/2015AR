using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterStats))]
[RequireComponent (typeof(SphereCollider))]
public class FireStormDevice : MonoBehaviour {

  public GameObject FXObject;
  private ParticleSystem FXParticleSystem;

  public float additionalFXTime = 3.0f;

  [SerializeField]
  private float damageScale = 1.0f;
  public float DamageScale {
    get {
      return damageScale;
    }
  }

  public GameObject deviceBase;
  private Rotating deviceBaseRotating;

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
    FXParticleSystem = FXObject.GetComponent<ParticleSystem>();
    FXParticleSystem.startSize *= attackingRange;

    deviceBaseRotating = deviceBase.GetComponent<Rotating>();

    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }

    lastAttackingTime = Time.time - additionalFXTime - 1;
  }

  void Update() {
    if (Time.time - lastAttackingTime <= additionalFXTime) {
      FXParticleSystem.loop = true;
      if (FXParticleSystem.isStopped) {
        FXParticleSystem.Play();
      }
      deviceBaseRotating.enabled = true;
    } else {
      FXParticleSystem.loop = false;
      deviceBaseRotating.enabled = false;
    }
  }

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      CharacterStats targetCharacterStats = collider.GetComponent<CharacterStats>();
      targetCharacterStats.MovingSpeedModifier -= characterStats.Damage;
    }
  }

  void OnTriggerStay(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      lastAttackingTime = Time.time;
      CharacterStats targetCharacterStats = collider.GetComponent<CharacterStats>();
      targetCharacterStats.CurrentHP -= characterStats.Damage * attackingSpeed * damageScale * Time.deltaTime;
      if (targetCharacterStats.CurrentHP <= 0) {
        ++characterStats.UnitKilled;
        if (game.HasTechnology(GameConstants.TechnologyID.SELF_LEARNING)) {
          characterStats.DamageModifier += GameConstants.SELF_LEARNING_IMPROVEMENT_PERCENT_PER_KILL;
        } else {
        }
      }
    }
  }

  void OnTriggerExit(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      CharacterStats targetCharacterStats = collider.GetComponent<CharacterStats>();
      targetCharacterStats.MovingSpeedModifier += characterStats.Damage;
    }
  }
}
