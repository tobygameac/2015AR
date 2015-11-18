using UnityEngine;
using System.Collections;

public class Rotating : MonoBehaviour {

  public float rotatingSpeed;

  public Vector3 rotatingScale;

  void Update() {
    transform.eulerAngles = transform.eulerAngles + rotatingScale * rotatingSpeed * Time.deltaTime;
  }
}
