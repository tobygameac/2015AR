using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterStats))]
[RequireComponent (typeof(SphereCollider))]
public class SuperFireTurret : MonoBehaviour {

  //public AudioClip projectileSound;

  public GameObject[] FXObject;
  private ParticleSystem[] FXParticleSystem;

  public Transform[] muzzles;
  public Transform muzzleBase;
  private Rotating muzzleBaseRotating;

  public float attackingAngleForEachMuzzle;

  private float attackingSpeed;
  public float turningSpeed;

  public float warmingUpTime;
  private float nextAttackTime;

  public float stopFireTimeWhenNoTarget;
  private float noTargetTime;

  private CharacterStats characterStats;

  private Transform target;

  private static Game game;

  void Start() {
    FXParticleSystem = new ParticleSystem[FXObject.Length];
    for (int i = 0; i < FXParticleSystem.Length; ++i) {
      FXParticleSystem[i] = FXObject[i].GetComponent<ParticleSystem>();
    }

    characterStats = GetComponent<CharacterStats>();

    attackingSpeed = characterStats.AttackingSpeed;
    GetComponent<SphereCollider>().radius = characterStats.AttackingRange;

    muzzleBaseRotating = muzzleBase.GetComponent<Rotating>();
    muzzleBaseRotating.rotatingSpeed = turningSpeed;

    target = null;

    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }
  }

  void Update() {
    if (target != null) {
      noTargetTime = 0;
      for (int i = 0; i < FXParticleSystem.Length; ++i) {
        if (!FXParticleSystem[i].isPlaying) {
          nextAttackTime = Time.time + warmingUpTime;
          FXParticleSystem[i].Play();
          muzzleBaseRotating.enabled = true;
        }
      }
    } else {
      noTargetTime += Time.deltaTime;
      if (noTargetTime >= stopFireTimeWhenNoTarget) {
        for (int i = 0; i < FXParticleSystem.Length; ++i) {
          if (FXParticleSystem[i].isPlaying) {
            FXParticleSystem[i].Stop();
          }
        }
        muzzleBaseRotating.enabled = false;
      }
    }
  }

  void OnTriggerEnter(Collider collider) {
    if (target == null && collider.gameObject.tag == "Enemy") {
      target = collider.gameObject.transform;
    }
  }

  void OnTriggerStay(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      if (target == null) {
        target = collider.gameObject.transform;
      }
      if (Time.time >= nextAttackTime) {
        for (int i = 0; i < muzzles.Length; ++i) {
          Quaternion desiredRotation = Quaternion.LookRotation(collider.transform.position - muzzleBase.position);
          float angleFromMuzzleToEnemy = Quaternion.Angle(muzzles[i].rotation, desiredRotation);
          if (angleFromMuzzleToEnemy <= attackingAngleForEachMuzzle / 2) {
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
    }
  }

  void OnTriggerExit(Collider collider) {
    if (target == collider.transform) {
      target = null;
    }
  }

}
