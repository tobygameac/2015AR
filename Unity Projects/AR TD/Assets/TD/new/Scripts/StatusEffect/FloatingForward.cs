using UnityEngine;
using System.Collections;

public class FloatingForward: MonoBehaviour {

  public float floatingMagnitude;
  public float floatingSpeed;

  private Vector3 originalPosition;

  void Start() {
    originalPosition = transform.localPosition;
  }

  void Update() {
    Vector3 newPosition = originalPosition + transform.forward * floatingMagnitude * Mathf.Sin(floatingSpeed * Time.time);
    transform.localPosition = newPosition;
  }

  void Disable() {
    transform.localPosition = originalPosition;
  }
}
