#pragma strict

var angle : Vector3;

function Start () {
  angle = Vector3(1, 1, 1);
}

function Update () {
  transform.Rotate(angle * 25.0f * Time.deltaTime, Space.World);
}
