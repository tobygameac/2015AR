using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(CharacterStats))]
public class Core : MonoBehaviour {

  private CharacterStats characterStats;

  void Start() {
    characterStats = GetComponent<CharacterStats>();
  }

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      CharacterStats targetCharacterStats = collider.GetComponent<CharacterStats>();
      targetCharacterStats.CurrentHP = 0;
      --characterStats.CurrentHP;
    }
  }
}
