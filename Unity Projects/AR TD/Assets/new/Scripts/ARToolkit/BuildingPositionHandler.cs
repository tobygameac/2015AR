using UnityEngine;
using System.Collections.Generic;

public class BuildingPositionHandler : MonoBehaviour {

  public Transform[] locaterMarkers;
  private ARTrackedObject[] locaterMarkersTrackedObjects;

  public Transform[] buildingMarkers;
  private ARTrackedObject[] buildingMarkersTrackedObjects;

  private float left;
  private float top;
  private float width;
  private float height;


  public class BuildingPositionStatus {

    public BuildingPositionStatus() {
      isVisible = false;
      xPercent = yPercent = 0;
    }

    public bool isVisible;
    public float xPercent;
    public float yPercent;
  };

  private List<BuildingPositionStatus> buildingPositionStatusList;
  public List<BuildingPositionStatus> BuildingPositionStatusList {
    get {
      return buildingPositionStatusList;
    }
  }

  void Start() {
    locaterMarkersTrackedObjects = new ARTrackedObject[locaterMarkers.Length];
    for (int i = 0; i < locaterMarkers.Length; ++i) {
      locaterMarkersTrackedObjects[i] = locaterMarkers[i].GetComponent<ARTrackedObject>();
    }

    buildingMarkersTrackedObjects = new ARTrackedObject[buildingMarkers.Length];
    for (int i = 0; i < buildingMarkers.Length; ++i) {
      buildingMarkersTrackedObjects[i] = buildingMarkers[i].GetComponent<ARTrackedObject>();
    }

    buildingPositionStatusList = new List<BuildingPositionStatus>(new BuildingPositionStatus[buildingMarkers.Length]);
    for (int i = 0; i < buildingPositionStatusList.Count; ++i) {
      buildingPositionStatusList[i] = new BuildingPositionStatus();
    }
  }

  void Update() {

    int count = 0;
    bool[] locaterMarkerIsVisible = new bool[locaterMarkersTrackedObjects.Length];
    float newLeft = 2e9f;
    float newTop = -2e9f;
    for (int i = 0; i < locaterMarkersTrackedObjects.Length; ++i) {
      if (locaterMarkersTrackedObjects[i].GetMarker().Visible) {
        ++count;
        locaterMarkerIsVisible[i] = true;
        newLeft = Mathf.Min(newLeft, locaterMarkers[i].position.x);
        newTop = Mathf.Max(newTop, locaterMarkers[i].position.z);
      }
    }

    if (count > 0) {
      left = newLeft;
      top = newTop;
    }

    if (count < 2) {
    } else {
      float widthSum = 0;
      float heightSum = 0;
      int w_count = 0, h_count = 0;
      for (int i = locaterMarkersTrackedObjects.Length - 1; i > 1; i -= 2) {
        if (locaterMarkerIsVisible[i]) {
          if (locaterMarkerIsVisible[i - 1]) {
            ++w_count;
            widthSum += (locaterMarkers[i].position.x - left);
          }
          if (locaterMarkerIsVisible[i - 2]) {
            ++h_count;
            heightSum += (top - locaterMarkers[i].position.z);
          }
        }
      }

      if (w_count > 0) {        
        width = widthSum / w_count;
      }

      if (h_count > 0) {
        height = heightSum / h_count;
      }
    }

    if (width == 0 || height == 0) {
      return;
    }

    for (int i = 0; i < buildingMarkers.Length; ++i) {
      buildingPositionStatusList[i].isVisible = false;

      if (buildingMarkersTrackedObjects[i].GetMarker().Visible) {
        buildingPositionStatusList[i].xPercent = (buildingMarkers[i].position.x - left) / width;
        buildingPositionStatusList[i].yPercent = 1 - ((top - buildingMarkers[i].position.z) / height);
        buildingPositionStatusList[i].isVisible = true;
      }

      //Debug.Log((buildingPositionStatusList[i].isVisible ? "Ok" : "GG") + " : " + buildingPositionStatusList[i].xPercent + " " + buildingPositionStatusList[i].yPercent);
    }
  }
}

