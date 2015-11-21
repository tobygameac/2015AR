using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public partial class Game : MonoBehaviour {

  [SerializeField]
  private GameConstants.GameMode gameMode;
  public GameConstants.GameMode GameMode {
    get {
      return gameMode;
    }
  }

  // Audio
  public AudioClip backgroundMusic;
  public AudioClip finishedMusic;

  public AudioClip buttonSound;
  public AudioClip buildSound;
  public AudioClip errorSound;
  public AudioClip deniedSound;
  public AudioClip sellSound;
  public AudioClip researchSound;

  public GameObject selectedBuildingHighlightObject;

  // Building
  public GameObject coreGameObject;

  [SerializeField]
  private List<GameObject> buildingList;
  public List<GameObject> BuildingList {
    get {
      return buildingList;
    }
  }

  [SerializeField]
  private List<GameObject> initialBuildingList;

  [SerializeField]
  private int initialBuildingNumber;
  public int InitialBuildingNumber {
    get {
      return initialBuildingNumber;
    }
  }

  private int currentBuildingNumber;
  public int CurrentBuildingNumber {
    get {
      return currentBuildingNumber;
    }
  }

  [SerializeField]
  private int maxBuildingNumber;
  public int MaxBuildingNumber {
    get {
      return maxBuildingNumber;
    }
  }

  public LayerMask buildingLayerMask;
  private GameObject lastHoverBuilding;
  private GameObject _selectedBuilding;
  private GameObject selectedBuilding {
    get {
      return _selectedBuilding;
    }
    set {
      // Disable the range displayer of previous building
      if (_selectedBuilding != null) {
        _selectedBuilding.GetComponent<CharacterStats>().RangeDisplayer.SetActive(false);
      }
      _selectedBuilding = value;
      if (_selectedBuilding != null) {
        _selectedBuilding.GetComponent<CharacterStats>().RangeDisplayer.SetActive(true);
        selectedBuildingHighlightObject.transform.position = _selectedBuilding.transform.position;
      }
      buildingStatsCanvas.SetActive(value != null);
      selectedBuildingHighlightObject.SetActive(value != null);
    }
  }
  public GameObject SelectedBuilding {
    get {
      return _selectedBuilding;
    }
  }

  private bool isDraggingBuilding;

  private Vector3 selectedBuildingPositionOnScreen;
  private Vector3 selectedBuildingPositionOffset;

  // Technology
  private static TechnologyManager technologyManager;
  private int _viewingTechnologyIndex;
  private int viewingTechnologyIndex {
    get {
      return _viewingTechnologyIndex;
    }
    set {
      _viewingTechnologyIndex = value;
      if (value >= 0 && value < technologyManager.AvailableTechnology.Count) {
        technologyDetailCanvas.SetActive(true);
        _viewingTechnology = technologyManager.AvailableTechnology[viewingTechnologyIndex];
      } else {
        technologyDetailCanvas.SetActive(false);
        _viewingTechnology = null;
      }
    }
  }
  public int ViewingTechnologyIndex {
    get {
      return _viewingTechnologyIndex;
    }
  }

  public Technology _viewingTechnology;
  public Technology ViewingTechnology {
    get {
      return _viewingTechnology;
    }
  }

  public GameObject freezingLevel1Effect;
  public GameObject freezingLevel2Effect;
  public GameObject freezingLevel3Effect;

  // Game Stats
  [SerializeField]
  private int basicmoney;
  private int money;
  public int Money {
    get {
      return money;
    }
  }

  private bool initialized;

  private bool scoreSubmitted;

  public bool HasTechnology(GameConstants.TechnologyID technologyID) {
    return technologyManager.HasTechnology(technologyID);
  }

  public void AddMoney(int moneyToAdd) {
    money += moneyToAdd;
  }

  public void SubmitScore(int score) {
    if (scoreSubmitted) {
      return;
    }
    if (gameMode == GameConstants.GameMode.SURVIVAL_NORMAL) {
      string name = nameInputField.GetComponent<InputField>().text;
      if (name.Length > 0) {
        scoreSubmitted = true;
        StartCoroutine(ScoreboardManager.PostScore(gameMode, name, score, true));
      } else {
        MessageManager.AddMessage("請輸入名稱");
        AudioManager.PlayAudioClip(errorSound);
      }
    }
  }

  void Start() {
    initialized = false;
    InitializeGame();
    initialized = true;
  }

  void Update() {

    if (systemState != GameConstants.SystemState.PLAYING) {
      return;
    }

    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit raycastHit;

    // Hover
    if (Physics.Raycast(ray, out raycastHit, 1000, buildingLayerMask)) {
      if (raycastHit.collider.gameObject.transform.parent != null) {
        // Find a real building object
        do {
          lastHoverBuilding = raycastHit.collider.transform.parent.gameObject;
        } while (lastHoverBuilding.transform.parent != null);
      } else {
        lastHoverBuilding = raycastHit.collider.gameObject;
      }
    } else {
      lastHoverBuilding = null;
    }


    GameObject newBuilding;

    if (!EventSystem.current.IsPointerOverGameObject()) {
      // Left click
      if (Input.GetMouseButtonDown(0)) {
        if (lastHoverBuilding != null) {
          if (playerState == GameConstants.PlayerState.COMBINATING_BUILDINGS) {
            if (lastHoverBuilding == selectedBuilding) {
              MessageManager.AddMessage("請選擇本身以外的裝置進行組合");
              return;
            }
            CharacterStats buildingStats1 = selectedBuilding.GetComponent<CharacterStats>();
            CharacterStats buildingStats2 = lastHoverBuilding.GetComponent<CharacterStats>();
            GameConstants.BuildingID buildingID1 = buildingStats1.BuildingID;
            GameConstants.BuildingID buildingID2 = buildingStats2.BuildingID;
            newBuilding = CombinationTable.GetCombinationObject(buildingID1, buildingID2);
            if (newBuilding != null) {
              if (buildingStats1.NextLevel != null || buildingStats2.NextLevel != null) {
                AudioManager.PlayAudioClip(errorSound);
                MessageManager.AddMessage("需將兩個裝置都升級到最高等級才能進行組合");
              } else {
                // Build new building on the position of selected building
                newBuilding = Instantiate(newBuilding, selectedBuilding.transform.position/* + new Vector3(0, 1, 0) */, Quaternion.identity) as GameObject;
                CharacterStats newBuildingStats = newBuilding.GetComponent<CharacterStats>();
                newBuildingStats.UnitKilled = buildingStats1.UnitKilled + buildingStats2.UnitKilled;
                newBuildingStats.DamageModifier = buildingStats1.DamageModifier + buildingStats2.DamageModifier;

                // Adjust cost, prevent from money laundering
                int originalCost = buildingStats1.Cost + buildingStats2.Cost;
                if (originalCost < newBuildingStats.Cost) {
                  newBuildingStats.Cost = originalCost;
                }

                // Clear original building
                Destroy(selectedBuilding);
                Destroy(lastHoverBuilding);

                selectedBuilding = newBuilding;
                --currentBuildingNumber;

                AudioManager.PlayAudioClip(buildSound);

                MessageManager.AddMessage("將 " + GameConstants.NameOfBuildingID[(int)buildingID1] + " 與 " + GameConstants.NameOfBuildingID[(int)buildingID2] + " 進行組合");
                MessageManager.AddMessage("組合完畢 : " + GameConstants.NameOfBuildingID[(int)newBuildingStats.BuildingID]);

                playerState = GameConstants.PlayerState.IDLE;
              }
            } else {
              AudioManager.PlayAudioClip(errorSound);
              MessageManager.AddMessage("無法將 " + GameConstants.NameOfBuildingID[(int)buildingID1] + " 與 " + GameConstants.NameOfBuildingID[(int)buildingID2] + " 進行組合");
            }
            return;
          }
          if (playerState == GameConstants.PlayerState.IDLE) {
            selectedBuilding = lastHoverBuilding;
            selectedBuildingPositionOnScreen = Camera.main.WorldToScreenPoint(selectedBuilding.transform.position);
            selectedBuildingPositionOffset = selectedBuilding.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectedBuildingPositionOnScreen.z));
            isDraggingBuilding = true;
          }
        }

        if (lastHoverBuilding == null) {

          if (playerState == GameConstants.PlayerState.COMBINATING_BUILDINGS) {
            AudioManager.PlayAudioClip(errorSound);
            MessageManager.AddMessage("請選擇正確的目標");
            playerState = GameConstants.PlayerState.IDLE;
            return;
          }

          lastHoverBuilding = selectedBuilding = null;
        }

        if (playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
          ViewTechnologyList();
        }
      }

      if (Input.GetMouseButtonUp(0)) {
        isDraggingBuilding = false;
      }
    }

    if (isDraggingBuilding) {
      if (selectedBuilding != null) {
        Vector3 cursorPositionOnScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectedBuildingPositionOnScreen.z);
        Vector3 cursorPositionInWorld = Camera.main.ScreenToWorldPoint(cursorPositionOnScreen) + selectedBuildingPositionOffset;
        Vector3 newBuildingPosition = new Vector3(cursorPositionInWorld.x, selectedBuilding.transform.position.y, cursorPositionInWorld.z);

        SetBuildingPosition(selectedBuilding, newBuildingPosition);
      }
    }

    // Esc
    if (Input.GetKeyDown(KeyCode.Escape)) {
      if (playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
        ViewTechnologyList();
      } else if (selectedBuilding != null) {
        if (playerState == GameConstants.PlayerState.COMBINATING_BUILDINGS) {
          playerState = GameConstants.PlayerState.IDLE;
        }
        lastHoverBuilding = selectedBuilding = null;
      } else {
        /*
        OnPauseButtonClick();
        */
      }
    }

    // Cheat
    // Cheat
    // Cheat
    if (Input.GetKeyDown(KeyCode.F10)) {
      money += 100000000;
    }
    // Cheat
    // Cheat
    // Cheat
    // Cheat

    // Upgrade
    if (Input.GetKeyDown(KeyCode.U)) {
      if (selectedBuilding != null && HasTechnology(GameConstants.TechnologyID.UPGRADE)) {
        if (selectedBuilding.GetComponent<CharacterStats>().NextLevel != null) {
          OnUpgradeButtonClick();
        }
      }
    }

    // Combinate
    if (Input.GetKeyDown(KeyCode.C)) {
      if (selectedBuilding != null && HasTechnology(GameConstants.TechnologyID.COMBINATE)) {
        if (selectedBuilding.GetComponent<CharacterStats>().NextLevel == null) {
          OnCombinateButtonClick();
        }
      }
    }

    if (Input.GetKeyDown(KeyCode.R)) {
      if (playerState == GameConstants.PlayerState.IDLE
        || playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
        OnViewTechnologyListButtonClick();
      }
    }

    if (playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
      for (int i = 0; i < technologyManager.AvailableTechnology.Count; ++i) {
        if (Input.GetKeyDown(KeyCode.Keypad1 + i) || Input.GetKeyUp(KeyCode.Alpha1 + i)) {
          OnTechnologyListButtonClick(i);
        }
      }
    }

  }

  private void SetBuildingPosition(GameObject movedBuilding, Vector3 targetPosition) {
    movedBuilding.transform.position = targetPosition;

    if (movedBuilding == selectedBuilding) {
      selectedBuildingHighlightObject.transform.position = targetPosition;
    }
  }

  private void ViewTechnologyList() {
    lastHoverBuilding = selectedBuilding = null;

    if (playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
      playerState = GameConstants.PlayerState.IDLE;
    } else {
      playerState = GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST;
    }
  }

  private void Upgrade() {

    if (selectedBuilding.GetComponent<CharacterStats>().NextLevel == null) {
      return;
    }

    GameObject newBuilding = selectedBuilding.GetComponent<CharacterStats>().NextLevel;
    int upgradeCost = newBuilding.GetComponent<CharacterStats>().Cost - selectedBuilding.GetComponent<CharacterStats>().Cost;
    if (money >= upgradeCost) {
      MessageManager.AddMessage("升級完成 : " + GameConstants.NameOfBuildingID[(int)selectedBuilding.GetComponent<CharacterStats>().BuildingID]);
      AudioManager.PlayAudioClip(researchSound);

      money -= upgradeCost;

      newBuilding = Instantiate(newBuilding, selectedBuilding.transform.position, selectedBuilding.transform.rotation) as GameObject;

      newBuilding.GetComponent<CharacterStats>().UnitKilled = selectedBuilding.GetComponent<CharacterStats>().UnitKilled;

      newBuilding.GetComponent<CharacterStats>().DamageModifier = selectedBuilding.GetComponent<CharacterStats>().DamageModifier;

      Destroy(selectedBuilding.gameObject);

      selectedBuilding = newBuilding;
    } else {
      AudioManager.PlayAudioClip(errorSound);
      MessageManager.AddMessage("需要更多金錢");
    }
  }

  private void Combinate() {
    playerState = GameConstants.PlayerState.COMBINATING_BUILDINGS;

    MessageManager.AddMessage("請選擇組合目標");
  }

  private void Sell() {
    MessageManager.AddMessage("拆除 : " + GameConstants.NameOfBuildingID[(int)selectedBuilding.GetComponent<CharacterStats>().BuildingID]);
    int remainingMoney = (int)(selectedBuilding.GetComponent<CharacterStats>().Cost * 0.8);
    MessageManager.AddMessage("取回 " + remainingMoney + " 金錢");
    money += remainingMoney;
    --currentBuildingNumber;
    Destroy(selectedBuilding.gameObject);

    selectedBuilding = null;
  }

  private void Pause() {
    if (systemState == GameConstants.SystemState.PAUSE_MENU) {
      systemState = GameConstants.SystemState.PLAYING;
      Time.timeScale = 1;
    } else {
      systemState = GameConstants.SystemState.PAUSE_MENU;
      Time.timeScale = 0;
    }
  }

  private void BackToGame() {
    systemState = GameConstants.SystemState.PLAYING;
    Time.timeScale = 1;
  }

  private void Exit() {
    Time.timeScale = 1;
    Application.LoadLevel("MainMenu");
  }

  private void ResearchTechnology() {
    int technologyCost = ViewingTechnology.Cost;

    if (money < technologyCost) {
      return;
    }

    money -= technologyCost;

    if (ViewingTechnology.ID == GameConstants.TechnologyID.ADDITIONAL_BUILDING_NUMBER) {
      maxBuildingNumber += GameConstants.ADDITIONAL_BUILDING_NUMBER_PER_RESEARCH;
    }

    if (ViewingTechnology.ID == GameConstants.TechnologyID.FREEZING_LEVEL1) {
      GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER = Mathf.Min(GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER, GameConstants.FREEZING_LEVEL1_MOVING_SPEED_MODIFIER);
      freezingLevel1Effect.GetComponent<ParticleSystem>().Play();
    }

    if (ViewingTechnology.ID == GameConstants.TechnologyID.FREEZING_LEVEL2) {
      GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER = GameConstants.FREEZING_LEVEL2_MOVING_SPEED_MODIFIER;
      GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER = Mathf.Min(GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER, GameConstants.FREEZING_LEVEL2_MOVING_SPEED_MODIFIER);
      freezingLevel1Effect.GetComponent<ParticleSystem>().Stop();
      freezingLevel2Effect.GetComponent<ParticleSystem>().Play();
    }

    if (ViewingTechnology.ID == GameConstants.TechnologyID.FREEZING_LEVEL3) {
      GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER = Mathf.Min(GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER, GameConstants.FREEZING_LEVEL3_MOVING_SPEED_MODIFIER);
      freezingLevel1Effect.GetComponent<ParticleSystem>().Stop();
      freezingLevel2Effect.GetComponent<ParticleSystem>().Stop();
      freezingLevel3Effect.GetComponent<ParticleSystem>().Play();
    }

    if (ViewingTechnology.ID == GameConstants.TechnologyID.LAST_STAND) {
      GameConstants.ADDITIONAL_TIME_BY_LAST_STAND += GameConstants.LAST_STAND_ADDITIONAL_TIME;
    }

    MessageManager.AddMessage("研發完成 : " + ViewingTechnology.Name);
    technologyManager.ResearchTechnology(viewingTechnologyIndex);
    for (int i = 0; i < technologyManager.NewTechnology.Count; ++i) {
      MessageManager.AddMessage("發現新科技 : " + technologyManager.NewTechnology[i].Name);
    }
  }

  private void InitializeGame() {
    Time.timeScale = 0;

    technologyManager = new TechnologyManager();
    technologyManager.Initiate();

    money = basicmoney;

    GameConstants.ResetModifier();

    InitializeUI();

    Time.timeScale = 1;

    scoreSubmitted = false;

    StartCoroutine(AudioManager.PlayFadeInLoopAudioClip(backgroundMusic, 10.0f));

    if (initialBuildingList.Count > 0) {
      for (int i = 0; i < initialBuildingNumber; ++i) {
        int initialBuildingIndex = Random.Range(0, initialBuildingList.Count);
        GameObject newBuilding = Instantiate(initialBuildingList[initialBuildingIndex], new Vector3(Random.Range(0, 10), 1, Random.Range(0, 10)), Quaternion.identity) as GameObject;

        GameConstants.BuildingID newBuildingID = newBuilding.GetComponent<CharacterStats>().BuildingID;
        MessageManager.AddMessage("建造完成 : " + GameConstants.NameOfBuildingID[(int)newBuildingID]);

        ++currentBuildingNumber;
      }
    }

    isDraggingBuilding = false;

  }

}
