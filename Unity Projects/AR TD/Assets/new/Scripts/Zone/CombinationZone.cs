using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CombinationZone : MonoBehaviour {

  private Game game;

  private LayerMask buildingLayerMask;

  private List<Transform> toCombinationBuildingTransformList;

  void Start() {
    game = Camera.main.GetComponent<Game>();

    buildingLayerMask = LayerMask.NameToLayer("Building");

    toCombinationBuildingTransformList = new List<Transform>();
  }

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.layer == buildingLayerMask) {
      Transform targetBuildingTransform = collider.transform;
      // Find real building object
      while (targetBuildingTransform.transform.parent != null) {
        targetBuildingTransform = targetBuildingTransform.parent;
      }
      toCombinationBuildingTransformList.Add(targetBuildingTransform);

      if (toCombinationBuildingTransformList.Count >= 2) {
        CheckCombination();
      }
    }
  }

  void OnTriggerExit(Collider collider) {
    if (collider.gameObject.layer == buildingLayerMask) {
      Transform targetBuildingTransform = collider.transform;
      // Find real building object
      while (targetBuildingTransform.transform.parent != null) {
        targetBuildingTransform = targetBuildingTransform.parent;
      }
      toCombinationBuildingTransformList.Remove(targetBuildingTransform);
    }
  }

  private void CheckCombination() {
    for (int i = 0; i < toCombinationBuildingTransformList.Count; ++i) {
      if (toCombinationBuildingTransformList[i] == null) {
        toCombinationBuildingTransformList.RemoveAt(i--);
      }
    }

    for (int i = 0; i < toCombinationBuildingTransformList.Count; ++i) {
      for (int j = i + 1; j < toCombinationBuildingTransformList.Count; ++j) {
        if (game.CombinateBuilding(toCombinationBuildingTransformList[i].gameObject, toCombinationBuildingTransformList[j].gameObject)) {
          toCombinationBuildingTransformList.Clear();
          return;
        }
      }
    }
  }
}
