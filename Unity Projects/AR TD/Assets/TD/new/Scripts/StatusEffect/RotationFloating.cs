using UnityEngine;
using System.Collections;

public class RotationFloating : MonoBehaviour {

  public float floatingMagnitude;
  public float floatingSpeed;

  public Vector3 floatingScale;

  private Vector3 originalEulerAngles;

  void Start() {
    originalEulerAngles = transform.localEulerAngles;
  }

  void Update() {
    transform.localEulerAngles = originalEulerAngles + floatingMagnitude * floatingScale * Mathf.Sin(floatingSpeed * Time.time);
  }

  void OnDisable() {
    transform.localEulerAngles = originalEulerAngles;
  }
}
