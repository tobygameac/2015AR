using UnityEngine;

public class Core : MonoBehaviour {

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      CharacterStats targetCharacterStats = collider.GetComponent<CharacterStats>();
      targetCharacterStats.CurrentHP = 0;
    }
  }

  void OnTriggerStay(Collider collider) {
    if (collider.gameObject.tag == "Enemy") {
      CharacterStats targetCharacterStats = collider.GetComponent<CharacterStats>();
      targetCharacterStats.CurrentHP = 0;
    }
  }
}
