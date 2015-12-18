#pragma strict

var bornPoint : GameObject;

var nextAddTime : float;
var delayTime : float;

private var newBornPoint : GameObject;
var pushTime : float;
var push : boolean;

// Border
var limitSmall : int;
var limitBig : int;

// Sound
var evilLaugh : AudioClip;

function Start () {
  nextAddTime = 0;
  push = true;
}

function Update () {
  if (!push && Time.time > pushTime) { // Push this position into WaveHandler
    push = true;
    GetComponent(WaveHandler).initialPositions.Push(newBornPoint.transform.position);
  }
  if (Time.time > nextAddTime) {
    GetComponent.<AudioSource>().PlayOneShot(evilLaugh);
    GetComponent(MessageShow).AddMessage("場上出現了新的病菌入侵點");
    /* var xMinus = Random.Range(0, 2);
    var zMinus = Random.Range(0, 2);
    var xBig = Random.Range(0, 2);
    var newBornPoint : GameObject;
    if (xBig) { // x cannot smaller than xLimitSmall
      newBornPoint = Instantiate(bornPoint, Vector3(Random.Range(xLimitSmall, xLimitBig) * (xMinus ? -1 : 1), 1, Random.Range(0, zLimitBig) * (zMinus ? -1 : 1)), Quaternion.Euler(-90, 0, 0));
    } else { // z cannot smaller than xLimitSmall
      newBornPoint = Instantiate(bornPoint, Vector3(Random.Range(0, xLimitBig) * (xMinus ? -1 : 1), 1, Random.Range(zLimitSmall, zLimitBig) * (zMinus ? -1 : 1)), Quaternion.Euler(-90, 0, 0));
    } */
    var newX : int = 0;
    var newZ : int = 0;
    while (newX == 0 && newZ == 0) {
      var xType : int = Random.Range(0, 3);
      var zType : int = Random.Range(0, 3);
      var value : int = Random.Range(limitSmall, limitBig);
      if (xType == 0) {
        newX = 0;
      } else if (xType == 1) {
        newX = value;
      } else {
        newX = -value;
      }
      if (zType == 0) {
        newZ = 0;
      } else if (zType == 1) {
        newZ = value;
      } else {
        newZ = -value;
      }
    }
    newBornPoint = Instantiate(bornPoint, Vector3(newX, 1, newZ), Quaternion.Euler(-90, 0, 0));
    pushTime = Time.time + 15; // Wait for 15 seconds
    push = false;
    nextAddTime = Time.time + delayTime;

    // Move camera
    transform.position.x = newX;
    transform.position.z = newZ;
  }
}
