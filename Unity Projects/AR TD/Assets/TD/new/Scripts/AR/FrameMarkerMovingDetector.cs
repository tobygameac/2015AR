using UnityEngine;
using System.Collections;

public class FrameMarkerMovingDetector : MonoBehaviour {

  public float movingSpeedAlertThreshold = 1.0f;

  public float checkTimeGap = 1.0f;

  private Vector3 lastPosition;

  private float lastCheckTime;

  private static Game game;

  void Start() {
    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }

    lastPosition = transform.position;

    lastCheckTime = Time.time;
  }

  void Update() {
    if (Time.time - lastCheckTime >= checkTimeGap) {

      float movingSpeed = Vector3.Distance(transform.position, lastPosition) / Time.deltaTime;

      if (movingSpeed >= movingSpeedAlertThreshold) {

        GameObject buildingBelongsToThisFrame = null;

        foreach (Transform childTransform in transform) {
          if (childTransform.GetComponent<CharacterStats>() != null) {
            buildingBelongsToThisFrame = childTransform.gameObject;
            break;
          }
        }

        if (buildingBelongsToThisFrame != null) {
          game.selectedBuilding = buildingBelongsToThisFrame;
        }
      }
      lastCheckTime = Time.time;
    }

    lastPosition = transform.position;
  }

}
