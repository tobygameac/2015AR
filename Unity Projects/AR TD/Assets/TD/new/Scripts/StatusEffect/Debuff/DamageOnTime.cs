using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterStats))]
public class DamageOnTime : MonoBehaviour {

  private CharacterStats characterStats;

  public float damage;

  void Start() {
    characterStats = GetComponent<CharacterStats>();
  }

  void Update() {
    characterStats.CurrentHP -= damage * Time.deltaTime;
  }

}
