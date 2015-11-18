#pragma strict

private var damage : float;

function Start () {
  damage = GetComponent(Stats).damage;
}

function Update () {

}

function OnTriggerEnter (other : Collider) {
  if (other.gameObject.tag == "Enemy") {
    if (!other.gameObject.GetComponent(Slowing)) {
      Destroy(other.GetComponent(Speeding));
      other.gameObject.AddComponent(Slowing);
      other.gameObject.GetComponent(Slowing).damage = damage;
    }
  }
}

function OnTriggerStay (other : Collider) {
  if (other.gameObject.tag == "Enemy") {
    if (!other.gameObject.GetComponent(Slowing)) {
      Destroy(other.GetComponent(Speeding));
      other.gameObject.AddComponent(Slowing);
      other.gameObject.GetComponent(Slowing).damage = damage;
    }
  }
}

function OnTriggerExit (other : Collider) {
  if (other.gameObject.tag == "Enemy") {
    if (!other.gameObject.GetComponent(Speeding)) {
      Destroy(other.GetComponent(Slowing));
      other.gameObject.AddComponent(Speeding);
      other.gameObject.GetComponent(Speeding).damage = 1.0 / damage;
    }
  }
}
