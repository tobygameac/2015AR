#pragma strict

function Start () {
  transform.localScale.x *= 1.5;
  transform.localScale.z *= 1.5;
  GetComponent(Stats).nowHP *= 10;
  GetComponent(Stats).maxHP *= 10;
  GetComponent(Monster).dx *= 0.25;
  GetComponent(Monster).dz *= 0.25;
}

function Update () {

}
