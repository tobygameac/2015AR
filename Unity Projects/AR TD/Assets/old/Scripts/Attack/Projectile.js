#pragma strict

var cannon : GameObject;

private var damage : float;
private var bonusDamage : float;

var mySpeed : float;
var myRange : float;
var targetPosition : Vector3;
var dx : float;
var dy : float;
var dz : float;

private var myDist : float;

function Start () {
  damage = cannon.GetComponent(Stats).damage;
  var xDis : float = targetPosition.x - transform.position.x;
  var yDis : float = targetPosition.y - transform.position.y;
  var zDis : float = targetPosition.z - transform.position.z;
  var dis : float = Mathf.Sqrt(xDis * xDis + yDis * yDis + zDis * zDis);
  dx = (xDis / dis) * (mySpeed);
  dy = (yDis / dis) * (mySpeed);
  dz = (zDis / dis) * (mySpeed);
}

function Update () {
  transform.position.x += dx * Time.deltaTime;
  transform.position.y += dy * Time.deltaTime;
  transform.position.z += dz * Time.deltaTime;
  myDist += Time.deltaTime * mySpeed;
  if (myDist >= myRange) Destroy(gameObject);
}

function OnCollisionEnter (collision : Collision) {
  if (collision.gameObject.tag == "Enemy") {
    collision.gameObject.GetComponent(Stats).nowHP -= (damage + bonusDamage);
    if (collision.gameObject.GetComponent(Stats).nowHP <= 0) {
      if (Camera.main.GetComponent(MainFunction).learnable && cannon) {
        cannon.GetComponent(Stats).kill++;
        bonusDamage = (cannon.GetComponent(Stats).kill / 10.0) / 100.0 * damage;
      }
    }
    Destroy(gameObject);
  }
}
