using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(SphereCollider))]
public class Turret : MonoBehaviour {

  public AudioClip projectileSound;

  public GameObject projectilePrefab;
  public float turningSpeed;

  private float attackingSpeed;
  private float attackingRange;

  private float reloadTime;

  public Transform[] muzzles;
  public Transform turretBall;

  private float nextFireTime;

  private Transform target;

  private CharacterStats characterStats;

  private static Game game;

  void Start() {
    characterStats = GetComponent<CharacterStats>();

    attackingSpeed = characterStats.AttackingSpeed;

    if (attackingSpeed <= 1e-8f) {
      attackingSpeed = 1e-8f;
    }

    reloadTime = (1 / attackingSpeed);

    GetComponent<SphereCollider>().radius = characterStats.AttackingRange;

    target = null;

    nextFireTime = Time.time;

    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }
  }

  void Update() {
    if (target != null) {
      Quaternion desiredRotation = Quaternion.LookRotation(target.position - turretBall.position);
      desiredRotation.eulerAngles = new Vector3(turretBall.localEulerAngles.x, desiredRotation.eulerAngles.y, turretBall.localEulerAngles.z); // y-axis only
      turretBall.localRotation = Quaternion.Slerp(turretBall.localRotation, desiredRotation, Time.deltaTime * turningSpeed);
      if (Time.time >= nextFireTime) {
        FireProjectile();
      }
    }
  }

  void OnTriggerEnter(Collider collider) {
    if (target == null && collider.gameObject.tag == "Enemy") {
      target = collider.gameObject.transform;
    }
  }

  void OnTriggerStay(Collider collider) {
    if (target == null && collider.gameObject.tag == "Enemy") {
      target = collider.gameObject.transform;
    }
  }

  void OnTriggerExit(Collider collider) {
    if (target == collider.transform) {
      target = null;
    }
  }

  public void DealDamage(GameObject enemyGameObject) {
    CharacterStats enemyCharacterStats = enemyGameObject.GetComponent<CharacterStats>();
    enemyCharacterStats.CurrentHP -= characterStats.Damage / muzzles.Length;
    if (enemyCharacterStats.CurrentHP <= 0) {
      ++characterStats.UnitKilled;
      if (game.HasTechnology(GameConstants.TechnologyID.SELF_LEARNING)) {
        characterStats.DamageModifier += GameConstants.SELF_LEARNING_IMPROVEMENT_PERCENT_PER_KILL;
      } else {
      }
    }
  }

  private void FireProjectile() {
    AudioManager.PlayAudioClip(projectileSound);

    nextFireTime = Time.time + reloadTime;

    for (int i = 0; i < muzzles.Length; ++i) {
      GameObject projectileGameObject = Instantiate(projectilePrefab, muzzles[i].position, Quaternion.identity) as GameObject;

      projectileGameObject.transform.parent = game.gameSceneParentTransform;

      Projectile projectile = projectileGameObject.GetComponent<Projectile>();

      projectile.SourceTurret = this;
      projectile.TargetPosition = target.position;
    }
  }
}
