using UnityEngine;
using System.Collections;

public class RangeDisplayer : MonoBehaviour {

  public GameObject backgroundImage;
  public GameObject imageToRotate;

  public float rotatingSpeed = 90;

  private SphereCollider sphereCollider;

  private Transform transformToDisplayRange;

  void Start() {
    transformToDisplayRange = transform;
    while (transformToDisplayRange.gameObject.GetComponent<CharacterStats>() == null) {
      transformToDisplayRange = transform.parent;
    }
    sphereCollider = transformToDisplayRange.GetComponent<SphereCollider>();
    transform.localScale = Vector3.one * sphereCollider.radius;

    backgroundImage.SetActive(true);
    imageToRotate.SetActive(true);
  }

  void Update() {
    transform.localScale = Vector3.one * sphereCollider.radius;

    //imageToRotate.transform.Rotate(Vector3.up * Time.deltaTime * rotatingSpeed, Space.World);
    imageToRotate.transform.Rotate(Vector3.forward * Time.deltaTime * rotatingSpeed);
  }

}
