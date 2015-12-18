#pragma strict

var maxDivision : int;
var nowDivision : int;

function Start () {
  nowDivision = 0;
}

function Update () {
  // HP is lower than 20%
  if ((GetComponent(Stats).nowHP > 0) && ((GetComponent(Stats).nowHP / GetComponent(Stats).maxHP) < 0.2)) {
    if (nowDivision < maxDivision) {
      var newObject : GameObject = Instantiate(gameObject);

      // Target
      newObject.GetComponent(Monster).nowTarget = GetComponent(Monster).nowTarget;
      newObject.GetComponent(Monster).getDirection();

      // No money
      newObject.GetComponent(Stats).cost = 0;

      // Not to divide again
      Destroy(newObject.GetComponent(CellDivision));
      Destroy(newObject.GetComponent(BigResizer));

      // Size
      newObject.transform.localScale.x *= 0.5;
      newObject.transform.localScale.z *= 0.5;

      // location
      newObject.transform.Translate(Random.Range(-20, 20), 0, Random.Range(-20, 20));

      // HP
      newObject.GetComponent(Stats).maxHP *= 0.25;
      newObject.GetComponent(Stats).nowHP = newObject.GetComponent(Stats).maxHP;

      // Speed
      newObject.GetComponent(Stats).speed += Random.Range(-5, 20);

      nowDivision++;
      Camera.main.GetComponent(WaveHandler).monsterOnMap++;
    } else {
      Camera.main.GetComponent(WaveHandler).monsterOnMap--;
      Destroy(gameObject);
    }
  }
}
