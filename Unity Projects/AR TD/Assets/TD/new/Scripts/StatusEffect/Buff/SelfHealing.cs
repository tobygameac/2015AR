using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterStats))]
public class SelfHealing : MonoBehaviour {

  private CharacterStats characterStats;

  void Start() {
    characterStats = GetComponent<CharacterStats>();

    for (int i = 0; i < transform.childCount; ++i) {
      Renderer targetRenderer = transform.GetChild(i).GetComponent<Renderer>();
      if (targetRenderer != null) {
        targetRenderer.material.color = Color.green;
      }
    }
  }

  void Update() {
    characterStats.CurrentHP += characterStats.MaxHP * GameConstants.HP_PERCENT_REGENERATING_PER_SECOND_OF_SELF_HEALING * Time.deltaTime;
  }

}
