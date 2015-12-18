using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterStats))]
[RequireComponent (typeof(SphereCollider))]
public class FireTurret : MonoBehaviour {

  //public AudioClip projectileSound;

  public GameObject[] FXObject;
  private ParticleSystem[] FXParticleSystem;

  public Transform[] muzzles;
  public Transform muzzleBase;

  public float attackingAngleForEachMuzzle;

  public float turningSpeed;
  public float warmingUpTime;
  private float nextAttackTime;

  private float attackingRange;
  private float attackingSpeed;

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
    attackingRange = characterStats.AttackingRange;

    GetComponent<SphereCollider>().radius = attackingRange;

    target = null;

    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }
  }

  void Update() {
    if (target != null) {
      noTargetTime = 0;
      for (int i = 0; i < FXParticleSystem.Length; ++i) {
        if (FXParticleSystem[i].isStopped) {
          nextAttackTime = Time.time + warmingUpTime;
          FXParticleSystem[i].Play();
        }
      }
      Quaternion desiredRotation = Quaternion.LookRotation(target.position - muzzleBase.position);
      desiredRotation.eulerAngles = new Vector3(muzzleBase.localEulerAngles.x, desiredRotation.eulerAngles.y,muzzleBase.localEulerAngles.z); // y-axis only
      muzzleBase.localRotation = Quaternion.Slerp(muzzleBase.localRotation, desiredRotation, Time.deltaTime * turningSpeed);
    } else {
      noTargetTime += Time.deltaTime;
      if (noTargetTime >= stopFireTimeWhenNoTarget) {
        for (int i = 0; i < FXParticleSystem.Length; ++i) {
          FXParticleSystem[i].Stop();
        }
      }
    }
  }

  void OnTriggerEnter(Collider collider) {
    if (target == null && collider.gameObject.tag == "Enemy") {
      target = collider.transform;
    }
  }

  void OnTriggerStay(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      if (target == null) {
        target = collider.transform;
      }
      if (Time.time >= nextAttackTime) {
        for (int i = 0; i < muzzles.Length; ++i) {
          Quaternion desiredRotation = Quaternion.LookRotation(collider.transform.position - muzzleBase.position);
          float angleToEnemy = Quaternion.Angle(muzzles[i].rotation, desiredRotation);
          if (angleToEnemy <= attackingAngleForEachMuzzle / 2) {
            target = collider.transform;
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
