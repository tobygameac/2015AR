using UnityEngine;
using System.Collections;

public class EnemyStatsModifier : MonoBehaviour {
  
  private static EnemyStatsModifier instance;

  public static EnemyStatsModifier GetInstance() {
    return instance;
  }

  void Awake() {
    if (instance != null && instance != this) {
      Destroy(this.gameObject);
      return;
    }
    instance = this;
  }

  public static void ModifyStatsWithWave(CharacterStats characterStats, int wave) {
    for (int w = 1, hpScalerIndex = 0; w <= wave; ++w) {
      if (w > GameConstants.WAVE_THERSHOLD_FOR_THE_NEXT_HP_SCALER[hpScalerIndex]) {
        if (hpScalerIndex < GameConstants.WAVE_THERSHOLD_FOR_THE_NEXT_HP_SCALER.Length) {
          ++hpScalerIndex;
        }
      }
      characterStats.HPScaler *= GameConstants.HP_SCALER_FOR_WAVE[hpScalerIndex];
    }
    characterStats.Cost += (int)(characterStats.Cost * (wave - 1) * GameConstants.COST_MODIFIER_FOR_EACH_WAVE);

    characterStats.MovingSpeedModifier += Random.Range(-1, 1) * GameConstants.ENEMY_MOVING_SPEED_FLOATING_MODIFIER;

    characterStats.MovingSpeedModifier += (wave - 1) * GameConstants.ENEMY_MOVING_SPEED_MODIFIER_FOR_EACH_WAVE; 
  }

  public static void AddRandomImprovementWithWave(GameObject targetCharacter, int wave) {
    float probabilityScale = wave * GameConstants.IMPROVEMENT_PROBABILITY_SCALE_PER_WAVE;
    float dice = Random.Range(0.0f, 1.0f);

    CharacterStats characterStats = targetCharacter.GetComponent<CharacterStats>();

    if (dice < GameConstants.PROBABILITY_OF_STRONGGER * probabilityScale) {
      characterStats.Cost = (int)(characterStats.Cost * GameConstants.COST_SCALE_OF_STRONGGER);
      targetCharacter.AddComponent<Strongger>();
      return;
    }

    dice = Random.Range(0.0f, 1.0f);
    if (dice < GameConstants.PROBABILITY_OF_INSANE * probabilityScale) {
      characterStats.Cost = (int)(characterStats.Cost * GameConstants.COST_SCALE_OF_INSANE);
      targetCharacter.AddComponent<Insane>();
      return;
    }

    dice = Random.Range(0.0f, 1.0f);
    if (dice < GameConstants.PROBABILITY_OF_SELF_HEALING * probabilityScale) {
      characterStats.Cost = (int)(characterStats.Cost * GameConstants.COST_SCALE_OF_SELF_HEALING);
      targetCharacter.AddComponent<SelfHealing>();
      return;
    }

    //dice = Random.Range(0.0f, 1.0f);
    //if (dice < GameConstants.PROBABILITY_OF_CELL_DIVISION * probabilityScale) {
    //  characterStats.Cost = (int)(characterStats.Cost * GameConstants.COST_SCALE_OF_CELL_DIVISION);
    //  targetCharacter.AddComponent<CellDivision>();
    //  return;
    //}
  }
}
