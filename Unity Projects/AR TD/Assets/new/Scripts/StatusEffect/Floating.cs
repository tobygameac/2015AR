using UnityEngine;
using System.Collections;

public class Floating : MonoBehaviour {

  public float floatingMagnitude;
  public float floatingSpeed;

  private float originalY;

  void Start() {
    originalY = transform.position.y;
  }

  void Update() {
    float newY = originalY + floatingMagnitude * Mathf.Sin(floatingSpeed * Time.time);
    transform.position = new Vector3(transform.position.x, newY, transform.position.z);
  }

  void Disable() {
    transform.position = new Vector3(transform.position.x, originalY, transform.position.z);
  }
}
