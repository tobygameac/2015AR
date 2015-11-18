#pragma strict

var myGUISkin : GUISkin;
var background : Texture;

var buttonSound : AudioClip;

function Update () {

}

function OnGUI () {
  GUI.skin = myGUISkin;
  GUI.DrawTexture(Rect(0, 0, Screen.width, Screen.height), background, ScaleMode.StretchToFill, true, 10.0f);
  GUI.color = Color.black;
  GUI.Label(Rect(0, 0, 100, 100), "Last Update : " + "5/31 9.57");
  GUILayout.BeginArea(Rect(Screen.width / 2 - 50, 200, 500, 500));
  if (GUI.Button(Rect(0, 0, 100, 50), "回到主選單")) {
      GetComponent.<AudioSource>().PlayOneShot(buttonSound);
      Time.timeScale = 1;
      Application.LoadLevel("MainMenu");
  }
  GUILayout.EndArea();
}
