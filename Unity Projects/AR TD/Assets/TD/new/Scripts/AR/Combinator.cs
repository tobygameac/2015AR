using UnityEngine;
using System.Collections;

public class Combinator : MonoBehaviour {

  public float combinationDistance = 1.5f;

  public float checkTimeGap = 0.1f;
  private float lastCheckTime;

  public GameObject combinationFXObject;
  private ParticleSystem combinationParticleSystem;

  public Transform[] markersToTrack;
  private GameObject[] buildingBelongsToMarkers;

  private static Game game;
  private bool hasCombinationTechnology;

  void Start() {
    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }

    if (combinationFXObject != null) {
      combinationParticleSystem = combinationFXObject.GetComponent<ParticleSystem>();
    }

    buildingBelongsToMarkers = new GameObject[markersToTrack.Length];
    for (int i = 0; i < buildingBelongsToMarkers.Length; ++i) {
      buildingBelongsToMarkers[i] = null;
    }

    hasCombinationTechnology = false;

    lastCheckTime = Time.time;
  }

  void Update() {
    if (Time.time - lastCheckTime >= checkTimeGap) {

      if (!hasCombinationTechnology) {
        hasCombinationTechnology = game.HasTechnology(GameConstants.TechnologyID.COMBINATE);
      }

      if (!hasCombinationTechnology) {
        return;
      }

      for (int i = 0; i < markersToTrack.Length; ++i) {
        for (int j = i + 1; j < markersToTrack.Length; ++j) {
          double markerDistance = Vector3.Distance(markersToTrack[i].position, markersToTrack[j].position);
          if (markerDistance < combinationDistance) {
            if (buildingBelongsToMarkers[i] == null) {
              GetBuildingBelongsToMarker(i);
            }
            if (buildingBelongsToMarkers[j] == null) {
              GetBuildingBelongsToMarker(j);
            }

            if (buildingBelongsToMarkers[i] != null && buildingBelongsToMarkers[j] != null) {
              bool combinationSuccess = game.CombinateBuilding(buildingBelongsToMarkers[i], buildingBelongsToMarkers[j]);
              if (combinationSuccess) {
                lastCheckTime = Time.time;
                if (combinationParticleSystem != null) {
                  //combinationFXObject.transform.position = (buildingBelongsToMarkers[i].transform.position + buildingBelongsToMarkers[j].transform.position) * 0.5f;
                  combinationFXObject.transform.position = buildingBelongsToMarkers[i].transform.position;
                  combinationParticleSystem.Play();
                }
                return;
              }
            }
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
