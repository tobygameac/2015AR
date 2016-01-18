using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public partial class Game : MonoBehaviour {

  public GameObject HUDCanvas;
  public GameObject basicButtonCanvas;
  public GameObject buildingListCanvas;
  public GameObject techonologyListCanvas;
  public GameObject pauseMenuCanvas;
  public GameObject audioMenuCanvas;

  public GameObject submitScoreCanvas;

  public GameObject buildingDetailCanvas;
  public GameObject technologyDetailCanvas;

  public GameObject buildingStatsCanvas;

  public GameObject nameInputField;

  public GameObject buttonTemplate;

  private List<GameObject> buildingButtons;
  private List<GameObject> technologyButtons;

  public void OnLeftButtonClick() {
    if (playerState == GameConstants.PlayerState.VIEWING_BUILDING_LIST) {
      viewingBuildingIndex = (viewingBuildingIndex - 1 + BasicBuildingList.Count) % BasicBuildingList.Count;
    }
    if (playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
      viewingTechnologyIndex = (ViewingTechnologyIndex - 1 + technologyManager.AvailableTechnology.Count) % technologyManager.AvailableTechnology.Count;
    }
  }

  public void OnRightButtonClick() {
    if (playerState == GameConstants.PlayerState.VIEWING_BUILDING_LIST) {
      viewingBuildingIndex = (viewingBuildingIndex + 1) % BasicBuildingList.Count;
    }
    if (playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
      viewingTechnologyIndex = (ViewingTechnologyIndex + 1) % technologyManager.AvailableTechnology.Count;
    }
  }

  public void OnViewBuildingListButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    ViewBuildingList();
  }

  public void OnViewTechnologyListButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    ViewTechnologyList();
  }

  public void OnChangeButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    ChangeBuilding(selectedBuilding, ViewingBuildingToChange);
  }

  public void OnResearchButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }

    ResearchTechnology(viewingTechnologyIndex);
  }

  public void OnUpgradeButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }

    UpgradeBuilding(selectedBuilding);
  }

  public void OnCombinateButtonClick() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    Combinate();
  }

  public void OnPauseButtonClick() {
    Pause();
  }

  public void OnBackToGameButtonClick() {
    BackToGame();
  }

  public void OnExitButtonClick() {
    Exit();
  }

  public void OnBuildingListButtonClick(int i) {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    AudioManager.PlayAudioClip(buttonSound);
    if (viewingBuildingIndex != i) { // Message spamming
      MessageManager.AddMessage("請選擇轉換類型");
    }
    viewingBuildingIndex = i;
  }

  public void OnTechnologyListButtonClick(int i) {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }
    AudioManager.PlayAudioClip(buttonSound);
    viewingTechnologyIndex = i;
  }

  private void InstantiateBuildingButton() {

    if (buildingButtons == null) {
      buildingButtons = new List<GameObject>();
    } else {
      for (int i = 0; i < buildingButtons.Count; ++i) {
        Destroy(buildingButtons[i]);
      }
      buildingButtons.Clear();
    }

    for (int i = 0; i < basicBuildingList.Count; ++i) {
      GameObject button = Instantiate(buttonTemplate) as GameObject;
      button.transform.SetParent(buildingListCanvas.transform);

      button.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
      button.GetComponent<RectTransform>().localPosition = new Vector3(-380 + button.GetComponent<RectTransform>().sizeDelta.x * i * 1.1f, -150, 0);

      int buttonIndex = i; // Delegate is capturing a reference to the variable i
      button.GetComponent<Button>().onClick.AddListener(delegate {
        OnBuildingListButtonClick(buttonIndex);
      });

      string buildingName = GameConstants.NameOfBuildingID[(int)basicBuildingList[i].GetComponent<CharacterStats>().BuildingID];
      button.transform.GetChild(0).GetComponent<Text>().text = buildingName + "(" + (i + 1) + ")";

      buildingButtons.Add(button);
    }

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
      buildingListCanvas.SetActive(false);
      techonologyListCanvas.SetActive(false);
      pauseMenuCanvas.SetActive(false);
      audioMenuCanvas.SetActive(false);
      technologyDetailCanvas.SetActive(false);
      return;
    }

    basicButtonCanvas.SetActive((playerState == GameConstants.PlayerState.IDLE
                               || playerState == GameConstants.PlayerState.VIEWING_BUILDING_LIST
                               || playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST)
                               && systemState == GameConstants.SystemState.PLAYING);
    buildingListCanvas.SetActive(playerState == GameConstants.PlayerState.VIEWING_BUILDING_LIST && systemState == GameConstants.SystemState.PLAYING);
    techonologyListCanvas.SetActive(playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST && systemState == GameConstants.SystemState.PLAYING);
    pauseMenuCanvas.SetActive(systemState == GameConstants.SystemState.PAUSE_MENU);
    audioMenuCanvas.SetActive(systemState == GameConstants.SystemState.AUDIO_MENU);

    if (ViewingTechnologyIndex >= 0 && ViewingTechnologyIndex < technologyManager.AvailableTechnology.Count) {
      technologyDetailCanvas.SetActive(playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST
                                      && systemState == GameConstants.SystemState.PLAYING);
    } else {
      technologyDetailCanvas.SetActive(false);
    }

    if (ViewingBuildingIndex >= 0 && ViewingBuildingIndex < basicBuildingList.Count) {
      buildingDetailCanvas.SetActive(playerState == GameConstants.PlayerState.VIEWING_BUILDING_LIST
                                    && systemState == GameConstants.SystemState.PLAYING);
    } else {
      buildingDetailCanvas.SetActive(false);
    }
  }

  private void InitializeUI() {
    //InstantiateBuildingButton();
    //InstantiateTechnologyButton();
  }

}
