using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterStats))]
public class Insane : MonoBehaviour {

  public float movingSpeedModifier = GameConstants.MOVING_SPEED_MODIFIER_OF_INSANE;

  private CharacterStats characterStats;

  void Start() {
    characterStats = GetComponent<CharacterStats>();
    characterStats.MovingSpeedModifier += movingSpeedModifier;

    for (int i = 0; i < transform.childCount; ++i) {
      Renderer targetRenderer = transform.GetChild(i).GetComponent<Renderer>();
      if (targetRenderer != null) {
        targetRenderer.material.color = Color.red;
      }
    }
  }

}
