using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChangeZone : MonoBehaviour {

  public GameObject newBuilding;

  private Game game;

  private LayerMask buildingLayerMask;

  public float restTimeAfterChange;

  private float timeAfterResting;

  private bool initialized;

  void Start() {
    initialized = false;

    game = Camera.main.GetComponent<Game>();

    buildingLayerMask = LayerMask.NameToLayer("Building");

    timeAfterResting = Time.time;

    initialized = true;
  }

  void OnTriggerStay(Collider collider) {
    if (!initialized) {
      return;
    }

    if (Time.time < timeAfterResting) {
      return;
    }

    if (newBuilding == null) {
      return;
    }

    if (collider.gameObject.layer == buildingLayerMask) {
      Transform targetBuildingTransform = collider.transform;
      // Find real building object
      while (targetBuildingTransform.GetComponent<CharacterStats>() == null) {
        targetBuildingTransform = targetBuildingTransform.parent;
      }

      if (game.ChangeBuilding(targetBuildingTransform.gameObject, newBuilding)) {
        timeAfterResting = Time.time + restTimeAfterChange;
      }
    }
  }

}
