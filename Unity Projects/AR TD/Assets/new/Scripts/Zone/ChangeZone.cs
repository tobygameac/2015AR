﻿using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChangeZone : MonoBehaviour {

  public GameObject newBuilding;

  private Game game;

  private LayerMask buildingLayerMask;

  public float restTimeAfterChange;

  private float timeAfterResting;

  void Start() {
    game = Camera.main.GetComponent<Game>();

    buildingLayerMask = LayerMask.NameToLayer("Building");

    timeAfterResting = Time.time;
  }

  void OnTriggerStay(Collider collider) {
    if (Time.time < timeAfterResting) {
      return;
    }

    if (newBuilding == null) {
      return;
    }

    if (collider.gameObject.layer == buildingLayerMask) {
      Transform targetBuildingTransform = collider.transform;
      // Find real building object
      while (targetBuildingTransform.transform.parent != null) {
        targetBuildingTransform = targetBuildingTransform.parent;
      }

      if (game.ChangeBuilding(targetBuildingTransform.gameObject, newBuilding)) {
        timeAfterResting = Time.time + restTimeAfterChange;
      }
    }
  }

}
