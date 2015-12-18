#pragma strict

var projectile : GameObject;
var barrel : Transform;

var delay : float;
var turnSpeed : float;
var target : Transform;
var body : Transform;

private var nextFireTime : float;
private var desiredRotation : Quaternion;

function Start () {

}

function Update () {
  if (target) {
    CalculateAimPosition(target.position);
    body.rotation = Quaternion.Lerp(body.rotation, desiredRotation, Time.deltaTime * turnSpeed);
    if (Time.time >= nextFireTime) {
      Fire();
    }
  }
}

function OnTriggerEnter (other : Collider) {
  if (!target && other.gameObject.tag == "Enemy") {
    target = other.gameObject.transform;
  }
}

function OnTriggerStay (other : Collider) {
  if (!target && other.gameObject.tag == "Enemy") {
    target = other.gameObject.transform;
  }
}

function OnTriggerExit (other : Collider) {
  if (other.gameObject.transform == target) target = null;
}

function CalculateAimPosition (targetPos : Vector3) {
  var aimPoint = Vector3(targetPos.x, targetPos.y, targetPos.z);
  desiredRotation = Quaternion.LookRotation(aimPoint - transform.position) * Quaternion.Euler(0, 180, 0);
}

function Fire () {
  nextFireTime = Time.time + delay;
  var bullet : GameObject = Instantiate(projectile, barrel.position, barrel.rotation);
  bullet.GetComponent(Projectile).targetPosition = target.position;
  bullet.GetComponent(Projectile).cannon = gameObject;
  GetComponent.<AudioSource>().Play();
}
