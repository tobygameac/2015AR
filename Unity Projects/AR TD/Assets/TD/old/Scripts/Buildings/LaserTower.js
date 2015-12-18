#pragma strict

private var damage : float;
private var bonusDamage : float;

var myLaser : GameObject;
var reloadTime : float;
var target : Transform;

private var nextFireTime : float;

function Start () {
  damage = GetComponent(Stats).damage;

}

function Update () {
  if (target) {
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
  if (other.gameObject.transform == target) {
    target = null;
  }
}

function Fire () {
  GetComponent.<AudioSource>().Play();
  var laser : GameObject = Instantiate(myLaser, transform.position, transform.rotation);
  laser.GetComponent(Laser).targetPosition = target.position;
  target.GetComponent(Stats).nowHP -= (damage + bonusDamage);
  if (target.GetComponent(Stats).nowHP <= 0) {
    if (Camera.main.GetComponent(MainFunction).learnable) {
      GetComponent(Stats).kill++;
      bonusDamage = (GetComponent(Stats).kill / 10.0) / 100.0 * damage;
    }
  }
  nextFireTime = Time.time + reloadTime;
}
