using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CanvasGroup))]
public class CanvasGroupAlphaFloating : MonoBehaviour {

  public bool loop;
  public float floatingTime;
  private float floatedTime;
  public float floatingSpeed;

  public float baseAlpha = 0.5f;
  public float alphaFloatingRange = 0.25f;

  private CanvasGroup canvasGroup;

  private float originalAlpha;

  void Start() {
    canvasGroup = GetComponent<CanvasGroup>();
    originalAlpha = canvasGroup.alpha;
  }

  void Update() {
    if (!loop) {
      floatedTime += Time.deltaTime;
      if (floatedTime >= floatingTime) {
        canvasGroup.alpha = originalAlpha;
        return;
      }
    }
    canvasGroup.alpha = 0.5f + (Mathf.Sin(Time.time * floatingSpeed) + 1) * 0.5f * alphaFloatingRange;
  }

}
