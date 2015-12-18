using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Slider))]
public class AudioVolumeSlider : MonoBehaviour {

  private Slider slider;

  private static bool notFirstAdjustment;
  private static float volume;

  public void OnSliderValueChanged() {
    if (slider != null) {
      volume = AudioListener.volume = slider.value;
    }
  }

  void Start() {
    slider = GetComponent<Slider>();

    if (!notFirstAdjustment) {
      volume = 0.5f;
      notFirstAdjustment = true;
    }

    slider.value = AudioListener.volume = volume;
  }
}
