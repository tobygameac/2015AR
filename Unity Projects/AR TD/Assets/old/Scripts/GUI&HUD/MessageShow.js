#pragma strict

var timeRemain : float;
var message : String;
var background : Texture;

function Start () {
}

function Update () {
  // timeRemain -= Time.deltaTime;
}

function AddMessage (s : String) {
  timeRemain = 3;
  message = s + (message.length ? "\n" + message : "");
}

function OnGUI () {
   //if (timeRemain > 0) {
    GUI.depth = 0;
    // message handle
    message = GUI.TextArea(Rect(-99, -99, 0, 0), message, 300);
    GUI.DrawTexture(Rect(230, 5, 320, 110), background);
    GUI.color = Color.red;
    GUI.Label(Rect(250, 15, 300, 95), message);
   //}
}
