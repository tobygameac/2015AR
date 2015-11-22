using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UpgradeZone : MonoBehaviour {

  private Game game;

  private LayerMask buildingLayerMask;

  public float restTimeAfterUpgrading;

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

    if (collider.gameObject.layer == buildingLayerMask) {
      Transform targetBuildingTransform = collider.transform;
      // Find real building object
      while (targetBuildingTransform.transform.parent != null) {
        targetBuildingTransform = targetBuildingTransform.parent;
      }

      if (game.UpgradeBuilding(targetBuildingTransform.gameObject)) {
        timeAfterResting = Time.time + restTimeAfterUpgrading;
      }
    }
  }
}
