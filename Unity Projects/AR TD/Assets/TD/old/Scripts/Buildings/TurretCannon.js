#pragma strict

var myProjectile : GameObject;
var reloadTime : float;
var turnSpeed : float;
var firePauseTime : float;
var muzzleEffect : GameObject;
var errorAmount : float;
var target : Transform;
var muzzlePositions : Transform[];
var turretBall : Transform;

private var nextFireTime : float;
private var nextMoveTime : float;
private var desiredRotation : Quaternion;

function Start () {

}

function Update () {
  if (target) {
    if (Time.time >= nextMoveTime) {
      CalculateAimPosition(target.position);
      turretBall.rotation = Quaternion.Lerp(turretBall.rotation, desiredRotation, Time.deltaTime * turnSpeed);
    }
    if (Time.time >= nextFireTime) {
      FireProjectile();
    }
  }
}

function OnTriggerEnter (other : Collider) {
  if (!target && other.gameObject.tag == "Enemy") {
    nextFireTime = Time.time + reloadTime * 0.5;
    target = other.gameObject.transform;
  }
}

function OnTriggerStay (other : Collider) {
  if (!target && other.gameObject.tag == "Enemy") {
    nextFireTime = Time.time + reloadTime * 0.5;
    target = other.gameObject.transform;
  }
}

function OnTriggerExit (other : Collider) {
  if (other.gameObject.transform == target) target = null;
}

function CalculateAimPosition (targetPos : Vector3) {
  var aimPoint = Vector3(targetPos.x, targetPos.y, targetPos.z);
  desiredRotation = Quaternion.LookRotation(aimPoint - transform.position) * Quaternion.Euler(0, -90, 0);
}

function FireProjectile () {
  GetComponent.<AudioSource>().Play();
  nextFireTime = Time.time + reloadTime;
  nextMoveTime = Time.time + firePauseTime;

  for (theMuzzlePos in muzzlePositions) {
    var projectile : GameObject = Instantiate(myProjectile, theMuzzlePos.position, theMuzzlePos.rotation);
    projectile.GetComponent(Projectile).targetPosition = target.position;
    projectile.GetComponent(Projectile).cannon = gameObject;
    //Instantiate(muzzleEffect, theMuzzlePos.position, theMuzzlePos.rotation);
  }
}
