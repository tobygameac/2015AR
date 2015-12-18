using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Projectile : MonoBehaviour {

  [SerializeField]
  private float shootingTime = 0.2f;
  private float passedTime;
  private bool hasTarget;

  public GameObject explosion;

  private static Game game;

  private Turret _sourceTurret;
  public Turret SourceTurret {
    get {
      return _sourceTurret;
    }
    set {
      _sourceTurret = value;
    }
  }

  private Vector3 _targetPosition;
  public Vector3 TargetPosition {
    get {
      return _targetPosition;
    }
    set {
      _targetPosition = value;
      hasTarget = true;
    }
  }

  void Start() {
    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }
  }

  void Update() {
    if (!hasTarget) {
      return;
    }
    transform.position = Vector3.Lerp(transform.position, TargetPosition, passedTime / shootingTime);
    passedTime += Time.deltaTime;
    if (passedTime >= shootingTime) {
      if (explosion != null) {
        GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation) as GameObject;
        Destroy(newExplosion, 1);
      }
      Destroy(gameObject);
    }
  }

  void OnCollisionEnter(Collision collision) {
    if (collision.gameObject.tag == "Enemy") {
      if (_sourceTurret != null) {
        _sourceTurret.DealDamage(collision.gameObject);
      }
      if (explosion != null) {
        GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation) as GameObject;

        newExplosion.transform.parent = game.gameSceneParentTransform;

        Destroy(newExplosion, 1);
      }
      Destroy(gameObject);
    }
  }
}
