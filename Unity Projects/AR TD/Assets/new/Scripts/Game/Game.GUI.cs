using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public partial class Game : MonoBehaviour {

  public GameObject HUDCanvas;
  public GameObject basicButtonCanvas;
  public GameObject pauseMenuCanvas;
  public GameObject audioMenuCanvas;

  public GameObject submitScoreCanvas;

  public GameObject buildingStatsCanvas;

  public GameObject nameInputField;

  public GameObject buttonTemplate;

  private List<GameObject> technologyButtons;

  public void OnUpgradeButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    //AudioManager.PlayAudioClip(researchSound);
    UpgradeBuilding(selectedBuilding);
  }

  public void OnCombinateButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    AudioManager.PlayAudioClip(buttonSound);
    Combinate();
  }

  public void OnPauseButtonClick() {
    AudioManager.PlayAudioClip(buttonSound);
    Pause();
  }

  public void OnBackToGameButtonClick() {
    AudioManager.PlayAudioClip(buttonSound);
    BackToGame();
  }

  public void OnExitButtonClick() {
    AudioManager.PlayAudioClip(buttonSound);
    Exit();
  }

  private void UpdateCanvas() {

    if (!initialized) {
      return;
    }

    bool isGameOver = (gameState == GameConstants.GameState.FINISHED) || (gameState == GameConstants.GameState.LOSED);

    if (isGameOver) {
      submitScoreCanvas.SetActive(true);
      basicButtonCanvas.SetActive(false);
      pauseMenuCanvas.SetActive(false);
      audioMenuCanvas.SetActive(false);
      return;
    }

    basicButtonCanvas.SetActive(playerState == GameConstants.PlayerState.IDLE && systemState == GameConstants.SystemState.PLAYING);
    pauseMenuCanvas.SetActive(systemState == GameConstants.SystemState.PAUSE_MENU);
    audioMenuCanvas.SetActive(systemState == GameConstants.SystemState.AUDIO_MENU);
  }

}
