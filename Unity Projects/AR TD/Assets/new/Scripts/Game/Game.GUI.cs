using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public partial class Game : MonoBehaviour {

  public GameObject HUDCanvas;
  public GameObject basicButtonCanvas;
  public GameObject techonologyListCanvas;
  public GameObject pauseMenuCanvas;
  public GameObject audioMenuCanvas;

  public GameObject submitScoreCanvas;

  public GameObject technologyDetailCanvas;

  public GameObject buildingStatsCanvas;

  public GameObject nameInputField;

  public GameObject buttonTemplate;

  private List<GameObject> technologyButtons;

  public void OnViewTechnologyListButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    AudioManager.PlayAudioClip(buttonSound);
    ViewTechnologyList();
  }

  public void OnResearchButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    int technologyCost = ViewingTechnology.Cost;
    if (money >= technologyCost) {
      AudioManager.PlayAudioClip(researchSound);

      ResearchTechnology();

      viewingTechnologyIndex = -1;

      InstantiateTechnologyButton();
    } else {
      AudioManager.PlayAudioClip(errorSound);
      MessageManager.AddMessage("需要更多金錢");
    }
  }

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

  public void OnSellButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    AudioManager.PlayAudioClip(sellSound);
    Sell();
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

  public void OnTechnologyListButtonClick(int i) {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    AudioManager.PlayAudioClip(buttonSound);
    viewingTechnologyIndex = i;
  }

  private void InstantiateTechnologyButton() {

    if (technologyButtons == null) {
      technologyButtons = new List<GameObject>();
    } else {
      for (int i = 0; i < technologyButtons.Count; ++i) {
        Destroy(technologyButtons[i]);
      }
      technologyButtons.Clear();
    }

    for (int i = 0; i < technologyManager.AvailableTechnology.Count; ++i) {
      GameObject button = Instantiate(buttonTemplate) as GameObject;
      button.transform.SetParent(techonologyListCanvas.transform);

      button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
      button.GetComponent<RectTransform>().localPosition = new Vector3(-380 + button.GetComponent<RectTransform>().sizeDelta.x * i * 1.1f, -150, 0);

      int buttonIndex = i; // Delegate is capturing a reference to the variable i
      button.GetComponent<Button>().onClick.AddListener(delegate {
        OnTechnologyListButtonClick(buttonIndex);
      });

      string technologyName = technologyManager.AvailableTechnology[i].Name;
      button.transform.GetChild(0).GetComponent<Text>().text = technologyName + "(" + (i + 1) + ")";

      technologyButtons.Add(button);
    }
  }

  private void UpdateCanvas() {

    if (!initialized) {
      return;
    }

    bool isGameOver = (gameState == GameConstants.GameState.FINISHED) || (gameState == GameConstants.GameState.LOSED);

    if (isGameOver) {
      submitScoreCanvas.SetActive(true);
      basicButtonCanvas.SetActive(false);
      techonologyListCanvas.SetActive(false);
      pauseMenuCanvas.SetActive(false);
      audioMenuCanvas.SetActive(false);
      technologyDetailCanvas.SetActive(false);
      return;
    }

    basicButtonCanvas.SetActive((playerState == GameConstants.PlayerState.IDLE
                              || playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST)
                              && systemState == GameConstants.SystemState.PLAYING);
    techonologyListCanvas.SetActive(playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST && systemState == GameConstants.SystemState.PLAYING);
    pauseMenuCanvas.SetActive(systemState == GameConstants.SystemState.PAUSE_MENU);
    audioMenuCanvas.SetActive(systemState == GameConstants.SystemState.AUDIO_MENU);

    if (ViewingTechnologyIndex >= 0 && ViewingTechnologyIndex < technologyManager.AvailableTechnology.Count) {
      technologyDetailCanvas.SetActive(playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST
                                      && systemState == GameConstants.SystemState.PLAYING);
    } else {
      technologyDetailCanvas.SetActive(false);
    }

  }

  private void InitializeUI() {
    InstantiateTechnologyButton();
  }

}
