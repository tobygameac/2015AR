#pragma strict

var speed : int;
var maxX : int;
var maxZ : int;
var minS : int;
var maxS : int;

function Start () {

}

function Update () {
  var dx : float = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
  var dz : float = Input.GetAxis("Vertical") * Time.deltaTime * speed;
  transform.position.x += dx;
  transform.position.z += dz;
  // scroll
  if (Input.GetAxis("Mouse ScrollWheel")) {
    Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 4000;
  }
  // Camera boundary
  if (Camera.main.orthographicSize < minS) Camera.main.orthographicSize = minS;
  if (Camera.main.orthographicSize > maxS) Camera.main.orthographicSize = maxS;
  // Under construction
  var cameraLength : int = 0;
  if (transform.position.x < -maxX + cameraLength) transform.position.x = -maxX + cameraLength;
  if (transform.position.x > maxX - cameraLength) transform.position.x = maxX - cameraLength;
  if (transform.position.z < -maxZ + cameraLength) transform.position.z = -maxZ + cameraLength;
  if (transform.position.z > maxZ - cameraLength) transform.position.z = maxZ - cameraLength;
}
