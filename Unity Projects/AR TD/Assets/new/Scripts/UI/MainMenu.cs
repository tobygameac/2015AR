using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour {

  public string versionMessage;
  public GameObject versionTextObject;
  private Text versionText;

  public AudioClip backgroundMusic;

  public AudioClip buttonSound;

  public void OnStartButtonClick() {
    AudioManager.PlayAudioClip(buttonSound);
    Time.timeScale = 1;
    Application.LoadLevel("SurvivalNormal");
  }

  public void OnScoreboardButtonClick() {
    AudioManager.PlayAudioClip(buttonSound);
    Application.LoadLevel("Scoreboard");
  }

  public void OnExitButtonClick() {
    AudioManager.PlayAudioClip(buttonSound);
    Application.Quit();
  }

  void Start() {
    StartCoroutine(AudioManager.PlayFadeInLoopAudioClip(backgroundMusic, 10.0f));
    if (versionTextObject != null) {
      versionText = versionTextObject.GetComponent<Text>();
      versionText.text = versionMessage;
    }
  }
}
