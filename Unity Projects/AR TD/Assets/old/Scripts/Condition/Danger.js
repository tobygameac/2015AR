#pragma strict

var timeRemain : float;
var originalColor : Color;

function Start () {
  originalColor = transform.GetComponent.<Light>().color;
  transform.GetComponent.<Light>().color = Color.red;
  timeRemain = 0.2;
}

function Update () {
  timeRemain -= Time.deltaTime;
  if (timeRemain <= 0) {
    transform.GetComponent.<Light>().color = originalColor;
    Destroy(transform.GetComponent(Danger));
  }
}
