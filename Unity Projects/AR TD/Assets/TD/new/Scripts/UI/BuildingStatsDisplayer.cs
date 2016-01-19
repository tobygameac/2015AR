using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingStatsDisplayer : MonoBehaviour {

  public Sprite[] iconSprites;

  public GameObject buildingIcon;
  private Image buildingIconImage;

  public GameObject buildingStats;
  private Text buildingStatsText;
  
  private Game game;

  private GameObject lastBuilding;
  private CharacterStats characterStats;

  void Start() {
    buildingIconImage = buildingIcon.GetComponent<Image>();
    buildingStatsText = buildingStats.GetComponent<Text>();

    game = Camera.main.GetComponent<Game>();

    lastBuilding = null;
  }

  void Update() {
    GameObject building = game.SelectedBuilding;

    if (building != null && building != lastBuilding) {
      characterStats = building.GetComponent<CharacterStats>();
      lastBuilding = building;
    }

    if (building == null) {
      return;
    }

    if ((int)characterStats.BuildingID >= 0 && (int)characterStats.BuildingID < iconSprites.Length) {
      buildingIconImage.enabled = true;
      buildingIconImage.sprite = iconSprites[(int)characterStats.BuildingID];
    } else {
      buildingIconImage.enabled = false;
    }

    buildingStatsText.text = "<color=#ffffff>" + GameConstants.NameOfBuildingID[(int)characterStats.BuildingID] + "</color>\n";
    buildingStatsText.text += "<color=red>價值 : </color><color=yellow>" + characterStats.Cost + "</color>\n\n";
    if (characterStats.BuildingID == GameConstants.BuildingID.SLOWING_DEVICE) {
      buildingStatsText.text += "<color=#ffffff>減緩移動速度 : </color><color=#00ff00>" + (characterStats.Damage * 100).ToString("0.00") + "%\n";
    } else if (characterStats.BuildingID == GameConstants.BuildingID.SPEEDING_DEVICE) {
      buildingStatsText.text += "<color=#ffffff>增加移動速度 : </color><color=#00ff00>" + (-characterStats.Damage * 100).ToString("0.00") + "%\n";
    } else if (characterStats.BuildingID == GameConstants.BuildingID.WEAKENING_DEVICE) {
      buildingStatsText.text += "<color=#ffffff>降低敵軍最大生命 : </color><color=#00ff00>" + (characterStats.Damage * 100).ToString("0.00") + "%\n";
    } else if (characterStats.BuildingID == GameConstants.BuildingID.FIRE_STORM_DEVICE) {
      float damageScale = building.GetComponent<FireStormDevice>().DamageScale;
      buildingStatsText.text += "<color=#ffffff>減緩移動速度 : </color><color=#00ff00>" + (characterStats.Damage * 100).ToString("0.00") + "%</color>";
      if (game.HasTechnology(GameConstants.TechnologyID.SELF_LEARNING)) {
        buildingStatsText.text += "<color=red>(+" + (characterStats.DamageModifier * characterStats.BasicDamage * 100).ToString("0.00") + "%)</color>";
      }
      buildingStatsText.text += "\n";
      buildingStatsText.text += "<color=#ffffff>傷害 : </color><color=#00ff00>" + (characterStats.Damage * damageScale).ToString("0.0");
    } else {
      buildingStatsText.text += "<color=#ffffff>傷害 : </color><color=#00ff00>" + (characterStats.Damage).ToString("0.0");
    }
    buildingStatsText.text += "</color>";
    if (game.HasTechnology(GameConstants.TechnologyID.SELF_LEARNING)) {
      if (characterStats.BuildingID == GameConstants.BuildingID.SLOWING_DEVICE) {
      } else if (characterStats.BuildingID == GameConstants.BuildingID.SPEEDING_DEVICE) {
      } else if (characterStats.BuildingID == GameConstants.BuildingID.WEAKENING_DEVICE) {
      } else if (characterStats.BuildingID == GameConstants.BuildingID.FIRE_STORM_DEVICE) {
        float damageScale = building.GetComponent<FireStormDevice>().DamageScale;
        buildingStatsText.text += "<color=red>(+" + (characterStats.DamageModifier * characterStats.BasicDamage * damageScale).ToString("0.0") + ")</color>";
      } else {
        buildingStatsText.text += "<color=red>(+" + (characterStats.DamageModifier * characterStats.BasicDamage).ToString("0.0") + ")</color>";
      }
    }
    buildingStatsText.text += "\n";
    if (characterStats.BuildingID == GameConstants.BuildingID.SLOWING_DEVICE) {
    } else if (characterStats.BuildingID == GameConstants.BuildingID.SPEEDING_DEVICE) {
    } else if (characterStats.BuildingID == GameConstants.BuildingID.WEAKENING_DEVICE) {
    } else {
      buildingStatsText.text += "<color=#ffffff>攻擊速度 : </color><color=#00ff00>" + (characterStats.AttackingSpeed).ToString("0.00") + "</color>\n";
    }
    buildingStatsText.text += "<color=#ffffff>攻擊範圍 : </color><color=#00ff00>" + characterStats.AttackingRange + "</color>\n";
    buildingStatsText.text += "<color=#ffffff>擊殺數 : </color><color=#00ff00>" + characterStats.UnitKilled + "</color>\n";
  }
}
