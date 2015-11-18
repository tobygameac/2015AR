#pragma strict

var scale : float;
private var smaller : boolean;
private var originalScale : Vector3;

function Start () {
  smaller = false;
  originalScale = transform.localScale;
}

function Update () {
  if (smaller) {
    transform.localScale -= Vector3(3 * Time.deltaTime, 0, 3 * Time.deltaTime);
  } else {
    transform.localScale += Vector3(3 * Time.deltaTime, 0, 3 * Time.deltaTime);
  }
  if (transform.localScale.x > scale * originalScale.x) {
    smaller = true;
  } else if (transform.localScale.x < originalScale.x) {
    smaller = false;
  }
}
