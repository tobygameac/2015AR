using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingDetailDisplayer : MonoBehaviour {

  public Sprite[] iconSprites;

  public GameObject changeButtonGameObject;
  private Button changeButton;

  public GameObject buildingIcon;
  private Image buildingIconImage;

  public GameObject buildingDetail;
  private Text buildingDetailText;

  private static Game game;

  private int previousViewingBuildingIndex;
  private GameObject previousSelectedBuilding;

  void Start() {
    buildingIconImage = buildingIcon.GetComponent<Image>();
    buildingDetailText = buildingDetail.GetComponent<Text>();

    changeButton = changeButtonGameObject.GetComponent<Button>();

    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }

    previousViewingBuildingIndex = -1;
    previousSelectedBuilding = null;
  }

  void Update() {
    if (previousViewingBuildingIndex != game.ViewingBuildingIndex || previousSelectedBuilding != game.SelectedBuilding) {
      UpdateBuildingDetail();
      previousViewingBuildingIndex = game.ViewingBuildingIndex;
      previousSelectedBuilding = game.SelectedBuilding;
    }
  }

  void UpdateBuildingDetail() {
    if (game.ViewingBuildingIndex >= 0) {
      CharacterStats characterStats = game.BasicBuildingList[game.ViewingBuildingIndex].GetComponent<CharacterStats>();

      if ((int)characterStats.BuildingID < iconSprites.Length) {
        buildingIconImage.sprite = iconSprites[(int)characterStats.BuildingID];
      } else {
        buildingIconImage.sprite = iconSprites[iconSprites.Length - 1];
      }

      buildingDetailText.text = "<color=orange>" + GameConstants.NameOfBuildingID[(int)characterStats.BuildingID] + "</color>\n\n";

      buildingDetailText.text += "<color=lime>" + characterStats.description + "</color>\n\n";

      if (game.SelectedBuilding != null) {
        int originalCost = game.SelectedBuilding.GetComponent<CharacterStats>().Cost;
        int need = characterStats.Cost - originalCost;
        changeButton.interactable = true;
        if (need >= 0) {
          buildingDetailText.text += "<color=red>需要金錢 : </color><color=yellow>" + need + "</color>\n\n";
        } else {
          buildingDetailText.text += "<color=green>取回金錢 : </color><color=yellow>" + -need + "</color>\n\n";
        }
      } else {
        changeButton.interactable = false;

      }
      /*
      if (characterStats.BuildingID == GameConstants.BuildingID.SLOWING_DEVICE) {
        buildingDetailText.text += "減緩 " + (characterStats.Damage * 100).ToString("0.00") + "% 移動速度\n";
      } else {
        buildingDetailText.text += "傷害 : " + characterStats.Damage + "\n";
      }
      buildingDetailText.text += "攻擊範圍 : " + characterStats.AttackingRange + "\n";
      */
    } else {
      buildingIconImage.sprite = null;
      buildingDetailText.text = "";
    }
  }
}