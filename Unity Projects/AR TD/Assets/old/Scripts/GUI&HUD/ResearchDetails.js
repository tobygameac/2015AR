#pragma strict

var researchIndex : int;
var background : Texture;

function Start () {

}

function Update () {
  researchIndex = GetComponent(MainFunction).researchIndex;
}

function OnGUI () {
  GUI.depth = 0;
  if (researchIndex != -1) {
    GUI.color = Color.yellow;
    GUI.DrawTexture(Rect(580, 5, 320, 110), background);
    var message = "科技 : " + GetComponent(MainFunction).allResearch[researchIndex].name + "\n";
    message += "價格 : " + GetComponent(MainFunction).allResearch[researchIndex].cost + "\n";
    switch (GetComponent(MainFunction).allResearch[researchIndex].name) {
      case "升級技術":
        message += "可以將奈米機械進行升級動作\n";
        message += "在劇情模式中可以帶到下一關卡\n";
        break;
      case "組合技術":
        message += "可以將不同奈米機械進行組合\n";
        message += "在劇情模式中可以帶到下一關卡\n";
        break;
      case "自我學習":
        message += "奈米機械隨殺敵數有更大的威力\n";
        message += "在劇情模式中可以帶到下一關卡\n";
        break;
      case "自癒":
        message += "傷口得到自我復原功能\n";
        message += "無法帶到下一關卡\n";
        break;
      case "額外機械數量":
        message += "提升場上最大建造數量限制\n";
        message += "無法帶到下一關卡\n";
        break;
    }
    GUI.color = Color.red;
    GUI.Label(Rect(600, 15, 200, 80), message);
  }
}
