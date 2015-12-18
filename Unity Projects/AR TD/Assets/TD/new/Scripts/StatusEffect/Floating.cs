using UnityEngine;
using System.Collections;

public class Floating : MonoBehaviour {

  public float floatingMagnitude;
  public float floatingSpeed;

  private float originalY;

  void Start() {
    originalY = transform.localPosition.y;
  }

  void Update() {
    float newY = originalY + floatingMagnitude * Mathf.Sin(floatingSpeed * Time.time);
    transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
  }

  void Disable() {
    transform.localPosition = new Vector3(transform.localPosition.x, originalY, transform.localPosition.z);
  }
}
