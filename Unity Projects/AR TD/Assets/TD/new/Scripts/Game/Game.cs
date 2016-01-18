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

  public Transform gameSceneParentTransform;

  [SerializeField]
  private GameObject zones;

  [SerializeField]
  private float mapWidth;

  [SerializeField]
  private float mapHeight;

  // Audio
  public AudioClip backgroundMusic;
  public AudioClip finishedMusic;

  public AudioClip buttonSound;
  public AudioClip buildSound;
  public AudioClip errorSound;
  public AudioClip deniedSound;
  public AudioClip researchSound;

  public GameObject selectedBuildingHighlightObject;

  // Building
  [SerializeField]
  private List<GameObject> basicBuildingList;
  public List<GameObject> BasicBuildingList {
    get {
      return basicBuildingList;
    }
  }

  [SerializeField]
  private int initialBuildingNumber;
  public int InitialBuildingNumber {
    get {
      return initialBuildingNumber;
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

  public GameObject selectedBuilding {
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
        //selectedBuildingHighlightObject.transform.position = _selectedBuilding.transform.position;
      }
      buildingStatsCanvas.SetActive(value != null);
      //selectedBuildingHighlightObject.SetActive(value != null);
    }
  }

  public GameObject SelectedBuilding {
    get {
      return _selectedBuilding;
    }
  }

  //private bool isDraggingBuilding;

  private Vector3 selectedBuildingPositionOnScreen;
  private Vector3 selectedBuildingPositionOffset;

  private int _viewingBuildingIndex;
  private int viewingBuildingIndex {
    get {
      return _viewingBuildingIndex;
    }
    set {
      _viewingBuildingIndex = value;

      if (value >= 0 && value < basicBuildingList.Count) {
        _viewingBuildingToChange = BasicBuildingList[value];
        buildingDetailCanvas.SetActive(true);
      } else {
        buildingDetailCanvas.SetActive(false);
        _viewingBuildingToChange = null;
      }
    }
  }
  public int ViewingBuildingIndex {
    get {
      return _viewingBuildingIndex;
    }
  }

  private GameObject _viewingBuildingToChange;
  public GameObject ViewingBuildingToChange {
    get {
      return _viewingBuildingToChange;
    }
  }

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
        _viewingTechnology = technologyManager.AvailableTechnology[value];
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

  private Technology _viewingTechnology;
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

  public List<Technology> AvailableTechnology() {
    return technologyManager.AvailableTechnology;
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

    if (selectedBuilding != null) {
      //selectedBuildingHighlightObject.transform.position = _selectedBuilding.transform.position;
      if (!selectedBuilding.GetComponent<Collider>().enabled) {
        selectedBuilding = null;
      }
    }

    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //RaycastHit raycastHit;

    //// Hover
    //if (Physics.Raycast(ray, out raycastHit, 1000, buildingLayerMask)) {
    //  Transform lastHoverBuildingTransform = raycastHit.collider.transform;
    //  while (lastHoverBuildingTransform.GetComponent<CharacterStats>() == null) {
    //      lastHoverBuildingTransform = lastHoverBuildingTransform.parent;
    //  }
    //  lastHoverBuilding = lastHoverBuildingTransform.gameObject;
    //} else {
    //  lastHoverBuilding = null;
    //}

    //if (!EventSystem.current.IsPointerOverGameObject()) {
    //  // Left button down
    //  if (Input.GetMouseButtonDown(0)) {
    //    if (lastHoverBuilding != null) {
    //      if (playerState == GameConstants.PlayerState.COMBINATING_BUILDINGS) {
    //        if (CombinateBuilding(selectedBuilding, lastHoverBuilding)) {
    //          playerState = GameConstants.PlayerState.IDLE;
    //        }
    //        return;
    //      }
    //      if (playerState == GameConstants.PlayerState.IDLE) {
    //        selectedBuilding = lastHoverBuilding;
    //        selectedBuildingPositionOnScreen = Camera.main.WorldToScreenPoint(selectedBuilding.transform.position);
    //        selectedBuildingPositionOffset = selectedBuilding.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectedBuildingPositionOnScreen.z));
    //        isDraggingBuilding = true;
    //      }
    //    }

    //    if (lastHoverBuilding == null) {

    //      if (playerState == GameConstants.PlayerState.COMBINATING_BUILDINGS) {
    //        AudioManager.PlayAudioClip(errorSound);
    //        MessageManager.AddMessage("請選擇正確的目標");
    //        playerState = GameConstants.PlayerState.IDLE;
    //        return;
    //      }

    //      lastHoverBuilding = selectedBuilding = null;
    //    }

    //  }

    //  // Left button up
    //  if (Input.GetMouseButtonUp(0)) {
    //    isDraggingBuilding = false;
    //  }
    //}

    //if (isDraggingBuilding) {
    //  if (selectedBuilding != null) {
    //    Vector3 cursorPositionOnScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectedBuildingPositionOnScreen.z);
    //    Vector3 cursorPositionInWorld = Camera.main.ScreenToWorldPoint(cursorPositionOnScreen) + selectedBuildingPositionOffset;
    //    Vector3 newBuildingPosition = new Vector3(cursorPositionInWorld.x, selectedBuilding.transform.position.y, cursorPositionInWorld.z);

    //    SetBuildingPosition(selectedBuilding, newBuildingPosition);
    //  }
    //}

    // Esc
    if (Input.GetKeyDown(KeyCode.Escape)) {
      if (selectedBuilding != null) {
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

    if (Input.GetKeyDown(KeyCode.B)) {
      if (playerState == GameConstants.PlayerState.IDLE
        || playerState == GameConstants.PlayerState.VIEWING_BUILDING_LIST
        || playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
        OnViewBuildingListButtonClick();
      }
    }

    if (Input.GetKeyDown(KeyCode.R)) {
      if (playerState == GameConstants.PlayerState.IDLE
        || playerState == GameConstants.PlayerState.VIEWING_BUILDING_LIST
        || playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
        OnViewTechnologyListButtonClick();
      }
    }

    if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
      if (viewingTechnologyIndex >= 0) {
        OnResearchButtonClick();
      }

      if (viewingBuildingIndex >= 0) {
        OnChangeButtonClick();
      }
    }

    if (playerState == GameConstants.PlayerState.VIEWING_BUILDING_LIST) {
      for (int i = 0; i < basicBuildingList.Count; ++i) {
        if (Input.GetKeyDown(KeyCode.Keypad1 + i) || Input.GetKeyUp(KeyCode.Alpha1 + i)) {
          OnBuildingListButtonClick(i);
        }
      }
    } else if (playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
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

  private void ViewBuildingList() {
    //lastHoverBuilding = selectedBuilding = null;
    viewingBuildingIndex = viewingTechnologyIndex = -1;

    viewingBuildingIndex = 0;

    AudioManager.PlayAudioClip(buttonSound);

    if (playerState == GameConstants.PlayerState.VIEWING_BUILDING_LIST) {
      playerState = GameConstants.PlayerState.IDLE;
    } else {
      playerState = GameConstants.PlayerState.VIEWING_BUILDING_LIST;
    }

  }

  private void ViewTechnologyList() {
    //lastHoverBuilding = selectedBuilding = null;
    viewingBuildingIndex = viewingTechnologyIndex = -1;

    viewingTechnologyIndex = 0;

    AudioManager.PlayAudioClip(buttonSound);

    if (playerState == GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST) {
      playerState = GameConstants.PlayerState.IDLE;
    } else {
      playerState = GameConstants.PlayerState.VIEWING_TECHNOLOGY_LIST;
    }

  }


  public bool UpgradeBuilding(GameObject buildingToUpgrade) {
    if (buildingToUpgrade.GetComponent<CharacterStats>().NextLevel == null) {
      return false;
    }

    if (!HasTechnology(GameConstants.TechnologyID.UPGRADE)) {
      MessageManager.AddMessage("須先研發升級技術");
      return false;
    }

    GameObject newBuilding = buildingToUpgrade.GetComponent<CharacterStats>().NextLevel;

    int upgradeCost = newBuilding.GetComponent<CharacterStats>().Cost - buildingToUpgrade.GetComponent<CharacterStats>().Cost;

    if (money >= upgradeCost) {
      MessageManager.AddMessage("升級完成 : " + GameConstants.NameOfBuildingID[(int)buildingToUpgrade.GetComponent<CharacterStats>().BuildingID]);
      AudioManager.PlayAudioClip(researchSound);

      money -= upgradeCost;

      newBuilding = Instantiate(newBuilding, buildingToUpgrade.transform.position, buildingToUpgrade.transform.rotation) as GameObject;

      newBuilding.transform.parent = buildingToUpgrade.transform.parent;

      newBuilding.GetComponent<CharacterStats>().UnitKilled = buildingToUpgrade.GetComponent<CharacterStats>().UnitKilled;

      newBuilding.GetComponent<CharacterStats>().DamageModifier = buildingToUpgrade.GetComponent<CharacterStats>().DamageModifier;

      selectedBuilding = newBuilding;

      //// Update building list
      //int buildingToUpgradeIndex = buildingListIndexMapping[buildingToUpgrade];
      //buildingListIndexMapping[newBuilding] = buildingToUpgradeIndex;
      //buildingList[buildingToUpgradeIndex] = newBuilding;

      //buildingListIndexMapping.Remove(buildingToUpgrade);

      Destroy(buildingToUpgrade);

      return true;
    }

    AudioManager.PlayAudioClip(errorSound);
    MessageManager.AddMessage("需要更多金錢");

    return false;
  }

  public bool ChangeBuilding(GameObject buildingToChange, GameObject newBuilding) {

    if (buildingToChange == null) {
      AudioManager.PlayAudioClip(errorSound);
      MessageManager.AddMessage("請選擇需轉換建築");
      return false;
    }

    if (newBuilding == null) {
      AudioManager.PlayAudioClip(errorSound);
      MessageManager.AddMessage("請選擇轉換目標");
      return false;
    }

    int originalBuildingID = (int)buildingToChange.GetComponent<CharacterStats>().BuildingID;
    int newBuildingID = (int)newBuilding.GetComponent<CharacterStats>().BuildingID;

    if (originalBuildingID == newBuildingID) {
      AudioManager.PlayAudioClip(errorSound);
      MessageManager.AddMessage("同一建築無法互相轉換");
      return false;
    }

    int changeCost = newBuilding.GetComponent<CharacterStats>().Cost - buildingToChange.GetComponent<CharacterStats>().Cost;

    if (money >= changeCost) {
      MessageManager.AddMessage("將" + GameConstants.NameOfBuildingID[originalBuildingID] + "轉換為" + GameConstants.NameOfBuildingID[newBuildingID]);
      AudioManager.PlayAudioClip(buildSound);

      money -= changeCost;

      MessageManager.AddMessage((changeCost < 0) ? ("取回" + -changeCost + "金錢") : ("花費" + changeCost + "金錢"));

      newBuilding = Instantiate(newBuilding, buildingToChange.transform.position, buildingToChange.transform.rotation) as GameObject;

      newBuilding.transform.parent = buildingToChange.transform.parent;

      newBuilding.GetComponent<CharacterStats>().UnitKilled = buildingToChange.GetComponent<CharacterStats>().UnitKilled;

      newBuilding.GetComponent<CharacterStats>().DamageModifier = buildingToChange.GetComponent<CharacterStats>().DamageModifier;

      selectedBuilding = newBuilding;

      //// Update building list
      //int buildingToUpgradeIndex = buildingListIndexMapping[buildingToChange];
      //buildingListIndexMapping[newBuilding] = buildingToUpgradeIndex;
      //buildingList[buildingToUpgradeIndex] = newBuilding;

      //buildingListIndexMapping.Remove(buildingToChange);

      Destroy(buildingToChange);

      viewingBuildingIndex = -1;

      return true;
    }

    AudioManager.PlayAudioClip(errorSound);
    MessageManager.AddMessage("需要更多金錢");

    return false;
  }

  public bool CombinateBuilding(GameObject building1, GameObject building2) {
    if (building1 == building2) {
      MessageManager.AddMessage("請選擇本身以外的裝置進行組合");
      return false;
    }
    CharacterStats buildingStats1 = building1.GetComponent<CharacterStats>();
    CharacterStats buildingStats2 = building2.GetComponent<CharacterStats>();
    GameConstants.BuildingID buildingID1 = buildingStats1.BuildingID;
    GameConstants.BuildingID buildingID2 = buildingStats2.BuildingID;
    GameObject newBuilding = CombinationTable.GetCombinationObject(buildingID1, buildingID2);
    if (newBuilding != null) {
      if (buildingStats1.NextLevel != null || buildingStats2.NextLevel != null) {
        //AudioManager.PlayAudioClip(errorSound);
        //MessageManager.AddMessage("需將兩個裝置都升級到最高等級才能進行組合");
        return false;
      }

      // Build new building on the position of building 1
      newBuilding = Instantiate(newBuilding, building1.transform.position/* + new Vector3(0, 1, 0) */, Quaternion.identity) as GameObject;

      newBuilding.transform.parent = building1.transform.parent;

      CharacterStats newBuildingStats = newBuilding.GetComponent<CharacterStats>();
      newBuildingStats.UnitKilled = buildingStats1.UnitKilled + buildingStats2.UnitKilled;
      newBuildingStats.DamageModifier = buildingStats1.DamageModifier + buildingStats2.DamageModifier;

      // Adjust cost, prevent from money laundering
      int originalCost = buildingStats1.Cost + buildingStats2.Cost;
      if (originalCost < newBuildingStats.Cost) {
        newBuildingStats.Cost = originalCost;
      }

      // Replace building 2 as a basic building
      int basicBuildingIndex = Random.Range(0, basicBuildingList.Count);
      GameObject replacedBuilding = Instantiate(basicBuildingList[basicBuildingIndex], building2.transform.position, Quaternion.identity) as GameObject;

      replacedBuilding.transform.parent = building2.transform.parent;

      AudioManager.PlayAudioClip(buildSound);

      MessageManager.AddMessage("將 " + GameConstants.NameOfBuildingID[(int)buildingID1] + " 與 " + GameConstants.NameOfBuildingID[(int)buildingID2] + " 進行組合");
      MessageManager.AddMessage("組合完畢 : " + GameConstants.NameOfBuildingID[(int)newBuildingStats.BuildingID]);

      // Update building list
      //int building1Index = buildingListIndexMapping[building1];
      //buildingListIndexMapping[newBuilding] = building1Index;
      //buildingList[building1Index] = newBuilding;

      //buildingListIndexMapping.Remove(building1);

      //int building2Index = buildingListIndexMapping[building2];
      //buildingListIndexMapping[replacedBuilding] = building2Index;
      //buildingList[building2Index] = replacedBuilding;

      //buildingListIndexMapping.Remove(building2);

      // Clear original building
      Destroy(building1);
      Destroy(building2);

      selectedBuilding = newBuilding;

      return true;
    }
    //AudioManager.PlayAudioClip(errorSound);
    //MessageManager.AddMessage("無法將 " + GameConstants.NameOfBuildingID[(int)buildingID1] + " 與 " + GameConstants.NameOfBuildingID[(int)buildingID2] + " 進行組合");
    return false;
  }

  public bool ResearchTechnology(int technologyIndex) {
    if (technologyIndex < 0 || technologyIndex >= technologyManager.AvailableTechnology.Count) {
      return false;
    }

    Technology newTechnology = technologyManager.AvailableTechnology[technologyIndex];
    int technologyCost = newTechnology.Cost;

    if (money < technologyCost) {
      AudioManager.PlayAudioClip(errorSound);
      MessageManager.AddMessage("需要更多金錢");
      return false;
    }

    money -= technologyCost;

    if (newTechnology.ID == GameConstants.TechnologyID.ADDITIONAL_BUILDING_NUMBER) {
      maxBuildingNumber += GameConstants.ADDITIONAL_BUILDING_NUMBER_PER_RESEARCH;
    }

    if (newTechnology.ID == GameConstants.TechnologyID.FREEZING_LEVEL1) {
      GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER = Mathf.Min(GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER, GameConstants.FREEZING_LEVEL1_MOVING_SPEED_MODIFIER);
      freezingLevel1Effect.GetComponent<ParticleSystem>().Play();
    }

    if (newTechnology.ID == GameConstants.TechnologyID.FREEZING_LEVEL2) {
      GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER = GameConstants.FREEZING_LEVEL2_MOVING_SPEED_MODIFIER;
      GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER = Mathf.Min(GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER, GameConstants.FREEZING_LEVEL2_MOVING_SPEED_MODIFIER);
      freezingLevel1Effect.GetComponent<ParticleSystem>().Stop();
      freezingLevel2Effect.GetComponent<ParticleSystem>().Play();
    }

    if (newTechnology.ID == GameConstants.TechnologyID.FREEZING_LEVEL3) {
      GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER = Mathf.Min(GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER, GameConstants.FREEZING_LEVEL3_MOVING_SPEED_MODIFIER);
      freezingLevel1Effect.GetComponent<ParticleSystem>().Stop();
      freezingLevel2Effect.GetComponent<ParticleSystem>().Stop();
      freezingLevel3Effect.GetComponent<ParticleSystem>().Play();
    }

    if (newTechnology.ID == GameConstants.TechnologyID.LAST_STAND) {
      GameConstants.ADDITIONAL_TIME_BY_LAST_STAND += GameConstants.LAST_STAND_ADDITIONAL_TIME;
    }

    technologyManager.ResearchTechnology(technologyIndex);

    MessageManager.AddMessage("研發完成 : " + newTechnology.Name);
    for (int i = 0; i < technologyManager.NewTechnology.Count; ++i) {
      MessageManager.AddMessage("發現新科技 : " + technologyManager.NewTechnology[i].Name);
    }

    AudioManager.PlayAudioClip(researchSound);

    viewingTechnologyIndex = 0;

    //InstantiateTechnologyButton();

    return true;
  }

  private void Combinate() {
    AudioManager.PlayAudioClip(buttonSound);

    playerState = GameConstants.PlayerState.COMBINATING_BUILDINGS;

    MessageManager.AddMessage("請選擇組合目標");
  }

  private void Pause() {
    AudioManager.PlayAudioClip(buttonSound);

    if (systemState == GameConstants.SystemState.PAUSE_MENU) {
      systemState = GameConstants.SystemState.PLAYING;
      Time.timeScale = 1;
    } else {
      systemState = GameConstants.SystemState.PAUSE_MENU;
      Time.timeScale = 0;
    }
  }

  private void BackToGame() {
    AudioManager.PlayAudioClip(buttonSound);

    systemState = GameConstants.SystemState.PLAYING;
    Time.timeScale = 1;
  }

  private void Exit() {
    AudioManager.PlayAudioClip(buttonSound);

    Time.timeScale = 1;
    Application.LoadLevel("MainMenu");
  }

  private void InitializeGame() {
    Time.timeScale = 0;

    viewingBuildingIndex = viewingTechnologyIndex = -1;

    technologyManager = new TechnologyManager();
    technologyManager.Initiate();

    money = basicmoney;

    GameConstants.ResetModifier();

    InitializeUI();

    Time.timeScale = 1;

    scoreSubmitted = false;

    StartCoroutine(AudioManager.PlayFadeInLoopAudioClip(backgroundMusic, 10.0f));

    GameObject newZones = Instantiate(zones, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    newZones.transform.parent = gameSceneParentTransform;
  }

}
