using UnityEngine;
using UnityEngine.UI;

public class ResearchZone : MonoBehaviour {

  public int technologyIndex;

  public ResearchZoneManager researchZoneManager;

  public Text informationTextGUI;

  private Game game;

  private LayerMask buildingLayerMask;

  private bool initialized;

  void Start() {
    initialized = false;

    game = Camera.main.GetComponent<Game>();

    buildingLayerMask = LayerMask.NameToLayer("Building");

    initialized = true;
  }

  void OnTriggerStay(Collider collider) {
    if (!initialized) {
      return;
    }

    if (researchZoneManager == null) {
      return;
    }

    if (collider.gameObject.layer == buildingLayerMask) {
      researchZoneManager.ResearchTechnology(technologyIndex);
    }
  }

}
