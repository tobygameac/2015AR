#pragma strict

var buildingRangeTexture : Texture;

function Start () {
}

function Update () {
}

function OnGUI () {
  var x1 : int = GetComponent.<Collider>().bounds.min.x - 10;
  var x2 : int = GetComponent.<Collider>().bounds.max.x + 10;
  var z1 : int = GetComponent.<Collider>().bounds.max.z + 10;
  var z2 : int = GetComponent.<Collider>().bounds.min.z - 10;
  var locationOnScreenLU : Vector3 = Camera.main.WorldToScreenPoint(Vector3(x1, 0, z1));
  var locationOnScreenRD : Vector3 = Camera.main.WorldToScreenPoint(Vector3(x2, 0, z2));
  var differenceOnScreen : Vector3 = locationOnScreenRD - locationOnScreenLU;
  GUI.color = Color(1, 1, 1, 0.25f);
  GUI.DrawTexture(Rect(locationOnScreenLU.x - differenceOnScreen.x * 0, Screen.height - locationOnScreenLU.y + differenceOnScreen.y * 0, differenceOnScreen.x, -differenceOnScreen.y), buildingRangeTexture, ScaleMode.StretchToFill, true, 10.0f);
}
