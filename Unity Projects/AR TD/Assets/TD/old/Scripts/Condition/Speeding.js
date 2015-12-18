#pragma strict

var damage : float;

function Start () {
  GetComponent(Monster).dx *= damage;
  GetComponent(Monster).dz *= damage;
}

function Update () {
}
