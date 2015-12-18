#pragma strict
#pragma downcast

// GUI
var myGUISkin : GUISkin;
var myGUITexture : Texture;

var nextScene : String;

var score : int;

// Wave
var nowWave : int;
var maxWave : int;
var restTime : float;
var waveClear : boolean;
var isRest : boolean;
var isFinished : boolean;
var isLosed : boolean;
var isSubmit : boolean;


// Monster
var initialObjects : Transform;
var initialPositions = new Array();
var targetPosition : Vector3[];
var allMonsters : GameObject[];
var monsterNumber : int;
var monsterOnMap : int;
var nextMonsterTime : float;

// Time
var nextWaveTime : float;
var delayTime : float;

// Condition
var condition : Component[];

// Player's Name
var yourName = "Your Name";

function Start () {
  score = 0;
  switch (Application.loadedLevelName) {
    case "Level1":
      ShowMessage("你現在位在皮膚傷口位置，病菌正要開始入侵");
      ShowMessage("請建立你的奈米機械，防止病菌入侵!");
      break;
    case "Level2":
      ShowMessage("病菌來到了腸道");
      ShowMessage("請建立你的奈米機械，防止病菌通過腸道!");
      break;
    case "Level3":
      ShowMessage("現在位在人體血管處，有大量病菌即將入侵");
      ShowMessage("請建立你的奈米機械，抑制病菌數量!");
      break;
    case "Level4":
      ShowMessage("病菌已入侵到腦部");
      ShowMessage("建立你的奈米機械，擊敗超級病菌");
      break;
    case "SurvivalNormal":
      ShowMessage("建立你的奈米機械，擊敗病菌");
      break;
    case "SurvivalBoss":
      ShowMessage("建立你的奈米機械，對抗超級病菌");
      break;
  }
  if (Application.loadedLevelName == "Level1" || Application.loadedLevelName == "SurvivalNormal" || Application.loadedLevelName == "SurvivalBoss" || Application.loadedLevelName == "Demo") { // First Level and Survival
    switch (Application.loadedLevelName) {
      case "Level1":
        GetComponent(MainFunction).money = 500;
        break;
      case "SurvivalNormal":
        GetComponent(MainFunction).money = 1000;
        break;
      case "SurvivalBoss":
        GetComponent(MainFunction).money = 2500;
        break;
      case "Demo":
        GetComponent(MainFunction).money = 1000000;
        break;
    }
    GetComponent(MainFunction).upgradable = false;
    GetComponent(MainFunction).combinatable = false;
    GetComponent(MainFunction).learnable = false;
    GetComponent(MainFunction).allResearch.Clear();
    GetComponent(MainFunction).allResearch.Add(new Research("升級技術", 300));
    GetComponent(MainFunction).allResearch.Add(new Research("自我學習", 500));
  } else {
    var needToAdd : boolean = false;
    for (var i : int = 0; i < GetComponent(MainFunction).allResearch.Count; ++i) {
      if (GetComponent(MainFunction).allResearch[i].name == "自癒") {
        GetComponent(MainFunction).allResearch.RemoveAt(i);
        needToAdd = true;
      }
    }
    if (needToAdd) {
      GetComponent(MainFunction).allResearch.Add(new Research("自癒", 2500));
    }
  }
  nextMonsterTime = Time.time;
  nextWaveTime = Time.time + 60;
  for (var obj : Transform in initialObjects) {
    initialPositions.Push(obj.position);
  }
}

function Update () {
  if (isFinished || isLosed) {
    return;
  }
  if (isRest) {
    restTime -= Time.deltaTime;
    if (!waveClear) {
      NextWave();
      waveClear = true;
    } else if (restTime <= 0) {
      isRest = false;
      waveClear = false;
    }
  } else {
    if (Time.time >= nextMonsterTime && monsterNumber > 0) {
      NewMonster();
    } else {
      if (nowWave != maxWave) {
        if (monsterNumber <= nowWave && monsterOnMap <= nowWave) {
          nextWaveTime = Time.time + 45;
          isRest = true;
        } else if (Time.time >= nextWaveTime) {
          if (Application.loadedLevelName == "SurvivalNormal" || Application.loadedLevelName == "SurvivalBoss") {
            // Survival need to kill all monsters
          } else {
            nextWaveTime = Time.time + 45;
            isRest = true;
          }
        }
      } else {
        // Need to kill all monster when max wave
        if (monsterNumber <= 0 && monsterOnMap <= 0) {
          isRest = true;
        }
      }
    }
  }
}

function NextWave () {
  if (nowWave == maxWave) {
    isFinished = true;
    GetComponent(MainFunction).isFinished = true;
    GetComponent(MessageShow).enabled = false;
    GetComponent(BuildingDetails).enabled = false;
    ShowMessage("恭喜過關");
    if (nextScene == "Win") { // Last level
      Application.LoadLevel(nextScene);
    }
    SellAll();
  } else {
    isRest = true;
    nowWave++;
    if (Application.loadedLevelName == "SurvivalNormal" || Application.loadedLevelName == "SurvivalBoss") {
      // Survival don't need to know
    } else {
      ShowMessage("第 " + nowWave + " 波病菌即將入侵");
    }
    GetComponent(MainFunction).money += (nowWave - 1) * 25;
    monsterNumber += Mathf.Pow(nowWave, 2) + nowWave * 10;
    if (Application.loadedLevelName == "Demo") {
      restTime = 3;
    } else {
      restTime = 5 + (nowWave == 1 ? 20 : 0);
    }
  }
}

function NewMonster () {
  for (var newMonsterNumber = nowWave + 3 > monsterNumber ? monsterNumber : nowWave + 3; newMonsterNumber > 0; newMonsterNumber--) {
    monsterNumber--;
    monsterOnMap++;
    var monsterType : int;
    if (allMonsters.length > 1) {
      monsterType = Random.Range(nowWave / 10 > allMonsters.length - 1 ? allMonsters.length - 1 : nowWave / 10, (nowWave / 5 + 1) > allMonsters.length ? allMonsters.length : (nowWave / 5 + 1));
    } else {
      monsterType = 0;
    }
    var bornPosition : Vector3 = initialPositions[Random.Range(0, initialPositions.length)];
    var nextMonster : GameObject = Instantiate(allMonsters[monsterType], bornPosition, Quaternion.identity);
    nextMonster.GetComponent(Monster).targetPosition = targetPosition;
    // Add condition
    if (nowWave >= 3) {
      var addCondition : int = Random.Range(0, 100);
      switch (addCondition) {
        case 0:
          nextMonster.AddComponent(Strongger);
          nextMonster.GetComponent(Stats).cost *= 2;
          //ShowMessage("Mutation! An enemy has become strongger.");
          break;
        case 1:
        case 2:
          nextMonster.AddComponent(Insane);
          nextMonster.GetComponent(Stats).cost *= 2;
          //ShowMessage("Mutation! An enemy is insane.");
          break;
        case 4:
        case 5:
        case 6:
          nextMonster.AddComponent(BigResizer);
          nextMonster.AddComponent(CellDivision);
          nextMonster.GetComponent(Stats).cost *= 2;
          if (nowWave < 15) {
            nextMonster.GetComponent(BigResizer).scale = 1.2;
            nextMonster.GetComponent(CellDivision).maxDivision = 2;
          } else if (nowWave < 25) {
            nextMonster.GetComponent(BigResizer).scale = 1.4;
            nextMonster.GetComponent(CellDivision).maxDivision = 4;
          } else {
            var divNum : int = Random.Range(4, 10);
            nextMonster.GetComponent(BigResizer).scale = 1 + (divNum * 0.1f);
            nextMonster.GetComponent(CellDivision).maxDivision = divNum;
          }
          break;
        default:
          break;
      }
    }
  }
  nextMonsterTime = Time.time + delayTime;
}

function Kill (n : int) {
  if (n > 0) {
    score += nowWave + n;
  }
  GetComponent(MainFunction).money += n;
  monsterOnMap--;
}

function SellAll () {
  var all = FindObjectsOfType(GameObject);
  var buildingList = new System.Collections.Generic.List.<GameObject>();
  var moneyBack : int = 0;
  for (object in all) {
    if (object.layer == LayerMask.NameToLayer("Building")) {
      while (object.transform.parent) {
        object = object.transform.parent.gameObject;
      }

      var checkArray = buildingList.ToArray();
      var check : boolean = true;
      for (building in checkArray) {
        if (building == object) check = false;
      }
      if (check) {
        buildingList.Add(object);
      }
      
      object.Destroy(object.gameObject);
    }
  }
  var buildingArray = buildingList.ToArray();
  for (building in buildingArray) {
    GetComponent(MainFunction).money += building.GetComponent(Stats).cost;
    moneyBack += building.GetComponent(Stats).cost;
  }
  ShowMessage("從場上機械取回 " + moneyBack + " 金錢");
}

function ShowMessage (message : String) {
  GetComponent(MessageShow).AddMessage(message);
}

function ScoreSubmit (name : String, phpTarget : String, link : String) {
  isSubmit = true;
  var form = new WWWForm();
  if (score > 9999999 || (Time.time < 1800 && score >= 1000000)) score = -1; // cheat
  form.AddField("score", score.ToString());
  form.AddField("name", name);
  var w = WWW(phpTarget, form);
  yield w;
  if (!String.IsNullOrEmpty(w.error)) {
    print(w.error);
  } else {
    print("Finished");
  }
  if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.OSXWebPlayer) { // WebPlayer
    link = "window.open('" + link + "', '_blank')";
    Application.ExternalEval(link);
  } else {
    Application.OpenURL(link);
  }
  Application.LoadLevel("MainMenu");
}

function OnGUI () {
  GUI.skin = myGUISkin;
  GUI.depth = 0;
  GUI.color = Color.black;
  if (isFinished) {
    if (GUI.Button(Rect(Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50), "下一關")) {
      Application.LoadLevel(nextScene);
    }
    return;
  }
  if (isLosed) {
    if (Application.loadedLevelName == "SurvivalNormal" || Application.loadedLevelName == "SurvivalBoss") { // Survival
      yourName = GUI.TextArea(Rect(Screen.width / 2 - 50, Screen.height / 2 - 100, 100, 50), yourName);
      if (!isSubmit && GUI.Button(Rect(Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50), "送出總分")) {
        if (yourName.length >= 20) {
          ShowMessage("名稱過長");
        } else {
          //var postUrl : String = "http://NanoTDPost";
          var postUrl : String = "http://134.208.43.1:5631/NanoTD/postScore";
          var boardUrl : String = "http://NanoTDScoreboard";
          if (Application.loadedLevelName == "SurvivalNormal") {
            //postUrl += "1.tobygameac.com";
            postUrl += "1.php";
            boardUrl += "1.tobygameac.com";
          } else if (Application.loadedLevelName == "SurvivalBoss") {
            //postUrl += "2.tobygameac.com";
            postUrl += "2.php";
            boardUrl += "2.tobygameac.com";
          }
          ScoreSubmit(yourName, postUrl, boardUrl);
        }
      }
      if (GUI.Button(Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 100, 50), "回到主選單")) {
        Application.LoadLevel("MainMenu");
      }
    } else {
      if (GUI.Button(Rect(Screen.width / 2 - 50, Screen.height / 2 - 25, 100, 50), "任務失敗")) {
        Application.LoadLevel("MainMenu");
      }
    }
    return;
  }
}
