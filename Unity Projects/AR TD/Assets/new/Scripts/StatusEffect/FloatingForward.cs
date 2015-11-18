using UnityEngine;
using System.Collections;

public class FloatingForward: MonoBehaviour {

  public float floatingMagnitude;
  public float floatingSpeed;

  private Vector3 originalPosition;

  void Start() {
    originalPosition = transform.position;
  }

  void Update() {
    Vector3 newPosition = originalPosition + transform.forward * floatingMagnitude * Mathf.Sin(floatingSpeed * Time.time);
    transform.position = newPosition;
  }

  void Disable() {
    transform.position = originalPosition;
  }
}
