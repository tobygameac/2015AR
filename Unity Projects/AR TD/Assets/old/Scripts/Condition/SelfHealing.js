#pragma strict

var delay : float;
var nextHealTime : float;

function Start () {
}

function Update () {
  if (Time.time >= nextHealTime) {
    if (GetComponent(Stats).nowHP < GetComponent(Stats).maxHP) {
      GetComponent(Stats).nowHP++;
    }
    nextHealTime = Time.time + delay;
  }
}
