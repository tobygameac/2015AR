using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterGenerator : MonoBehaviour {

  private static CharacterGenerator instance;

  void Awake() {
    if (instance != null && instance != this) {
      Destroy(this.gameObject);
      return;
    }
    instance = this;
  }

  public static CharacterGenerator GetInstance() {
    return instance;
  }

  public static GameObject GenerateCharacter(GameObject characterPrefab, Vector3 position, List<Vector3> path = null) {
    return GenerateCharacter(characterPrefab, position, Quaternion.identity, path);
  }

  public static GameObject GenerateCharacter(GameObject characterPrefab, Vector3 position, Quaternion rotation, List<Vector3> path = null) {
    GameObject character = Instantiate(characterPrefab, position, rotation) as GameObject;
    
    CharacterStats characterStats = character.GetComponent<CharacterStats>();
    if (characterStats == null) {
      character.AddComponent<CharacterStats>();
    }

    characterStats.Path = path;

    return character;
  }
}
