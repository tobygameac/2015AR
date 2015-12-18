#pragma strict

private var damage : float;
var target : GameObject;
var burningEffect : Transform;

var targetLosingTime : float;

function Start () {
  damage = GetComponent(Stats).damage;
  burningEffect = transform.Find("burning");
  targetLosingTime = Time.time - 5;
}

function Update () {
  if (Time.time - targetLosingTime <= 3) {
    if (!GetComponent.<AudioSource>().loop) {
      GetComponent.<AudioSource>().Play();
      GetComponent.<AudioSource>().loop = true;
    }
    burningEffect.GetComponent.<Renderer>().enabled = true;
  } else {
    GetComponent.<AudioSource>().Stop();
    GetComponent.<AudioSource>().loop = false;
    burningEffect.GetComponent.<Renderer>().enabled = false;
  }
  if (target) {
    target = null;
    targetLosingTime = Time.time;
  }
}

function OnTriggerEnter (other : Collider) {
  if (other.gameObject.tag == "Enemy") {
    target = other.gameObject;
    if (!other.gameObject.GetComponent(Burning)) {
      other.gameObject.AddComponent(Burning);
      other.gameObject.GetComponent(Burning).damage = damage;
      other.gameObject.GetComponent(Burning).bonusDamage = ((GetComponent(Stats).kill / 10.0f) / 100.0f) * damage;
      other.gameObject.GetComponent(Burning).giver = gameObject;
    } else {
      other.gameObject.GetComponent(Burning).damage += damage;
      other.gameObject.GetComponent(Burning).bonusDamage += ((GetComponent(Stats).kill / 10.0f) / 100.0f) * damage;
      other.gameObject.GetComponent(Burning).giver = gameObject;
    }
  }
}

function OnTriggerStay (other : Collider) {
  if (other.gameObject.tag == "Enemy") {
    target = other.gameObject;
  }
}

function OnTriggerExit (other : Collider) {
  if (other.gameObject.tag == "Enemy") {
    if (other.gameObject.GetComponent(Burning)) {
      other.gameObject.GetComponent(Burning).damage -= damage;
      other.gameObject.GetComponent(Burning).bonusDamage -= GetComponent(Stats).kill / 10;
      if (other.gameObject.GetComponent(Burning).damage < 0 || other.gameObject.GetComponent(Burning).bonusDamage < 0) {
        other.gameObject.GetComponent(Burning).damage = other.gameObject.GetComponent(Burning).bonusDamage = 0;
      }
    }
  }
}
