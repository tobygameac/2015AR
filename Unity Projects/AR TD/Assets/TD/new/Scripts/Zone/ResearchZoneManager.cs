using UnityEngine;
using System.Collections.Generic;

public class ResearchZoneManager : MonoBehaviour {

  private Game game;

  public GameObject researchZone;
  private List<GameObject> researchZones;

  public float restTimeAfterChange;

  private float timeAfterResting;

  public Vector3 zoneGap;

  void Start() {
    game = Camera.main.GetComponent<Game>();

    researchZones = new List<GameObject>();

    timeAfterResting = Time.time;

    UpdateResearchZones();
  }

  private void UpdateResearchZones() {
    for (int i = 0; i < researchZones.Count; ++i) {
      Destroy(researchZones[i]);
    }

    researchZones.Clear();

    List<Technology> availableTechnology = game.AvailableTechnology();

    for (int i = 0; i < availableTechnology.Count; ++i) {
      GameObject newResearchZone = Instantiate(researchZone, transform.position + zoneGap * i, transform.rotation) as GameObject;

      newResearchZone.transform.parent = game.gameSceneParentTransform;
      newResearchZone.GetComponent<ResearchZone>().technologyIndex = i;
      newResearchZone.GetComponent<ResearchZone>().informationTextGUI.text = availableTechnology[i].Name;
      newResearchZone.GetComponent<ResearchZone>().researchZoneManager = this;

      researchZones.Add(newResearchZone);
    }
  }

  public void ResearchTechnology(int technologyIndex) {
    if (Time.time < timeAfterResting) {
      return;
    }

    if (game.ResearchTechnology(technologyIndex)) {
      timeAfterResting = Time.time + restTimeAfterChange;
      UpdateResearchZones();
    }

    return;
  }
}
