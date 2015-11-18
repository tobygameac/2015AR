using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterStats))]
public class Strongger : MonoBehaviour {

  public float hpScale = GameConstants.HP_SCALE_OF_STRONGGER;
  public float sizeScale = GameConstants.SIZE_SCALE_OF_STRONGGER;
  public float movingSpeedModifier = GameConstants.MOVING_SPEED_MODIFIER_OF_STRONGGER;

  private CharacterStats characterStats;

  void Start() {
    characterStats = GetComponent<CharacterStats>();

    characterStats.HPScaler *= hpScale;
    characterStats.MovingSpeedModifier += movingSpeedModifier;

    transform.localScale *= sizeScale;
  }

}
