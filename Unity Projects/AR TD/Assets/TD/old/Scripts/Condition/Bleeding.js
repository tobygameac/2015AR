#pragma strict


function Start () {
}

function Update () {
  GetComponent(Stats).nowHP -= Time.deltaTime;
}
