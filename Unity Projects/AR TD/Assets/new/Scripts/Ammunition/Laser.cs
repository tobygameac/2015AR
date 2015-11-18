using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

  [SerializeField]
  private float shootingTime = 0.1f;
  private float passedTime;
  private bool hasTarget;

  public GameObject explosion;

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
}
