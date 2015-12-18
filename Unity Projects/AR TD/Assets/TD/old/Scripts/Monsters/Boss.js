#pragma strict

// Materials
var materialGeneral : Material;
var materialAttack : Material;

// Attack
var damage : float;
var attackDelay : float;
var nextAttackTime : float;
var isAttacking : boolean;

// Move
var movable : boolean;
var dx : float;
var dz : float;
private var desiredRotation : Quaternion;
var turnSpeed : float = 500.0;

// Target
var target : GameObject;

// Sound
var attackSound : AudioClip;
var destroySound : AudioClip;


function Start () {
  movable = false;
  target = null;
  dx = dz = 0;
  nextAttackTime = Time.time;
}

function Update () {
  if (!movable) {
    GetComponent.<Renderer>().enabled = false;
    if (GetComponent.<Camera>().main.GetComponent(WaveHandler).restTime < 0) {
      GetComponent.<Camera>().main.GetComponent(WaveHandler).ShowMessage("超級病菌出現");
      GetComponent.<Camera>().main.GetComponent(WaveHandler).ShowMessage("想辦法抵擋超級病菌的攻勢");
      movable = true;
      GetComponent.<Renderer>().enabled = true;
    }
    return;
  }
  if (target) {
    if (Mathf.Abs(transform.position.x - target.transform.position.x) <= 40 && Mathf.Abs(transform.position.z - target.transform.position.z) <= 40) {
      transform.position.x = target.transform.position.x;
      transform.position.z = target.transform.position.z;
      if (!isAttacking && Time.time >= nextAttackTime) {
        isAttacking = true;
        Attack();
        isAttacking = false;
      }
    } else {
      transform.position.x += dx * Time.deltaTime;
      transform.position.z += dz * Time.deltaTime;
    }
  }
}

function OnTriggerStay (other : Collider) {
  if (!target && other.gameObject.layer == LayerMask.NameToLayer("Building")) {
    target = other.gameObject;
    while (target.transform.parent) {
      target = target.transform.parent.gameObject;
    }
    getDirection();
  }
}

function Attack () {
  nextAttackTime = Time.time + attackDelay;
  target.GetComponent(Stats).nowHP -= damage;

  // Destroy building
  if (target.GetComponent(Stats).nowHP <= 0) {
    GetComponent.<AudioSource>().PlayOneShot(destroySound);
    Destroy(target.gameObject);
    GetComponent.<Camera>().main.GetComponent(MainFunction).nowBuildingNumber--;

    // Stop moving
    target = null;
    dx = dz = 0;
  }
  GetComponent.<AudioSource>().PlayOneShot(attackSound);
  GetComponent.<Renderer>().material = materialAttack;
  yield WaitForSeconds(0.5);
  GetComponent.<Renderer>().material = materialGeneral;
}

function getDirection () {
  // Move
  var xDis : float = target.transform.position.x - transform.position.x;
  var zDis : float = target.transform.position.z - transform.position.z;
  var dis : float = Mathf.Sqrt(xDis * xDis + zDis * zDis);
  dx = (xDis / dis) * (GetComponent(Stats).speed);
  dz = (zDis / dis) * (GetComponent(Stats).speed);

  // Rotation
  var aimPoint = Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
  desiredRotation = Quaternion.LookRotation(aimPoint - transform.position) * Quaternion.Euler(0, -90, 0);
  transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * turnSpeed);
}
