using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UpgradeZone : MonoBehaviour {

  private Game game;

  private LayerMask buildingLayerMask;

  void Start() {
    game = Camera.main.GetComponent<Game>();

    buildingLayerMask = LayerMask.NameToLayer("Building");
  }

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.layer == buildingLayerMask) {
      Transform targetBuildingTransform = collider.transform;
      // Find real building object
      while (targetBuildingTransform.transform.parent != null) {
        targetBuildingTransform = targetBuildingTransform.parent;
      }
      game.UpgradeBuilding(targetBuildingTransform.gameObject);
    }
  }
}
