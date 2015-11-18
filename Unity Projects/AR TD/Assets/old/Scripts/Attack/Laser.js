#pragma strict

var myRange : float;
var targetPosition : Vector3;
var dx : float;
var dy : float;
var dz : float;

private var myDist : float;

function Start () {
  var xDis : float = targetPosition.x - transform.position.x;
  var yDis : float = targetPosition.y - transform.position.y;
  var zDis : float = targetPosition.z - transform.position.z;
  var dis : float = Mathf.Sqrt(xDis * xDis + yDis * yDis + zDis * zDis);
  dx = (xDis / dis) * (5000);
  dy = (yDis / dis) * (5000);
  dz = (zDis / dis) * (5000);
}

function Update () {
  transform.position.x += dx * Time.deltaTime;
  transform.position.y += dy * Time.deltaTime;
  transform.position.z += dz * Time.deltaTime;
  myDist += Time.deltaTime * 5000;
  if (myDist >= myRange) Destroy(gameObject);
}
