using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CharacterStats))]
public class Enemy : MonoBehaviour {

  private static GameManager gameManager;
  private CharacterStats characterStats;

  private float previousHP;

  private int currentTargetIndex;
  public int CurrentTargetIndex {
    get {
      return currentTargetIndex;
    }
    set {
      currentTargetIndex = value;
    }
  }

  [SerializeField]
  private float minStoppingDistance = 0.1f;

  void Start() {
    if (gameManager == null) {
      gameManager = Camera.main.GetComponent<GameManager>();
    }

    characterStats = GetComponent<CharacterStats>();

    previousHP = characterStats.MaxHP;
  }

  void Update() {
    float movingSpeed = characterStats.MovingSpeed * (1 + GameConstants.GLOBAL_ENEMY_SPEED_MODIFIER);

    transform.localPosition = Vector3.MoveTowards(transform.localPosition, characterStats.Path[currentTargetIndex], movingSpeed * Time.deltaTime);

    if (characterStats.CurrentHP <= 0) {
      if (previousHP == characterStats.MaxHP) { // One shot kill
        int oneShotKillBonus = (int)(characterStats.Cost * GameConstants.ONE_SHOT_KILL_BONUS_MODIFIER);
        gameManager.KillEnemyWithCost(characterStats.Cost + oneShotKillBonus);
        if (oneShotKillBonus > 0) {
          MessageManager.AddMessage("瞬間擊殺! 獲得額外" + oneShotKillBonus + "金錢與分數");
        }
      } else {
        gameManager.KillEnemyWithCost(characterStats.Cost);
      }
      Destroy(gameObject);
    }

    previousHP = characterStats.CurrentHP;

    if (CloseEnoughToCurrentTarget()) {
      currentTargetIndex = (currentTargetIndex + 1) % characterStats.Path.Count;
      transform.localEulerAngles = Quaternion.LookRotation(characterStats.Path[currentTargetIndex] - transform.localPosition).eulerAngles;
    }
  }

  private bool CloseEnoughToCurrentTarget() {
    return (Vector3.Distance(transform.localPosition, characterStats.Path[currentTargetIndex]) <= minStoppingDistance);
  }
}
