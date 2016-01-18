using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TechonologyDetailDisplayer : MonoBehaviour {

  public Sprite[] iconSprites;

  public GameObject technologyIcon;
  private Image technologyIconImage;

  public GameObject technologyDetail;
  private Text technologyDetailText;

  private static Game game;

  private Technology previousViewingTechnology;

  void Start() {
    technologyIconImage = technologyIcon.GetComponent<Image>();
    technologyDetailText = technologyDetail.GetComponent<Text>();

    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }

    previousViewingTechnology = null;
  }

  void Update() {
    if (previousViewingTechnology != game.ViewingTechnology) {
      UpdateTechnologyDetail();
      previousViewingTechnology = game.ViewingTechnology;
    }
  }

  void UpdateTechnologyDetail() {
    if (game.ViewingTechnology != null) {
      if (iconSprites.Length > (int)game.ViewingTechnology.ID) {
        technologyIconImage.sprite = iconSprites[(int)game.ViewingTechnology.ID];

        //  switch (game.ViewingTechnology.ID) {
        //    case GameConstants.TechnologyID.UPGRADE:
        //      technologyIconImage.color = new Color(0, 128, 0);
        //      break;
        //    case GameConstants.TechnologyID.COMBINATE:
        //      technologyIconImage.color = new Color(128, 128, 0);
        //      break;
        //    case GameConstants.TechnologyID.SELF_LEARNING:
        //      technologyIconImage.color = new Color(128, 128, 0);
        //      break;
        //    case GameConstants.TechnologyID.SELF_HEALING:
        //      technologyIconImage.color = new Color(128, 0, 0);
        //      break;
        //    case GameConstants.TechnologyID.FREEZING_LEVEL1:
        //    case GameConstants.TechnologyID.FREEZING_LEVEL2:
        //    case GameConstants.TechnologyID.FREEZING_LEVEL3:
        //      technologyIconImage.color = new Color(0, 0, 128);
        //      break;
        //  }
      }

      technologyDetailText.text = "需要金錢 : <color=yellow>" + game.ViewingTechnology.Cost + "</color>\n";
      technologyDetailText.text += "<color=lime>" + GameConstants.DetailOfTechnologyID[(int)game.ViewingTechnology.ID] + "</color>";

    } else {
      technologyIconImage.sprite = null;
      technologyDetailText.text = "";
    }
  }
}
