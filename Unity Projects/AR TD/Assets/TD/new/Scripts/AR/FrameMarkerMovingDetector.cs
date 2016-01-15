using UnityEngine;
using System.Collections;

public class FrameMarkerMovingDetector : MonoBehaviour {

  public float movingSpeedAlertThreshold = 3.0f;

  public float checkTimeGap = 0.1f;
  private float lastCheckTime;

  public Transform[] markersToTrack;
  private Vector3[] lastPosition;

  private GameObject[] buildingBelongsToMarkers;

  private static Game game;

  void Start() {
    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }

    lastPosition = new Vector3[markersToTrack.Length];
    for (int i = 0; i < lastPosition.Length; ++i) {
      lastPosition[i] = markersToTrack[i].position;
    }

    buildingBelongsToMarkers = new GameObject[markersToTrack.Length];
    for (int i = 0; i < buildingBelongsToMarkers.Length; ++i) {
      buildingBelongsToMarkers[i] = null;
    }

    lastCheckTime = Time.time;
  }

  void Update() {
    if (Time.time - lastCheckTime >= checkTimeGap) {

      float maxMovingSpeed = 0;
      int maxMovingSpeedIndex = 0;

      for (int i = 0; i < markersToTrack.Length; ++i) {

        float movingSpeed = Vector3.Distance(markersToTrack[i].position, lastPosition[i]) / Time.deltaTime;

        if (movingSpeed > maxMovingSpeed) {
          maxMovingSpeed = movingSpeed;
          maxMovingSpeedIndex = i;
        }

        lastPosition[i] = markersToTrack[i].position;
      }

      if (maxMovingSpeed >= movingSpeedAlertThreshold) {
        if (buildingBelongsToMarkers[maxMovingSpeedIndex] == null) {
          GetBuildingBelongsToMarker(maxMovingSpeedIndex);
        }
        if (buildingBelongsToMarkers[maxMovingSpeedIndex] != null) {
          game.selectedBuilding = buildingBelongsToMarkers[maxMovingSpeedIndex];
        }
      }

      lastCheckTime = Time.time;
    }

  }

  void GetBuildingBelongsToMarker(int markerIndex) {
    foreach (Transform childTransform in markersToTrack[markerIndex]) {
      if (childTransform.GetComponent<CharacterStats>() != null) {
        buildingBelongsToMarkers[markerIndex] = childTransform.gameObject;
        return;
      }
    }
  }

}
