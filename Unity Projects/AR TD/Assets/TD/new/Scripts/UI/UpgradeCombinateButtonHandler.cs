using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpgradeCombinateButtonHandler : MonoBehaviour {

  public GameObject upgradeButtonObject;
  public GameObject combinateButtonObject;

  private Button upgradeButton;
  //private Button combinateButton;

  private Text upgradeButtonText;
  private string upgradeButtonOriginalText;
  private bool hasUpgradeTechnology;
  private bool hasCombinateTechnology;

  private Game game;

  private GameObject lastBuilding;
  private CharacterStats characterStats;


  void Start() {
    game = Camera.main.GetComponent<Game>();

    upgradeButton = upgradeButtonObject.GetComponent<Button>();
    //combinateButton = combinateButtonObject.GetComponent<Button>();

    upgradeButtonObject.SetActive(true);
    combinateButtonObject.SetActive(false);

    upgradeButton.interactable = false;
    upgradeButtonText = upgradeButton.transform.GetChild(0).GetComponent<Text>();
    upgradeButtonOriginalText = upgradeButtonText.text;

    string upgradeTechnologyName = GameConstants.NameOfTechnologyID[(int)GameConstants.TechnologyID.UPGRADE];
    upgradeButtonText.text = "需研發" + upgradeTechnologyName + "";

    hasUpgradeTechnology = false;

    lastBuilding = null;
  }

  void Update() {

    if (!hasUpgradeTechnology) {
      hasUpgradeTechnology = game.HasTechnology(GameConstants.TechnologyID.UPGRADE);
      return;
    }

    GameObject building = game.SelectedBuilding;

    if (!hasCombinateTechnology) {
      hasCombinateTechnology = game.HasTechnology(GameConstants.TechnologyID.COMBINATE);
      if (hasCombinateTechnology && building != null) {
        characterStats = building.GetComponent<CharacterStats>();
        if (characterStats.NextLevel == null) {
          upgradeButtonObject.SetActive(false);
          combinateButtonObject.SetActive(true);
        }
      }
    }

    if (building != null && building != lastBuilding) {
      characterStats = building.GetComponent<CharacterStats>();
      lastBuilding = building;

      if (characterStats.NextLevel != null) {
        upgradeButton.interactable = hasUpgradeTechnology;
        int nextLevelCost = characterStats.NextLevel.GetComponent<CharacterStats>().Cost;
        upgradeButtonText.text = upgradeButtonOriginalText + " : " + (nextLevelCost - characterStats.Cost);

        upgradeButtonObject.SetActive(true);
        combinateButtonObject.SetActive(false);
      } else {
        upgradeButton.interactable = false;
        string combinateTechnologyName = GameConstants.NameOfTechnologyID[(int)GameConstants.TechnologyID.COMBINATE];
        upgradeButtonText.text = "需研發" + combinateTechnologyName + "";
        if (hasCombinateTechnology) {
          upgradeButtonObject.SetActive(false);
          combinateButtonObject.SetActive(true);
        }
      }
    }

    if (building == null) {
      upgradeButtonText.text = upgradeButtonOriginalText;
      return;
    }

  }

}
