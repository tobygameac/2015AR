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

  private const float SMOOTHING_WIEGHT = 0.0f;

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

    int visibleLocaterMarkerCount = 0;
    bool[] locaterMarkerIsVisible = new bool[locaterMarkersTrackedObjects.Length];
    float newLeft = 2e9f;
    float newTop = -2e9f;
    for (int i = 0; i < locaterMarkersTrackedObjects.Length; ++i) {
      if (locaterMarkersTrackedObjects[i].GetMarker().Visible) {
        ++visibleLocaterMarkerCount;
        locaterMarkerIsVisible[i] = true;
        newLeft = Mathf.Min(newLeft, locaterMarkers[i].position.x);
        newTop = Mathf.Max(newTop, locaterMarkers[i].position.z);
      }
    }

    if (visibleLocaterMarkerCount < 2) {
    } else {
      left = newLeft;
      top = newTop;

      float widthSum = 0;
      float heightSum = 0;

      float newWidth = 0;
      float newHeight = 0;

      int widthCount = 0, heightCount = 0;
      for (int i = locaterMarkersTrackedObjects.Length - 1; i > 1; i -= 2) {
        if (locaterMarkerIsVisible[i]) {
          if (locaterMarkerIsVisible[i - 1]) {
            ++widthCount;
            widthSum += (locaterMarkers[i].position.x - left);
          }
          if (locaterMarkerIsVisible[i - 2]) {
            ++heightCount;
            heightSum += (top - locaterMarkers[i].position.z);
          }
        }
      }

      if (widthCount > 0) {
        newWidth = widthSum / widthCount;
      }

      if (heightCount > 0) {
        newHeight = heightSum / heightCount;
      }

      if (newWidth > 0) {
        width = newWidth;
      }

      if (newHeight > 0) {
        height = newHeight;
      }
    }

    if (width == 0 || height == 0) {
      return;
    }

    for (int i = 0; i < buildingMarkers.Length; ++i) {
      buildingPositionStatusList[i].isVisible = false;

      if (buildingMarkersTrackedObjects[i].GetMarker().Visible) {
        buildingPositionStatusList[i].xPercent = SMOOTHING_WIEGHT * buildingPositionStatusList[i].xPercent + (1 - SMOOTHING_WIEGHT) * (buildingMarkers[i].position.x - left) / width;
        buildingPositionStatusList[i].yPercent = SMOOTHING_WIEGHT * buildingPositionStatusList[i].yPercent + (1 - SMOOTHING_WIEGHT) * (1 - ((top - buildingMarkers[i].position.z) / height));
        buildingPositionStatusList[i].isVisible = true;
      }

      Debug.Log((buildingPositionStatusList[i].isVisible ? "Ok" : "GG") + " : " + buildingPositionStatusList[i].xPercent + " " + buildingPositionStatusList[i].yPercent);
    }
  }
}