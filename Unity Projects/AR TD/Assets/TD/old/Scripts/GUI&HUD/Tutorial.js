#pragma strict

var myGUISkin : GUISkin;
var background : Texture;
var pictures : Texture[];
var index : int;

var buttonSound : AudioClip;

function Start () {
  index = 0;
}

function Update () {

}

function OnGUI () {
  GUI.skin = myGUISkin;
  GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), background);
  GUI.DrawTexture(Rect(100, 62.5, 760, 475), pictures[index]);
  GUI.color = Color.black;
  if (index != 0 && GUI.Button(Rect(Screen.width / 2 - 150, Screen.height - 50, 100, 25), "上一頁")) {
    GetComponent.<AudioSource>().PlayOneShot(buttonSound);
    --index;
  }
  if (index < pictures.Length - 1 && GUI.Button(Rect(Screen.width / 2 + 50, Screen.height - 50, 100, 25), "下一頁")) {
    GetComponent.<AudioSource>().PlayOneShot(buttonSound);
    index++;
  }
  if (GUI.Button(Rect(Screen.width - 250, Screen.height - 50, 100, 25), "回到主選單")) {
    GetComponent.<AudioSource>().PlayOneShot(buttonSound);
    Application.LoadLevel("MainMenu");
  }
}
