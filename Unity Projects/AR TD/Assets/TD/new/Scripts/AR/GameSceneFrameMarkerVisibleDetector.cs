using UnityEngine;
using Vuforia;
using System.Collections;

[RequireComponent(typeof(MarkerBehaviour))]
public class GameSceneFrameMarkerVisibleDetector : MonoBehaviour {

  public GameObject coreFrameMarkerMissingCanvas;

  private float originalTimeScale;

  private MeshRenderer meshRenderer;
  private MarkerBehaviour markerBehaviour;

  private static Game game;

  void Start() {
    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }

    markerBehaviour = GetComponent<MarkerBehaviour>();
  }

  void Update() {
    if (game.SystemState == GameConstants.SystemState.PLAYING && (game.gameState == GameConstants.GameState.MIDDLE_OF_THE_WAVE || game.gameState == GameConstants.GameState.WAIT_FOR_THE_NEXT_WAVE)) {
      if (markerBehaviour.CurrentStatus == TrackableBehaviour.Status.NOT_FOUND ||
        markerBehaviour.CurrentStatus == TrackableBehaviour.Status.UNDEFINED ||
        markerBehaviour.CurrentStatus == TrackableBehaviour.Status.UNKNOWN) {
        coreFrameMarkerMissingCanvas.SetActive(true);
        Time.timeScale = 0;
      } else {
        coreFrameMarkerMissingCanvas.SetActive(false);
        Time.timeScale = 1.0f;
      }
    } else {
      coreFrameMarkerMissingCanvas.SetActive(false);
      Time.timeScale = 0;
    }
  }
}
