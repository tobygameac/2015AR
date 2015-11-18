#pragma strict

var buildingTexture : Texture[];
var buildingIndex : int;
var background : Texture;

function Start () {

}

function Update () {
  buildingIndex = GetComponent(MainFunction).buildingIndex;
}

function OnGUI () {
  GUI.depth = 0;
  if (buildingIndex != -1) {
    GUI.color = Color.blue;
    GUI.DrawTexture(Rect(580, 5, 320, 110), background);
    GUI.color = Color.white;
    GUI.DrawTexture(Rect(600, 15, 80, 80), buildingTexture[buildingIndex]);
    var message = "價格 : " + GetComponent(MainFunction).allBuildings[buildingIndex].GetComponent(Stats).cost + "\n";
    if (GetComponent(MainFunction).allBuildings[buildingIndex].tag == "緩速光環") { // Slow Aura
      message += "將目標速度降至 : " + GetComponent(MainFunction).allBuildings[buildingIndex].GetComponent(Stats).damage * 100 + " %\n";
    } else {
      message += "傷害 : " + GetComponent(MainFunction).allBuildings[buildingIndex].GetComponent(Stats).damage + "\n";
    }
    message += "生命 : " + GetComponent(MainFunction).allBuildings[buildingIndex].GetComponent(Stats).maxHP + "\n";
    message += "攻擊模式 : " + GetComponent(MainFunction).allBuildings[buildingIndex].GetComponent(Stats).features + "\n";
    message += "攻擊距離 : " + GetComponent(MainFunction).allBuildings[buildingIndex].GetComponent(Stats).range + "\n";
    GUI.color = Color.red;
    GUI.Label(Rect(700, 15, 200, 80), message);
  }
}
