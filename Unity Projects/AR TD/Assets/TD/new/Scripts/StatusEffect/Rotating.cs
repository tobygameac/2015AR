using UnityEngine;
using System.Collections;

public class Rotating : MonoBehaviour {

  public float rotatingSpeed;

  public Vector3 rotatingScale;

  void Update() {
    transform.localEulerAngles = transform.localEulerAngles + rotatingScale * rotatingSpeed * Time.deltaTime;
  }
}
