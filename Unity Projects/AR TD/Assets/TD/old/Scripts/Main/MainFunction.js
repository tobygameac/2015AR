#pragma strict
#pragma downcast
import System.Collections.Generic;

// GUI
var myGUISkin : GUISkin;
var myGUITexture : Texture;
var highlightTexture : Texture;
var towerFrameTexture : Texture;
var statsHUD : Texture;
var buildingHUD : Texture;
var maskTexture : Texture;
var nurseCheer : Texture;

// Sound
var buttonSound : AudioClip;
var buildSound : AudioClip;
var errorSound : AudioClip;
var deniedSound : AudioClip;
var sellSound : AudioClip;
var techSound : AudioClip;

// Money & Research
static var money : int;
static var allResearch = new List.<Research>();
static var upgradable : boolean;
static var combinatable : boolean;
static var learnable : boolean;
var researchIndex : int;

// Game state
var isBuilding : boolean;
var isResearching : boolean;
var isCombining : boolean;
var isPause : boolean;
var isAdjusting : boolean;
var isExit : boolean;
var isFinished : boolean;
var isLosed : boolean;

// Plane
var originalMat : Material;
var hoverMat : Material;
var placementPlanesRoot : Transform;
var placementLayerMask : LayerMask;
var lastHitPlane : GameObject;

// Building
var mainBuilding : GameObject;
var allBuildings : GameObject[];
var buildingIndex : int;
var nowBuildingNumber : int;
var maxBuildingNumber : int;
var buildingLayerMask : LayerMask;
var lastHitBuilding : GameObject;
var selectingBuilding : GameObject;

// Combination Building
var fireCannon : GameObject;
var laserCannon : GameObject;
var superBurningTower : GameObject;

function Start () {
  Time.timeScale = 1;
  ShowPlacementPlanes();
  buildingIndex = researchIndex = -1;
}

function Update () {
  // Building
  var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
  var hit : RaycastHit;
  if (isBuilding) {
    lastHitBuilding = null;
    if (Physics.Raycast(ray, hit, 1000, placementLayerMask)) {
      if (lastHitPlane) {
        lastHitPlane.GetComponent.<Renderer>().material = originalMat;
      }
      lastHitPlane = hit.collider.gameObject;
      originalMat = lastHitPlane.GetComponent.<Renderer>().material;
      lastHitPlane.GetComponent.<Renderer>().material = hoverMat;
    } else {
      if (lastHitPlane) {
        lastHitPlane.GetComponent.<Renderer>().material = originalMat;
        lastHitPlane = null;
      }
    }
  } else {
    if (Physics.Raycast(ray, hit, 1000, buildingLayerMask)) {
      if (hit.collider.gameObject.transform.parent) {
        lastHitBuilding = hit.collider.gameObject.transform.parent.gameObject;
        while (lastHitBuilding.transform.parent) { // Find a real building object
          lastHitBuilding = lastHitBuilding.transform.parent.gameObject;
        }
      } else {
        lastHitBuilding = hit.collider.gameObject;
      }
    } else {
      lastHitBuilding = null;
    }
  }

  var newBuilding : GameObject;
  // Left click
  if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0) {
    if (lastHitBuilding) {
      if (isCombining) { // Combinate
        if (!(lastHitBuilding.GetComponent(Stats).combinatable)) { // Unable to combinate
          //audio.PlayOneShot(deniedSound);
          GetComponent.<AudioSource>().PlayOneShot(errorSound);
          ShowMessage("目標機械無法進行結合");
          isCombining = false;
        } else if (selectingBuilding.tag == lastHitBuilding.tag) {
          //audio.PlayOneShot(deniedSound);
          GetComponent.<AudioSource>().PlayOneShot(errorSound);
          ShowMessage("無法與相同機械進行組合");
          isCombining = false;
        } else {
          ShowMessage("將 " + selectingBuilding.tag + " 與 " + lastHitBuilding.tag + " 進行結合");
          if ((selectingBuilding.tag == "基礎機械" && lastHitBuilding.tag == "燃燒裝置") || (selectingBuilding.tag == "燃燒裝置" && lastHitBuilding.tag == "基礎機械")) {
            newBuilding = fireCannon;
            newBuilding.GetComponent(Stats).planeTaken = selectingBuilding.GetComponent(Stats).planeTaken;
            newBuilding.GetComponent(Stats).kill = selectingBuilding.GetComponent(Stats).kill + lastHitBuilding.GetComponent(Stats).kill;
            newBuilding = Instantiate(newBuilding, selectingBuilding.transform.position, Quaternion.identity);
            // newBuilding.transform.localEulerAngles = selectingBuilding.transform.localEulerAngles;
            Destroy(selectingBuilding.gameObject);
            selectingBuilding = newBuilding;
            selectingBuilding.GetComponent(RangeDemo).enabled = true;
          } else if ((selectingBuilding.tag == "基礎機械" && lastHitBuilding.tag == "雷射裝置") || (selectingBuilding.tag == "雷射裝置" && lastHitBuilding.tag == "基礎機械")) {
            newBuilding = laserCannon;
            newBuilding.GetComponent(Stats).planeTaken = selectingBuilding.GetComponent(Stats).planeTaken;
            newBuilding.GetComponent(Stats).kill = selectingBuilding.GetComponent(Stats).kill + lastHitBuilding.GetComponent(Stats).kill;
            newBuilding = Instantiate(newBuilding, selectingBuilding.transform.position, Quaternion.identity);
            // newBuilding.transform.localEulerAngles = selectingBuilding.transform.localEulerAngles;
            Destroy(selectingBuilding.gameObject);
            selectingBuilding = newBuilding;
            selectingBuilding.GetComponent(RangeDemo).enabled = true;
          } else if ((selectingBuilding.tag == "雷射裝置" && lastHitBuilding.tag == "燃燒裝置") || (selectingBuilding.tag == "燃燒裝置" && lastHitBuilding.tag == "雷射裝置")) {
            newBuilding = superBurningTower;
            newBuilding.GetComponent(Stats).planeTaken = selectingBuilding.GetComponent(Stats).planeTaken;
            newBuilding.GetComponent(Stats).kill = selectingBuilding.GetComponent(Stats).kill + lastHitBuilding.GetComponent(Stats).kill;
            newBuilding = Instantiate(newBuilding, selectingBuilding.transform.position, Quaternion.Euler(-90, 0, 0));
            Destroy(selectingBuilding.gameObject);
            selectingBuilding = newBuilding;
            selectingBuilding.GetComponent(RangeDemo).enabled = true;
          }
          nowBuildingNumber--;
          lastHitBuilding.GetComponent(Stats).planeTaken.GetComponent.<Renderer>().tag = "PlacementPlaneOpen";
          lastHitBuilding.Destroy(lastHitBuilding.gameObject);
          isCombining = false;
        }
      } else if (!isBuilding && !isResearching) { // Players can only select building when player's not building and researching
        if (selectingBuilding) {
          selectingBuilding.GetComponent(RangeDemo).enabled = false;
        }
        selectingBuilding = lastHitBuilding;
        selectingBuilding.GetComponent(RangeDemo).enabled = true;
      }
    } else { // lastHitBuilding is null
      // Clear selected building
      if (selectingBuilding) {
        selectingBuilding.GetComponent(RangeDemo).enabled = false;
      }
      lastHitBuilding = selectingBuilding = null;
      if (isCombining) {
        GetComponent.<AudioSource>().PlayOneShot(errorSound);
        ShowMessage("請選擇正確的目標");
        isCombining = false;
      }
    }
    if (lastHitPlane) {
      /*
      if (buildingIndex == -1) {
        isBuilding = false;
        ShowPlacementPlanes();
      }
      if (isResearching) {
        isResearching = false;
      }
      */
      if (lastHitPlane.tag == "PlacementPlaneOpen" && buildingIndex >= 0) {
        if (money >= allBuildings[buildingIndex].GetComponent(Stats).cost && nowBuildingNumber < maxBuildingNumber) {
          if (allBuildings[buildingIndex].tag == "燃燒裝置") { // Problem of this model
            newBuilding = Instantiate(allBuildings[buildingIndex], lastHitPlane.transform.position + Vector3(0, 1, 0), Quaternion.Euler(-90, 0, 0));
          } else {
            newBuilding = Instantiate(allBuildings[buildingIndex], lastHitPlane.transform.position + Vector3(0, 1, 0), Quaternion.identity);
          }
          GetComponent.<AudioSource>().PlayOneShot(buildSound);
          //newBuilding.transform.localEulerAngles.y = (Random.Range(0, 360));
          newBuilding.GetComponent(Stats).planeTaken = lastHitPlane.gameObject;
          lastHitPlane.tag = "PlacementPlaneTaken";
          nowBuildingNumber++;
          money -= allBuildings[buildingIndex].GetComponent(Stats).cost;
          lastHitPlane.gameObject.GetComponent.<Renderer>().enabled = false;
          ShowMessage("建造完成 : " + newBuilding.tag);
          //buildingIndex = -1;
        } else {
          if (nowBuildingNumber >= maxBuildingNumber) {
            GetComponent.<AudioSource>().PlayOneShot(errorSound);
            ShowMessage("機械數量超過上限");
          } else {
            GetComponent.<AudioSource>().PlayOneShot(errorSound);
            ShowMessage("需要更多金錢");
          }
        }
      }
    } else {
      //isBuilding = false;
      //buildingIndex = -1;
      //ShowPlacementPlanes();
      //isResearching = false;
      //researchIndex = -1;
      //isCombining = false;
    }
  }

  // Right Click
  if (Input.GetMouseButtonDown(1)) {
  }

  // Esc
  if (Input.GetKeyDown(KeyCode.Escape)) {
    if (isBuilding) {
      isBuilding = false;
      buildingIndex = -1;
      ShowPlacementPlanes();
    } else if (selectingBuilding) {
      isCombining = false;
      if (selectingBuilding) {
        selectingBuilding.GetComponent(RangeDemo).enabled = false;
      }
      lastHitBuilding = selectingBuilding = null;
    } else {
      isPause = true;
    }
  }

  /*************************** Cheat *******************************/
  /*************************** Cheat *******************************/
  /*************************** Cheat *******************************/
  /*
  if (Input.GetKeyUp(KeyCode.F6)) {
    GetComponent(WaveHandler).score = 99999999;
    money += 1000000;
  }
  if (Input.GetKeyUp(KeyCode.F7)) {
    Application.LoadLevel(Application.loadedLevel + 1);
  }
  if (Input.GetKeyUp(KeyCode.F8)) {
    Time.timeScale++;
  }
  if (Input.GetKeyUp(KeyCode.F9)) {
    Time.timeScale--;
  }
  if (Input.GetKeyUp(KeyCode.F10)) {
    GetComponent(WaveHandler).score = 99999999;
    mainBuilding.GetComponent(MainBuilding).GetComponent(Stats).nowHP = 10000;
  }
  */
  /*************************** Cheat *******************************/
  /*************************** Cheat *******************************/
  /*************************** Cheat *******************************/

  // Upgrade
  if (Input.GetKeyDown(KeyCode.U) && upgradable && selectingBuilding && !isFinished) {
    if (selectingBuilding.GetComponent(Stats).nextLevel) {
      newBuilding = selectingBuilding.GetComponent(Stats).nextLevel;
      var need : int = newBuilding.GetComponent(Stats).cost - selectingBuilding.GetComponent(Stats).cost;
      upgradeButton();
    }
  }
  if (Input.GetKeyDown(KeyCode.B)) {
    buildButton();
  }
  if (Input.GetKeyDown(KeyCode.C)) {
    if (combinatable && selectingBuilding.GetComponent(Stats).combinatable) { // Player have to research & Building has combination
      combineButton();
    }
  }
  if (Input.GetKeyDown(KeyCode.R)) {
    researchButton();
  }
  // Build and research hotkey
  if (isBuilding) {
    for (var i : int = 0; i < 4; ++i) {
      if (Input.GetKeyDown(KeyCode.Keypad1 + i) || Input.GetKeyUp(KeyCode.Alpha1 + i)) {
        GetComponent.<AudioSource>().PlayOneShot(buttonSound);
        if (buildingIndex != i) { // Prevent multiple click
          ShowMessage("請選擇放置區域");
          buildingIndex = i;
        }
      }
    }
  } else if (isResearching) {
    for (i = 0; i < allResearch.Count; ++i) {
      if (Input.GetKeyDown(KeyCode.Keypad1 + i) || Input.GetKeyUp(KeyCode.Alpha1 + i)) {
        GetComponent.<AudioSource>().PlayOneShot(buttonSound);
        researchIndex = i;
      }
    }
  }
}

function OnGUI () {
  GUI.skin = myGUISkin;
  GUI.depth = 1;
  if (isBuilding || isResearching || isCombining) {
    GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), maskTexture, ScaleMode.StretchToFill, true, 10.0f);
  }
  if (isFinished || isLosed) {
    GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), nurseCheer, ScaleMode.StretchToFill, true, 10.0f);
    return;
  }
  GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), statsHUD, ScaleMode.StretchToFill, true, 10.0f);
  GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), buildingHUD, ScaleMode.StretchToFill, true, 10.0f);
  if (isPause) {
    Time.timeScale = 0;
    // In Game Menu
    GUI.color = Color.white;
    GUILayout.BeginArea(Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200), towerFrameTexture);
    GUI.color = Color.black;
    if (isExit) {
      GUI.Label(Rect(50, 40, 100, 25), "確定離開?");
      if (GUI.Button(Rect(50, 70, 40, 25), "是")) {
        GetComponent.<AudioSource>().PlayOneShot(buttonSound);
        Application.LoadLevel("MainMenu");
      }
      if (GUI.Button(Rect(120, 70, 40, 25), "否")) {
        GetComponent.<AudioSource>().PlayOneShot(buttonSound);
        isExit = false;
      }
    } else if (isAdjusting) {
      AudioListener.volume = GUI.HorizontalScrollbar(Rect(50, 40, 100, 25), AudioListener.volume, 0.01, 0.0, 1.0);
      if (GUI.Button(Rect(50, 70, 100, 25), "返回")) {
        GetComponent.<AudioSource>().PlayOneShot(buttonSound);
        isAdjusting = false;
      }
    } else {
      if (GUI.Button(Rect(50, 40, 100, 25), "音量調整")) {
        GetComponent.<AudioSource>().PlayOneShot(buttonSound);
        isAdjusting = true;
      }
      if (GUI.Button(Rect(50, 70, 100, 25), "返回")) {
        GetComponent.<AudioSource>().PlayOneShot(buttonSound);
        isPause = false;
        Time.timeScale = 1;
      }
      if (GUI.Button(Rect(50, 100, 100, 25), "離開")) {
        GetComponent.<AudioSource>().PlayOneShot(buttonSound);
        isExit = true;
      }
    }
    GUILayout.EndArea();
    return;
  }
  
  
  // Stats
  GUILayout.BeginArea(Rect(40, 30, 200, 200));

  // Life Bar
  GUI.color = Color.red;
  GUI.DrawTexture(Rect(0, 0, 100, 25), myGUITexture, ScaleMode.StretchToFill, true, 10.0f);
  GUI.color = Color.green;
  GUI.DrawTexture(Rect(0, 0, (mainBuilding.GetComponent(MainBuilding).GetComponent(Stats).nowHP + 0.0) / mainBuilding.GetComponent(MainBuilding).GetComponent(Stats).maxHP * 100.0, 25), myGUITexture, ScaleMode.StretchToFill, true, 10.0f);
  GUI.color = Color.black;

  GUI.Label(Rect(25, 0, 100, 25), mainBuilding.GetComponent(MainBuilding).GetComponent(Stats).nowHP + " / " + mainBuilding.GetComponent(MainBuilding).GetComponent(Stats).maxHP);

  GUI.color = Color.green;
  GUI.Label(Rect(0, 20, 150, 25), "金錢 : " + money);
  GUI.Label(Rect(0, 40, 150, 25), "機械數量 : " + nowBuildingNumber + " / " + maxBuildingNumber);
  if (GetComponent(WaveHandler).nowWave == GetComponent(WaveHandler).maxWave) {
    GUI.color = Color.red;
  }
  if (GetComponent(WaveHandler).maxWave == 99999) { // Survival
    // Survival show score
    GUI.Label(Rect(0, 60, 150, 25), "分數 : " + GetComponent(WaveHandler).score);
  } else {
    GUI.Label(Rect(0, 60, 150, 25), "波數 : " + GetComponent(WaveHandler).nowWave + " / " + GetComponent(WaveHandler).maxWave);
    GUI.color = Color.green;
    if (GetComponent(WaveHandler).nowWave == 1 && GetComponent(WaveHandler).restTime > 0) {
      GUI.Label(Rect(0, 80, 150, 25), "準備時間 : " + Mathf.RoundToInt(GetComponent(WaveHandler).restTime));
    } else if (GetComponent(MaxLoading)) {
      GUI.Label(Rect(0, 80, 150, 25), "負載 : " + GetComponent(WaveHandler).monsterOnMap + " / " + GetComponent(MaxLoading).maxLoad);
    }
  }
  /* else {
    GUI.Label(Rect(50, 110, 150, 20), "Monster : " + monsterOnMap);
  }
  */
  GUILayout.EndArea();

  /*************************** Test *******************************/
  /*************************** Test *******************************/
  /*************************** Test *******************************/
  /*
  GUI.color = Color.black;
  GUILayout.BeginArea(Rect(Screen.width - 150, 200, 150, 300));
  GUI.Label(Rect(0, 0, 150, 30), "NowSpeed : " + Time.timeScale);

  if (GUI.Button(Rect(0, 50, 50, 30), "+++++")) {
    Time.timeScale++;
  }

  if (GUI.Button(Rect(0, 100, 50, 30), "-----")) {
    Time.timeScale--;
  }

  if (GUI.Button(Rect(0, 150, 50, 30), "10000$")) {
    money += 1000000;
  }

  if (GUI.Button(Rect(0, 200, 50, 30), "Life")) {
    mainBuilding.GetComponent(MainBuilding).GetComponent(Stats).nowHP = 10000;
  }

  if (GUI.Button(Rect(0, 250, 50, 30), "Next")) {
    Application.LoadLevel(Application.loadedLevel + 1);
  }
  
  GUILayout.EndArea();
  */
  /*************************** Test *******************************/
  /*************************** Test *******************************/
  /*************************** Test *******************************/
  // Button

  GUI.color = Color.black;
  if (GUI.Button(Rect(Screen.width - 250, Screen.height - 75, 100, 30), "選單(ESC)")) {
    GetComponent.<AudioSource>().PlayOneShot(buttonSound);
    isPause = true;
  }

  GUILayout.BeginArea(Rect(25, Screen.height - 75, 100,  200));
  
  if (GUI.Button(Rect(0, 0, 100, 30), "研究(R)")) {
    researchButton();
  }
  if (GUI.Button(Rect(0, 35, 100, 30), "建造(B)")) {
    buildButton();
  }

  GUILayout.EndArea();

  // Researching buttons
  if (isResearching) {
    if (researchIndex != -1) {
      var researchName : String = allResearch[researchIndex].name;
      var cost : int = allResearch[researchIndex].cost;
      if (GUI.Button(Rect(600, 80, 100, 25), "研究")) {
        if (researchName == "升級技術") {
          if (money >= cost) {
            GetComponent.<AudioSource>().PlayOneShot(techSound);
            money -= cost;
            upgradable = true;
            allResearch.RemoveAt(researchIndex);
            allResearch.Add(new Research("組合技術", 2000));
            ShowMessage("研發完成 : 升級技術");
            ShowMessage("獲得新科技 : 組合技術");
            researchIndex = -1;
          } else {
            GetComponent.<AudioSource>().PlayOneShot(errorSound);
            ShowMessage("需要更多金錢");
          }
        } else if (researchName == "組合技術") {
          if (money >= cost) {
            GetComponent.<AudioSource>().PlayOneShot(techSound);
            money -= cost;
            combinatable = true;
            allResearch.RemoveAt(researchIndex);
            ShowMessage("研發完成 : 組合技術");
            allResearch.Add(new Research("額外機械數量", 5000));
            ShowMessage("獲得科技 : 額外機械數量");
            researchIndex = -1;
          } else {
            GetComponent.<AudioSource>().PlayOneShot(errorSound);
            ShowMessage("需要更多金錢");
          }
        } else if (researchName == "自我學習") {
          if (money >= cost) {
            GetComponent.<AudioSource>().PlayOneShot(techSound);
            money -= cost;
            learnable = true;
            allResearch.RemoveAt(researchIndex);
            var alreadyHeal : boolean = false;
            for (tempR in allResearch) {
              if (tempR.name == "自癒") {
                alreadyHeal = true;
              }
            }
            if (!alreadyHeal) {
              allResearch.Add(new Research("自癒", 2500));
            }
            ShowMessage("研發完成 : 自我學習");
            ShowMessage("獲得科技 : 自癒");
            researchIndex = -1;
          } else {
            GetComponent.<AudioSource>().PlayOneShot(errorSound);
            ShowMessage("需要更多金錢");
          }
        } else if (researchName == "自癒") {
          if (money >= cost) {
            GetComponent.<AudioSource>().PlayOneShot(techSound);
            mainBuilding.AddComponent(SelfHealing);
            mainBuilding.GetComponent(SelfHealing).delay = 15;
            mainBuilding.GetComponent(SelfHealing).nextHealTime = Time.time + 15;
            money -= cost;
            allResearch.RemoveAt(researchIndex);
            ShowMessage("研發完成 : 自癒");
            researchIndex = -1;
          } else {
            GetComponent.<AudioSource>().PlayOneShot(errorSound);
            ShowMessage("需要更多金錢");
          }
        } else if (researchName == "額外機械數量") {
          if (money >= cost) {
            GetComponent.<AudioSource>().PlayOneShot(techSound);
            money -= cost;
            maxBuildingNumber += 5;
            ShowMessage("研發完成 : 額外機械數量");
            researchIndex = -1;
          } else {
            GetComponent.<AudioSource>().PlayOneShot(errorSound);
            ShowMessage("需要更多金錢");
          }
        }
      }
    }

    // Highlight
    GUI.color = Color.green;
    // GUI.DrawTexture(Rect(25, Screen.height - 75, 100, 30), highlightTexture, ScaleMode.StretchToFill, true, 100.0f);

    GUI.color = Color.black;
    
    GUILayout.BeginArea(Rect(100, Screen.height - 75, 1000, 75));

    for (var i : int = 0; i < allResearch.Count; ++i) {
      if (GUI.Button(Rect(i * 150 + 75, 0, 135, 30), allResearch[i].name + "(" + (i + 1) + ")")) {
        researchIndex = i;
      }
    }

    GUILayout.EndArea();
    return;
  }


  // Building buttons
  if (isBuilding) {

    // Highlight
    GUI.color = Color.green;
    // GUI.DrawTexture(Rect(25, Screen.height - 40, 100, 30), highlightTexture, ScaleMode.StretchToFill, true, 100.0f);
    
    GUILayout.BeginArea(Rect(100, Screen.height - 40, 1000, 75));

    /* Highlight
    if (buildingIndex != -1) {
      if (money < allBuildings[buildingIndex].GetComponent(Stats).cost) {
        GUI.color = Color.red;
      }
      GUI.DrawTexture(Rect(buildingIndex * 150 + 75, 0, 135, 30), highlightTexture, ScaleMode.StretchToFill, true, 100.0f);
    }
    */
    GUI.color = Color.black;

    for (i = 0; i < allBuildings.length; ++i) {
      if (GUI.Button(Rect(i * 150 + 75, 0, 135, 30), allBuildings[i].tag + " (" + (i + 1) + ")")) { // Hotkey
        GetComponent.<AudioSource>().PlayOneShot(buttonSound);
        if (i != buildingIndex) { // Prevent multiple click
          ShowMessage("請選擇放置區域");
          buildingIndex = i;
        }
      }
    }
    GUILayout.EndArea();
    return;
  } 
  if (selectingBuilding) {

    // Highlight of the selecting building
    var locationOnScreenLU : Vector3;
    var locationOnScreenRD : Vector3;
    var differenceOnScreen : Vector3;
    locationOnScreenLU = Camera.main.WorldToScreenPoint(selectingBuilding.GetComponent(Stats).planeTaken.transform.position);
    locationOnScreenRD = Camera.main.WorldToScreenPoint(selectingBuilding.GetComponent(Stats).planeTaken.transform.position + Vector3(20, 0, 20));
    differenceOnScreen = locationOnScreenRD - locationOnScreenLU;
    GUI.color = Color.green;
     GUI.DrawTexture(Rect(locationOnScreenLU.x - differenceOnScreen.x * 0.5, Screen.height - locationOnScreenLU.y + differenceOnScreen.y * 0.5, differenceOnScreen.x, -differenceOnScreen.y), highlightTexture, ScaleMode.StretchToFill, true, 10.0f);
    
    //GUILayout.BeginArea(Rect(locationOnScreenLU.x - 100, Screen.height - locationOnScreenLU.y - 100, 200, 200));
    
    GUI.color = Color.white;
    GUILayout.BeginArea(Rect(25, 250, 300, 300), towerFrameTexture);

    GUI.color = Color.black;

    GUI.Label(Rect(50, 50, 200, 50), selectingBuilding.tag);
    
    GUI.color = Color.red;
    GUI.DrawTexture(Rect(50, 75, 150, 25), myGUITexture, ScaleMode.StretchToFill, true, 10.0f);
    GUI.color = Color.green;
    GUI.DrawTexture(Rect(50, 75, (selectingBuilding.GetComponent(Stats).nowHP + 0.0) / selectingBuilding.GetComponent(Stats).maxHP * 150.0, 25), myGUITexture, ScaleMode.StretchToFill, true, 10.0f);
    GUI.color = Color.black;
    GUI.Label(Rect(55, 75, 75, 25), selectingBuilding.GetComponent(Stats).nowHP + " / " + selectingBuilding.GetComponent(Stats).maxHP);
    
    if (selectingBuilding.tag != "緩速光環") { // Slow aura no damage
      if (learnable) {
        GUI.Label(Rect(50, 100, 200, 25), "傷害 : " + selectingBuilding.GetComponent(Stats).damage + "(+" + selectingBuilding.GetComponent(Stats).kill / 10 + "%)");
      } else {
        GUI.Label(Rect(50, 100, 200, 25), "傷害 : " + selectingBuilding.GetComponent(Stats).damage);
      }
    } else { // Slow effect
      GUI.Label(Rect(50, 100, 200, 25), "將目標速度降至" + selectingBuilding.GetComponent(Stats).damage * 100 + "%");
    }
    if (selectingBuilding.GetComponent(Stats).nextLevel) {
      var newBuilding : GameObject = selectingBuilding.GetComponent(Stats).nextLevel;
      var need : int = newBuilding.GetComponent(Stats).cost - selectingBuilding.GetComponent(Stats).cost;
      if (upgradable) {
        // Highlight
        /* if (money >= need) {
          GUI.color = Color.green;
          GUI.DrawTexture(Rect(50, 125, 150, 25), highlightTexture, ScaleMode.StretchToFill, true, 100.0f);
          GUI.color = Color.black;
        } else {
          GUI.color = Color.red;
          GUI.DrawTexture(Rect(50, 125, 150, 25), highlightTexture, ScaleMode.StretchToFill, true, 100.0f);
          GUI.color = Color.black;
        } */
        if (GUI.Button(Rect(50, 125, 150, 25), "升級(U) : " + need)) {
          upgradeButton();
        }
      }
    } else {
      if (combinatable && selectingBuilding.GetComponent(Stats).combinatable) { // Player have to research & Building need to has combination
        if (GUI.Button(Rect(50, 125, 150, 25), "組合(C)")) {
          combineButton();
        }
      }
    }
    if (GUI.Button(Rect(50, 155, 150, 25), "拆除(80%金錢)")) {
      GetComponent.<AudioSource>().PlayOneShot(sellSound);
      ShowMessage("拆除完成 : " + selectingBuilding.tag);
      ShowMessage("取回 : " + selectingBuilding.GetComponent(Stats).cost * 0.8);
      money += selectingBuilding.GetComponent(Stats).cost * 0.8;
      nowBuildingNumber--;
      selectingBuilding.GetComponent(Stats).planeTaken.GetComponent.<Renderer>().tag = "PlacementPlaneOpen";
      selectingBuilding.Destroy(selectingBuilding.gameObject);
    }
    /* if (GUI.Button(Rect(0, 100, 100, 30), "Repair")) {
      if (money > 0 && selectingBuilding.GetComponent(Stats).nowHP < selectingBuilding.GetComponent(Stats).maxHP) {
        money--;
        selectingBuilding.GetComponent(Stats).nowHP++;
      }
    } */
    GUILayout.EndArea();
  }
}

function buildButton () {
  if (selectingBuilding) {
    selectingBuilding.GetComponent(RangeDemo).enabled = false;
  }
  lastHitBuilding = selectingBuilding = null;
  GetComponent.<AudioSource>().PlayOneShot(buttonSound);
  buildingIndex = -1;
  isBuilding = !isBuilding;
  ShowPlacementPlanes();
  if (isResearching) {
    isResearching = false;
    researchIndex = -1;
  }
}

function researchButton () {
  if (selectingBuilding) {
    selectingBuilding.GetComponent(RangeDemo).enabled = false;
  }
  lastHitBuilding = selectingBuilding = null;
  GetComponent.<AudioSource>().PlayOneShot(buttonSound);
  isResearching = !isResearching;
  researchIndex = -1;
  if (isBuilding) {
    isBuilding = false;
    buildingIndex = -1;
    ShowPlacementPlanes();
  }
}

function combineButton () {
  GetComponent.<AudioSource>().PlayOneShot(buttonSound);
  isCombining = true;
  ShowMessage("請選擇組合目標");
}

function upgradeButton () {
  var newBuilding : GameObject = selectingBuilding.GetComponent(Stats).nextLevel;
  var need : int = newBuilding.GetComponent(Stats).cost - selectingBuilding.GetComponent(Stats).cost;
  if (money >= need) {
    ShowMessage("升級完成 : " + selectingBuilding.tag);
    GetComponent.<AudioSource>().PlayOneShot(techSound);
    money -= need;
    newBuilding.GetComponent(Stats).planeTaken = selectingBuilding.GetComponent(Stats).planeTaken;
    newBuilding.GetComponent(Stats).kill = selectingBuilding.GetComponent(Stats).kill;
    newBuilding = Instantiate(newBuilding, selectingBuilding.transform.position, Quaternion.identity);
    newBuilding.transform.localEulerAngles = selectingBuilding.transform.localEulerAngles;
    Destroy(selectingBuilding.gameObject);
    selectingBuilding = newBuilding;
    selectingBuilding.GetComponent(RangeDemo).enabled = true;
  } else {
    GetComponent.<AudioSource>().PlayOneShot(errorSound);
    ShowMessage("需要更多金錢");
  }
}

function ShowPlacementPlanes () {
  for (var plane : Transform in placementPlanesRoot) {
    if (isBuilding) {
      if (plane.tag == "PlacementPlaneOpen") { // Only show the empty plane
        plane.gameObject.GetComponent.<Renderer>().enabled = isBuilding;
      }
    } else {
      plane.gameObject.GetComponent.<Renderer>().enabled = isBuilding;
    }
  }
}

function ShowMessage (message : String) {
  GetComponent(MessageShow).AddMessage(message);
}

