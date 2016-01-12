using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class InformationText : MonoBehaviour {

  private Game game;
  private GameManager gameManager;

  private Text text;

  public float remainingTimeAlertTime = 10.0f;
  public float alertSpeed = 10.0f;

  void Start() {
    game = Camera.main.GetComponent<Game>();
    gameManager = Camera.main.GetComponent<GameManager>();
    text = GetComponent<Text>();
  }

  void Update() {
    text.text = "<size=20><color=red><b><i>" + gameManager.Score + "</i></b></color></size>\n\n";

    List<CharacterStats> coreGameObjectsStats = gameManager.CoreGameObjectsStats;
    for (int i = 0; i < coreGameObjectsStats.Count; ++i) {
      text.text += "<size=20>核心生命 : <color=red>" + coreGameObjectsStats[i].CurrentHP + "</color> / <color=red>" + coreGameObjectsStats[i].MaxHP + "</color></size>\n\n";
    }

    text.text += "金錢 : <color=yellow>" + game.Money + "</color>\n";

    //if (game.CurrentBuildingNumber == game.MaxBuildingNumber) {
    //  text.text += "裝置數量 : <color=red>" + game.CurrentBuildingNumber + "</color> / <color=red>" + game.MaxBuildingNumber + "</color>\n";
    //} else {
    //  text.text += "裝置數量 : <color=blue>" + game.CurrentBuildingNumber + "</color> / <color=blue>" + game.MaxBuildingNumber + "</color>\n";
    //}

    if (game.GameMode == GameConstants.GameMode.STORY) {
      text.text += "波數 : <color=blue>" + gameManager.CurrentWave + "</color> / <color=blue>" + gameManager.MaxWave + "</color>\n";
    } else if ((game.GameMode == GameConstants.GameMode.SURVIVAL_NORMAL) || (game.GameMode == GameConstants.GameMode.SURVIVAL_BOSS)) {
      text.text += "波數 : <color=blue>" + gameManager.CurrentWave + "</color>\n";
    }
    text.text += "\n";

    if ((game.GameMode == GameConstants.GameMode.SURVIVAL_NORMAL) || (game.GameMode == GameConstants.GameMode.SURVIVAL_BOSS)) {
      if (gameManager.GameState == GameConstants.GameState.WAIT_FOR_THE_NEXT_WAVE) {
        int remainingTime = (int)(gameManager.RestingTimeBetweenWaves - gameManager.RestedTime);
        if (gameManager.CurrentWave == 0) {
          remainingTime = (int)(gameManager.RestingTimeBeforeStart - gameManager.RestedTime);
        }
        text.text += "<color=black>敵軍將於 ";
        if (remainingTime >= remainingTimeAlertTime) {
          text.text += "<color=blue>";
        } else {
          int alpha = (int)((Mathf.Sin(Time.time * alertSpeed) + 1) / 2 * 255);
          text.text += "<color=#0000ff" + alpha.ToString("X2") + ">";
        }
        text.text += remainingTime + "</color> 秒後進行下一波攻勢</color>\n\n";
      } else if (gameManager.GameState == GameConstants.GameState.MIDDLE_OF_THE_WAVE) {
        //text.text += "場上敵軍數量 : <color=blue>" + gameManager.NumberOfEnemiesOnMap + "</color>\n";
        text.text += "尚未出現敵軍數量 : <color=blue>" + gameManager.NumberOfEnemiesToGenerate + "</color>\n\n";
        //int remainingTime = (int)gameManager.RemainingTimeOfCurrentWave;
        //text.text += "<color=black>下一波敵軍將於 ";
        //if (remainingTime >= remainingTimeAlertTime) {
        //  text.text += "<color=red>";
        //} else {
        //  int alpha = (int)((Mathf.Sin(Time.time * alertSpeed) + 1) / 2 * 255);
        //  text.text += "<color=#ff0000" + alpha.ToString("X2") + ">";
        //}
        //text.text += remainingTime + "</color> 秒內入侵</color>\n\n";
      } else if ((gameManager.GameState == GameConstants.GameState.FINISHED) || (gameManager.GameState == GameConstants.GameState.LOSED)) {
        text.text += "遊戲結束\n";
      }
    }
  }
}
