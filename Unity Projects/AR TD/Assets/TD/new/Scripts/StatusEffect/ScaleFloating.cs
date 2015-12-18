using UnityEngine;
using System.Collections;

public class ScaleFloating : MonoBehaviour {

  public float floatingMagnitude;
  public float floatingSpeed;

  public Vector3 floatingScale;

  private Vector3 originalScale;

  void Start() {
    originalScale = transform.localScale;
  }

  void Update() {
    transform.localScale = originalScale + floatingMagnitude * floatingScale * Mathf.Sin(floatingSpeed * Time.time);
  }
  
  void OnDisable() {
    transform.localScale = originalScale;
  }

}
