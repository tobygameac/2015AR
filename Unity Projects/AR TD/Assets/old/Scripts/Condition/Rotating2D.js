#pragma strict

var speed : float;

function Start () {
  speed = 100;
}

function Update () {
  transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.World);
}
