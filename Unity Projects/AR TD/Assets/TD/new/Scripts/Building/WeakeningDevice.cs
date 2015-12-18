using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterStats))]
[RequireComponent (typeof(SphereCollider))]
public class WeakeningDevice : MonoBehaviour {

  public GameObject FXObject;
  private ParticleSystem FXParticleSystem;

  public float additionalFXTime = 3.0f;

  public GameObject deviceBase;
  private Rotating deviceBaseRotating;

  private float attackingRange;

  private CharacterStats characterStats;

  private float lastAttackingTime;

  private static Game game;

  void Start() {
    characterStats = GetComponent<CharacterStats>();

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
      if (!FXParticleSystem.isPlaying) {
        FXParticleSystem.Play();
      }
      FXParticleSystem.loop = true;
      deviceBaseRotating.enabled = true;
    } else {
      FXParticleSystem.loop = false;
      deviceBaseRotating.enabled = false;
    }
  }

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      CharacterStats targetCharacterStats = collider.GetComponent<CharacterStats>();
      if (characterStats.Damage >= 1e-5f) {
        targetCharacterStats.HPScaler *= (1 - characterStats.Damage);
      }
    }
  }

  void OnTriggerStay(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      lastAttackingTime = Time.time;
    }
  }

  void OnTriggerExit(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      CharacterStats targetCharacterStats = collider.GetComponent<CharacterStats>();
      targetCharacterStats.HPScaler /= (1 - characterStats.Damage);
    }
  }
}
