#pragma strict

var myGUITexture : Texture;

function Start () {
}

function Update () {

}

function OnGUI() {
    var locationOnScreenLU : Vector3 = Camera.main.WorldToScreenPoint(transform.position);
    var locationOnScreenRD : Vector3 = Camera.main.WorldToScreenPoint(transform.position + Vector3(20, 0, 20));
    var differenceOnScreen : Vector3 = locationOnScreenRD - locationOnScreenLU;
    GUI.color = Color.red;
    GUI.DrawTexture(Rect(locationOnScreenLU.x - differenceOnScreen.x * 0.5, Screen.height - locationOnScreenLU.y - differenceOnScreen.y * 0.5, differenceOnScreen.x, -differenceOnScreen.y / 4), myGUITexture, ScaleMode.StretchToFill, true, 10.0f);
    GUI.color = Color.green;
    GUI.DrawTexture(Rect(locationOnScreenLU.x - differenceOnScreen.x * 0.5, Screen.height - locationOnScreenLU.y - differenceOnScreen.y * 0.5, (transform.GetComponent(Stats).nowHP + 0.0) / transform.GetComponent(Stats).maxHP * differenceOnScreen.x, -differenceOnScreen.y / 4), myGUITexture, ScaleMode.StretchToFill, true, 10.0f);
    /*var locationOnScreen : Vector3 = Camera.main.WorldToScreenPoint(transform.position);
    GUI.color = Color.red;
    GUI.DrawTexture(Rect(locationOnScreen.x - 25, Screen.height - locationOnScreen.y - 40, 50, 5), myGUITexture, ScaleMode.StretchToFill, true, 10.0f);
    GUI.color = Color.green;
    GUI.DrawTexture(Rect(locationOnScreen.x - 25, Screen.height - locationOnScreen.y - 40, (transform.GetComponent(Stats).nowHP + 0.0) / transform.GetComponent(Stats).maxHP * 50.0, 5), myGUITexture, ScaleMode.StretchToFill, true, 10.0f);*/
}
