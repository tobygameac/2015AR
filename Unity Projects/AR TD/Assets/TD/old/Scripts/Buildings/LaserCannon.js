#pragma strict

private var damage : float;
private var bonusDamage : float;

var myLaser : GameObject;
var reloadTime : float;
var turnSpeed : float;
var firePauseTime : float;
var target : Transform;
var muzzlePositions : Transform[];
var turretBall : Transform;

private var nextFireTime : float;
private var nextMoveTime : float;
private var desiredRotation : Quaternion;

function Start () {
  damage = GetComponent(Stats).damage;
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

  for (muzzle in muzzlePositions) {
    var laser : GameObject = Instantiate(myLaser, muzzle.position, muzzle.rotation);
    laser.GetComponent(Laser).targetPosition = target.position;
    target.GetComponent(Stats).nowHP -= (damage + bonusDamage);
    if (target.GetComponent(Stats).nowHP <= 0) {
      if (Camera.main.GetComponent(MainFunction).learnable) {
        GetComponent(Stats).kill++;
        bonusDamage = (GetComponent(Stats).kill / 10.0) / 100.0 * damage;
      }
    }
  }
}
