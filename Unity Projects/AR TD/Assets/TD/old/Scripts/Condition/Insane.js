#pragma strict

function Start () {
  GetComponent(Monster).dx *= 1.2;
  GetComponent(Monster).dz *= 1.2;
  GetComponent.<Renderer>().material.color = Color.red;
}

function Update () {

}
