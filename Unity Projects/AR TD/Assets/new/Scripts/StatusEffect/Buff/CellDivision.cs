using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof(Enemy))]
[RequireComponent (typeof(CharacterStats))]
public class CellDivision : MonoBehaviour {

  public int minCellDivisionCount = GameConstants.MIN_CELL_DIVISION_COUNT;
  public int maxCellDivisionCount = GameConstants.MAX_CELL_DIVISION_COUNT;

  public float hpPercentForCellDivision = GameConstants.HP_PERCENT_FOR_CELL_DIVISION;

  private int divisionCount;

  private CharacterStats characterStats;

  private static GameManager gameManager;

  void Start() {
    if (gameManager == null) {
      gameManager = Camera.main.GetComponent<GameManager>();
    }

    characterStats = GetComponent<CharacterStats>();

    ScaleFloating scaleFloating = gameObject.AddComponent<ScaleFloating>();
    scaleFloating.floatingMagnitude = 0.3f;
    scaleFloating.floatingSpeed = 2.0f;
    scaleFloating.floatingScale = new Vector3(1, 0, 1);

    divisionCount = Random.Range(minCellDivisionCount, maxCellDivisionCount);
  }

  void Update() {
    if (characterStats.CurrentHP <= characterStats.MaxHP * hpPercentForCellDivision) {
      Division();
    }
  }

  private void Division() {
    for (int i = 0; i < divisionCount; ++i) {
      Vector3 newPosition = transform.position + Vector3.Scale(new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), transform.localScale);
      GameObject newEnemyObject = CharacterGenerator.GenerateCharacter(gameObject, newPosition, characterStats.Path);

      Destroy(newEnemyObject.GetComponent<CellDivision>());
      Destroy(newEnemyObject.GetComponent<ScaleFloating>());

      newEnemyObject.transform.localScale = transform.localScale * 0.5f;

      CharacterStats newCharacterStats = newEnemyObject.GetComponent<CharacterStats>();

      newCharacterStats.Cost = 0;

      newCharacterStats.CurrentHP = newCharacterStats.MaxHP = (1.0f / divisionCount) * characterStats.MaxHP;

      newCharacterStats.MovingSpeedModifier += Random.Range(-1, 1) * GameConstants.ENEMY_MOVING_SPEED_FLOATING_MODIFIER;

      newEnemyObject.GetComponent<Enemy>().CurrentTargetIndex = GetComponent<Enemy>().CurrentTargetIndex;
    }

    gameManager.NumberOfEnemiesOnMap += divisionCount;
    gameManager.KillEnemyWithCost(characterStats.Cost);
    Destroy(gameObject);
  }
}
