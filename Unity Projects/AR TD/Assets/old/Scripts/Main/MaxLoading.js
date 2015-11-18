#pragma strict

var maxLoad : int;

function Start () {

}

function Update () {
  if (GetComponent(WaveHandler).monsterOnMap >= maxLoad) {
    Camera.main.GetComponent(WaveHandler).isLosed = true;
    Camera.main.GetComponent(MainFunction).isLosed = true;
    Camera.main.GetComponent(MessageShow).enabled = false;
    Camera.main.GetComponent(BuildingDetails).enabled = false;
    Camera.main.GetComponent(WaveHandler).SellAll();
  }
}
