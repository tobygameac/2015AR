#pragma strict

// Sound
var hitSound : AudioClip;

// State
var isDanger : boolean;

function Start () {
}

function Update () {
  if (GetComponent(Stats).nowHP <= 5 && !isDanger) {
    Camera.main.GetComponent.<AudioSource>().Stop();
    GetComponent.<AudioSource>().Play();
    isDanger = true;
  }
  if (GetComponent(Stats).nowHP > 5 && isDanger) {
    GetComponent.<AudioSource>().Stop();
    Camera.main.GetComponent.<AudioSource>().Play();
    isDanger = false;
  }
}

function OnCollisionEnter (collision : Collision) {
  if (collision.gameObject.tag == "Enemy") {
    GetComponent.<AudioSource>().PlayOneShot(hitSound);
    if (!GameObject.Find("Light").GetComponent(Danger)) {
      GameObject.Find("Light").AddComponent(Danger);
    }
    GetComponent(Stats).nowHP--;
    Camera.main.GetComponent(WaveHandler).Kill(0);
    Camera.main.GetComponent(MainFunction).ShowMessage("傷口遭到入侵!");
    Destroy(collision.gameObject);
    if (GetComponent(Stats).nowHP <= 0) {
      GetComponent.<AudioSource>().Stop();
      Camera.main.GetComponent(WaveHandler).isLosed = true;
      Camera.main.GetComponent(MainFunction).isLosed = true;
      Camera.main.GetComponent(MessageShow).enabled = false;
      Camera.main.GetComponent(BuildingDetails).enabled = false;
      Camera.main.GetComponent(WaveHandler).SellAll();
      Time.timeScale = 0;
    }
  }
}
