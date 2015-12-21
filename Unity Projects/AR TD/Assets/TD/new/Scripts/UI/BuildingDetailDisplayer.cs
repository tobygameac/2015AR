using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuildingDetailDisplayer : MonoBehaviour {

  public Sprite[] iconSprites;

  public GameObject buildingIcon;
  private Image buildingIconImage;

  public GameObject buildingDetail;
  private Text buildingDetailText;

  private Game game;

  private int previousViewingBuildingIndex;

  void Start() {
    buildingIconImage = buildingIcon.GetComponent<Image>();
    buildingDetailText = buildingDetail.GetComponent<Text>();

    game = Camera.main.GetComponent<Game>();

    previousViewingBuildingIndex = -1;
  }

  void Update() {
    if (previousViewingBuildingIndex != game.ViewingBuildingIndex) {
      UpdateBuildingDetail();
      previousViewingBuildingIndex = game.ViewingBuildingIndex;
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

      buildingDetailText.text = "<color=lime>";
      //buildingDetailText.text += GameConstants.NameOfBuildingID[(int)characterStats.BuildingID] + "\n\n";

      buildingDetailText.text += characterStats.description + "</color>\n\n";

      buildingDetailText.text += "<color=red>需要金錢 : </color><color=yellow>" + characterStats.Cost + "</color>\n\n";
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