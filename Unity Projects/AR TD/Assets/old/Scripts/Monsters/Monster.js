#pragma strict

// Moving
var direction : Vector3;
var targetPosition : Vector3[];
var target : Vector3;
var nowTarget : int;
var dx : float;
var dz : float;

private var oldHP : float;

function Start () {
  var nowWave = Camera.main.GetComponent(WaveHandler).nowWave;
  if (nowWave < 15) {
    GetComponent(Stats).nowHP += Mathf.Pow(nowWave, 3) / 30;
    GetComponent(Stats).maxHP += Mathf.Pow(nowWave, 3) / 30;
  } else if (nowWave < 30) {
    GetComponent(Stats).nowHP += Mathf.Pow(nowWave, 3) / 20;
    GetComponent(Stats).maxHP += Mathf.Pow(nowWave, 3) / 20;
  } else if (nowWave < 45) {
    GetComponent(Stats).nowHP += Mathf.Pow(nowWave, 3) / 10;
    GetComponent(Stats).maxHP += Mathf.Pow(nowWave, 3) / 10;
  } else {
    GetComponent(Stats).nowHP += Mathf.Pow(nowWave, 3);
    GetComponent(Stats).maxHP += Mathf.Pow(nowWave, 3);
  }
  oldHP = GetComponent(Stats).maxHP;
  GetComponent(Stats).speed += Random.Range(0, 15) + (nowWave / 5);
  nowTarget = 0;
  getDirection();
}

function Update () {
  transform.position.x += dx * Time.deltaTime;
  transform.position.z += dz * Time.deltaTime;
  //transform.Translate(direction * Time.deltaTime * speed);
  //transform.Translate((target.transform.position - transform.position) * Time.deltaTime * mySpeed);
  if (transform.position.y < 1) transform.position.y = 1;
  if (GetComponent(Stats).nowHP <= 0) {
    if (oldHP == GetComponent(Stats).maxHP) { // OneShot
      Camera.main.GetComponent(WaveHandler).ShowMessage("瞬間擊殺! 獎賞加倍");
      Camera.main.GetComponent(WaveHandler).Kill(GetComponent(Stats).cost * 2);
    } else {
      Camera.main.GetComponent(WaveHandler).Kill(GetComponent(Stats).cost);
    }
    Destroy(gameObject);
  }
  if (Mathf.Abs(transform.position.x - target.x) <= 5 && Mathf.Abs(transform.position.y - target.y) <= 5 && Mathf.Abs(transform.position.z - target.z) <= 5) {
    nowTarget = (nowTarget + 1) % targetPosition.length;
    getDirection();
  }
  oldHP = GetComponent(Stats).nowHP;
}

function getDirection () {
  target = targetPosition[nowTarget];
  var xDis : float = target.x - transform.position.x;
  var zDis : float = target.z - transform.position.z;
  var dis : float = Mathf.Sqrt(xDis * xDis + zDis * zDis);
  dx = (xDis / dis) * (GetComponent(Stats).speed);
  dz = (zDis / dis) * (GetComponent(Stats).speed);
  // direction = Vector3(xDis / dis, 0, zDis / dis);
}
