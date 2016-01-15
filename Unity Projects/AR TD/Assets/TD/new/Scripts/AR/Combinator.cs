using UnityEngine;
using System.Collections;

public class Combinator : MonoBehaviour {

  public float combinationDistance = 2.0f;

  public float checkTimeGap = 0.1f;
  private float lastCheckTime;

  public GameObject combinationFX;

  public Transform[] markersToTrack;
  private GameObject[] buildingBelongsToMarkers;

  private static Game game;

  void Start() {
    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }

    buildingBelongsToMarkers = new GameObject[markersToTrack.Length];
    for (int i = 0; i < buildingBelongsToMarkers.Length; ++i) {
      buildingBelongsToMarkers[i] = null;
    }

    lastCheckTime = Time.time;
  }

  void Update() {
    if (Time.time - lastCheckTime >= checkTimeGap) {

      double minDistance = 2e9;
      int minI = 0, minJ = 0;
      for (int i = 0; i < markersToTrack.Length; ++i) {
        for (int j = i + 1; j < markersToTrack.Length; ++j) {
          double markerDistance = Vector3.Distance(markersToTrack[i].position, markersToTrack[j].position);
          if (markerDistance < minDistance) {
            minDistance = markerDistance;
            minI = i;
            minJ = j;
          }
        }
      }

      if (minDistance < combinationDistance) {
        if (buildingBelongsToMarkers[minI] == null) {
          GetBuildingBelongsToMarker(minI);
        }
        if (buildingBelongsToMarkers[minJ] == null) {
          GetBuildingBelongsToMarker(minJ);
        }

        if (buildingBelongsToMarkers[minI] != null && buildingBelongsToMarkers[minJ] != null) {
          bool combinationSuccess = game.CombinateBuilding(buildingBelongsToMarkers[minI], buildingBelongsToMarkers[minJ]);
          if (combinationSuccess) {

          }
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
