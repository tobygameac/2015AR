#pragma strict

var giver : GameObject;
var damage : float;
var bonusDamage : float;

function Start () {
}

function Update () {
  transform.GetComponent(Stats).nowHP -= (Time.deltaTime) * (damage + bonusDamage);
  if (transform.GetComponent(Stats).nowHP <= 0) {
    if (giver && Camera.main.GetComponent(MainFunction).learnable) {
      giver.GetComponent(Stats).kill++;
    }
  }
}
