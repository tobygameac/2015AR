using UnityEngine;
using System.Collections;

public partial class Game : MonoBehaviour {

  private GameConstants.SystemState _systemState;
  private GameConstants.SystemState systemState {
    get {
      return _systemState;
    }
    set {
      _systemState = value;

      UpdateCanvas();
    }
  }
  public GameConstants.SystemState SystemState {
    get {
      return _systemState;
    }
  }

  private GameConstants.PlayerState _playerState;
  private GameConstants.PlayerState playerState {
    get {
      return _playerState;
    }
    set {
      _playerState = value;

      UpdateCanvas();
    }
  }
  public GameConstants.PlayerState PlayerState {
    get {
      return _playerState;
    }
  }

  private GameConstants.GameState _gameState;
  public GameConstants.GameState gameState {
    get {
      return _gameState;
    }
    set {
      _gameState = value;

      UpdateCanvas();

      if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
        StartCoroutine(AudioManager.FadeOutAllLoopAudioClip(5.0f));
        StartCoroutine(AudioManager.PlayFadeInLoopAudioClip(finishedMusic, 5.0f));
        Time.timeScale = 0;
      }
    }
  }

}
