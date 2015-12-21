using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class GameSceneFrameMarkerVisibleDetector : MonoBehaviour {

  public GameObject coreFrameMarkerMissingCanvas;

  private float originalTimeScale;

  private MeshRenderer meshRenderer;

  private static Game game;

  void Start() {
    meshRenderer = GetComponent<MeshRenderer>();

    if (game == null) {
      game = Camera.main.GetComponent<Game>();
    }
  }

  void Update() {
    if (game.SystemState == GameConstants.SystemState.PLAYING && (game.gameState == GameConstants.GameState.MIDDLE_OF_THE_WAVE || game.gameState == GameConstants.GameState.WAIT_FOR_THE_NEXT_WAVE)) {
      if (meshRenderer.enabled) {
        coreFrameMarkerMissingCanvas.SetActive(false);
        Time.timeScale = 1.0f;
      } else {
        coreFrameMarkerMissingCanvas.SetActive(true);
        Time.timeScale = 0;
      }
    } else {
      coreFrameMarkerMissingCanvas.SetActive(false);
      Time.timeScale = 0;
    }
  }
}
