using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

  public float speed;

  public float scrollingSpeed;
  public float maxCameraSize;
  public float minCameraSize;

  public int maxEdgeDistanceToMove = 5;
  public float edgeMovingModifier = 0.5f;

  public Vector3 minBoundPostion;
  public Vector3 maxBoundPostion;

  void Update () {
    
    float deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
    float deltaZ = Input.GetAxis("Vertical") * Time.deltaTime * speed;
    
    // Moving when mouse is close to the edge of the screen
    
    if (Input.mousePosition.x <= maxEdgeDistanceToMove) {
      deltaX = -edgeMovingModifier * speed * Time.deltaTime;
    }

    if (Input.mousePosition.y <= maxEdgeDistanceToMove) {
      deltaZ = -edgeMovingModifier * speed * Time.deltaTime;
    }

    if (Input.mousePosition.x >= Screen.width - maxEdgeDistanceToMove) {
      deltaX = edgeMovingModifier * speed * Time.deltaTime;
    }

    if (Input.mousePosition.y >= Screen.height - maxEdgeDistanceToMove) {
      deltaZ = edgeMovingModifier * speed * Time.deltaTime;
    }

    Vector3 newPosition = transform.position + new Vector3(deltaX, 0, deltaZ);

    newPosition = Vector3.Max(newPosition, minBoundPostion);
    newPosition = Vector3.Min(newPosition, maxBoundPostion);

    transform.position = newPosition;

    // Scroll

    if (Input.GetAxis("Mouse ScrollWheel") != 0) {
      Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scrollingSpeed;
    }

    Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, minCameraSize);
    Camera.main.orthographicSize = Mathf.Min(Camera.main.orthographicSize, maxCameraSize);

  }

}
