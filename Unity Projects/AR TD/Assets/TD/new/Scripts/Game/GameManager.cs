using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Game))]
public class GameManager : MonoBehaviour {

  private Game game;

  [SerializeField]
  private List<GameObject> coreGameObjects;

  private List<CharacterStats> coreGameObjectsStats;
  public List<CharacterStats> CoreGameObjectsStats {
    get {
      return coreGameObjectsStats;
    }
  }

  public List<Component> componentsToAdd;

  [System.Serializable]
  private struct EnemyPath {
    public Transform spawningPoints;
    public List<Vector3> path;
  };

  [SerializeField]
  private List<EnemyPath> enemyPaths;

  public GameObject pathArrow;
  public float distancePerArrow = 5.0f;

  [SerializeField]
  private List<GameObject> enemyPrefabs;

  [SerializeField]
  private List<GameObject> bossPrefabs;

  [SerializeField]
  private int waveThresholdForTheNextTypeOfEnemy = 5;

  private int _currentWave;
  private int currentWave {
    get {
      return _currentWave;
    }
    set {
      _currentWave = value;
    }
  }
  public int CurrentWave {
    get {
      return _currentWave;
    }
  }


  [SerializeField]
  private int _maxWave;
  private int maxWave {
    get {
      return _maxWave;
    }
    set {
      _maxWave = value;
    }
  }
  public int MaxWave {
    get {
      return _maxWave;
    }
  }

  private int _score;
  private int score {
    get {
      return _score;
    }
    set {
      if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
        return;
      }
      _score = value;
    }
  }
  public int Score {
    get {
      return _score;
    }
  }

  [SerializeField]
  private float restingTimeBeforeStart;
  public float RestingTimeBeforeStart {
    get {
      return restingTimeBeforeStart;
    }
  }

  [SerializeField]
  private float restingTimeBetweenWaves;
  public float RestingTimeBetweenWaves {
    get {
      return restingTimeBetweenWaves;
    }
  }

  private float restedTime;
  public float RestedTime {
    get {
      return restedTime;
    }
  }

  private GameConstants.GameState _gameState;
  private GameConstants.GameState gameState {
    get {
      return _gameState;
    }
    set {
      _gameState = value;
      game.gameState = value;
      if (value == GameConstants.GameState.WAIT_FOR_THE_NEXT_WAVE) {
        restedTime = 0;
        MessageManager.AddMessage("第 " + (currentWave + 1) + " 波病菌將於 " + (int)(restingTimeBetweenWaves) + " 秒後入侵");
        if ((game.GameMode == GameConstants.GameMode.SURVIVAL_NORMAL) || (game.GameMode == GameConstants.GameMode.SURVIVAL_BOSS)) {
          if (currentWave > 0) {
            int speedBonus = (int)remainingTimeOfCurrentWave * currentWave * currentWave;
            score += speedBonus;
            MessageManager.AddMessage("成功擊退第" + currentWave + "波病菌\n剩餘秒數 : " + (int)remainingTimeOfCurrentWave + "\n快速擊退加分 : " + speedBonus);
          }
        }
      }
      if (value == GameConstants.GameState.MIDDLE_OF_THE_WAVE) {
        nextGenerateEnemyTime = Time.time;
      }
    }
  }

  public GameConstants.GameState GameState {
    get {
      return _gameState;
    }
  }

  [SerializeField]
  private float maxRemainingTimeForEachWave = 90.0f;

  private float remainingTimeOfCurrentWave;
  public float RemainingTimeOfCurrentWave {
    get {
      return remainingTimeOfCurrentWave;
    }
  }

  private float timeBetweenGenerateEnemy;
  private float nextGenerateEnemyTime;

  private int numberOfEnemiesToGenerate;
  public int NumberOfEnemiesToGenerate {
    get {
      return numberOfEnemiesToGenerate;
    }
    set {
      numberOfEnemiesToGenerate = value;
    }
  }

  private int numberOfEnemiesOnMap;
  public int NumberOfEnemiesOnMap {
    get {
      return numberOfEnemiesOnMap;
    }
    set {
      numberOfEnemiesOnMap = value;
    }
  }

  public void KillEnemyWithCost(int cost) {
    score += cost;
    game.AddMoney(cost);
    --numberOfEnemiesOnMap;
  }

  public void OnSubmitScoreButtonClick() {
    game.SubmitScore(score);
  }

  void Start() {
    game = GetComponent<Game>();

    gameState = GameConstants.GameState.WAIT_FOR_THE_NEXT_WAVE;

    coreGameObjectsStats = null;
    if (coreGameObjects != null && coreGameObjects.Count > 0) {
      coreGameObjectsStats = new List<CharacterStats>(new CharacterStats[coreGameObjects.Count]);
      for (int i = 0; i < coreGameObjects.Count; ++i) {
        coreGameObjectsStats[i] = coreGameObjects[i].GetComponent<CharacterStats>();
      }
    }

    for (int i = 0; i < enemyPaths.Count; ++i) {
      if (enemyPaths[i].path.Count == 0 || enemyPaths[i].path[0] != enemyPaths[i].spawningPoints.position) {
        // Set spawing position as the first point of path
        enemyPaths[i].path.Insert(0, enemyPaths[i].spawningPoints.position);
      }
      GeneratePathArrows(enemyPaths[i], distancePerArrow);
    }

    //MessageManager.AddMessage("請建造攻擊裝置抵擋即將入侵的病菌");
  }

  void Update() {
    if (gameState == GameConstants.GameState.FINISHED || gameState == GameConstants.GameState.LOSED) {
      return;
    }

    if (coreGameObjectsStats != null && coreGameObjectsStats.Count > 0) {
      for (int i = 0; i < coreGameObjectsStats.Count; ++i) {
        if (coreGameObjectsStats[i].CurrentHP <= 0) {
          gameState = GameConstants.GameState.LOSED;
          MessageManager.AddMessage("遊戲結束，請輸入您的名稱將分數登入排行榜");
          return;
        }
      }
    }

    if (gameState == GameConstants.GameState.WAIT_FOR_THE_NEXT_WAVE) {
      restedTime += Time.deltaTime;
      if ((currentWave == 0 && restedTime >= restingTimeBeforeStart) || restedTime >= restingTimeBetweenWaves) {
        NextWave();
        gameState = GameConstants.GameState.MIDDLE_OF_THE_WAVE;
      }
      return;
    }

    if (gameState == GameConstants.GameState.MIDDLE_OF_THE_WAVE) {
      remainingTimeOfCurrentWave += GameConstants.ADDITIONAL_TIME_BY_LAST_STAND;
      GameConstants.ADDITIONAL_TIME_BY_LAST_STAND = 0;
      if (currentWave < maxWave || (game.GameMode == GameConstants.GameMode.SURVIVAL_NORMAL) || (game.GameMode == GameConstants.GameMode.SURVIVAL_BOSS)) {
        remainingTimeOfCurrentWave -= Time.deltaTime;
        if (remainingTimeOfCurrentWave < 0) {
          //gameState = GameConstants.GameState.LOSED;
          //MessageManager.AddMessage("遊戲結束，請輸入您的名稱將分數登入排行榜");
          //return;
          GenerateBosses();
          gameState = GameConstants.GameState.WAIT_FOR_THE_NEXT_WAVE;
        }
      }
      if (numberOfEnemiesToGenerate > 0) {
        if (Time.time >= nextGenerateEnemyTime) {
          GenerateEnemies();
        }
      }
      return;
    }
  }

  private void GeneratePathArrows(EnemyPath enemyPath, float distancePerArrow) {
    if (distancePerArrow <= 1e-5) {
      return;
    }

    List<Vector3> path = enemyPath.path;

    if (path.Count == 0) {
      return;
    }

    for (int i = 1; i < path.Count; ++i) {
      Vector3 arrowDirection = (path[i] - path[(i + path.Count - 1) % path.Count]).normalized;
      Quaternion arrowAngle = Quaternion.FromToRotation(Vector3.forward, arrowDirection);
      float distanceBetweenPoints = Vector3.Distance(path[i], path[(i + path.Count - 1) % path.Count]);
      float gapT = distancePerArrow / distanceBetweenPoints;
      if (distanceBetweenPoints <= 1e-5) {
        continue;
      }
      for (float t = gapT; t < 1; t += gapT) {
        Vector3 arrowPosition = Vector3.Lerp(path[(i + path.Count - 1) % path.Count], path[i], t)/* + new Vector3(0, 0.005f, 0) */;
        GameObject newArrow = Instantiate(pathArrow, arrowPosition, arrowAngle) as GameObject;
        newArrow.transform.parent = game.gameSceneParentTransform;
        newArrow.transform.localPosition = arrowPosition;
      }
    }
  }

  private void GenerateBosses() {
    int bossCount = (CurrentWave / 10) + 1;

    for (int bossNumber = 0; bossNumber < bossCount; ++bossNumber) {

      int maximalIndexRangeOfBossToGenerate = CurrentWave / 5;

      if (maximalIndexRangeOfBossToGenerate > bossPrefabs.Count) {
        maximalIndexRangeOfBossToGenerate = bossPrefabs.Count;
      }

      GameObject enemyPrefab = bossPrefabs[Random.Range(0, maximalIndexRangeOfBossToGenerate)];

      int enemyPathIndex = Random.Range(0, enemyPaths.Count);
      EnemyPath enemyPath = enemyPaths[enemyPathIndex];
      Vector3 spawningPosition = enemyPath.spawningPoints.position;

      GameObject newEnemy = CharacterGenerator.GenerateCharacter(enemyPrefab, spawningPosition, enemyPath.path);
      newEnemy.transform.parent = game.gameSceneParentTransform;

      EnemyStatsModifier.AddRandomImprovementWithWave(newEnemy, currentWave + GameConstants.BOSS_WAVE_OFFSET);
      EnemyStatsModifier.ModifyStatsWithWave(newEnemy.GetComponent<CharacterStats>(), currentWave + GameConstants.BOSS_WAVE_OFFSET);

      //newEnemy.transform.localScale *= GameConstants.BOSS_SCALE;
      //newEnemy.GetComponent<CharacterStats>().MovingSpeed = GameConstants.BOSS_SPEED;
    }

  }

  private void GenerateEnemies() {
    int indexRangeOfEnemyToGenerate = CurrentWave;

    if (waveThresholdForTheNextTypeOfEnemy > 0) {
      indexRangeOfEnemyToGenerate /= waveThresholdForTheNextTypeOfEnemy;
      ++indexRangeOfEnemyToGenerate;
    }
    if (indexRangeOfEnemyToGenerate > enemyPrefabs.Count) {
      indexRangeOfEnemyToGenerate = enemyPrefabs.Count;
    }

    GameObject enemyPrefab = enemyPrefabs[Random.Range(0, indexRangeOfEnemyToGenerate)];

    int enemyPathIndex = Random.Range(0, enemyPaths.Count);
    EnemyPath enemyPath = enemyPaths[enemyPathIndex];
    Vector3 spawningPosition = enemyPath.spawningPoints.position;

    GameObject newEnemy = CharacterGenerator.GenerateCharacter(enemyPrefab, spawningPosition, enemyPath.path);
    newEnemy.transform.parent = game.gameSceneParentTransform;

    EnemyStatsModifier.AddRandomImprovementWithWave(newEnemy, currentWave);
    EnemyStatsModifier.ModifyStatsWithWave(newEnemy.GetComponent<CharacterStats>(), currentWave);

    --numberOfEnemiesToGenerate;
    ++numberOfEnemiesOnMap;

    nextGenerateEnemyTime = Time.time + timeBetweenGenerateEnemy;
  }

  private void NextWave() {
    ++currentWave;
    /* temp */
    /* temp */
    /* temp */
    //numberOfEnemiesToGenerate = 10 + (currentWave - 1) * 5 * (int)Mathf.Pow(1.1f, currentWave);
    numberOfEnemiesToGenerate = 10 + (int)Mathf.Pow(1.2f, currentWave);
    if ((game.GameMode == GameConstants.GameMode.SURVIVAL_NORMAL) || (game.GameMode == GameConstants.GameMode.SURVIVAL_BOSS)) {
      remainingTimeOfCurrentWave = 45 + ((currentWave - 1) * 5) + GameConstants.ADDITIONAL_TIME_BY_LAST_STAND;

      // AR only
      remainingTimeOfCurrentWave *= 0.5f;
      // AR only

      GameConstants.ADDITIONAL_TIME_BY_LAST_STAND = 0;
      /* temp */
      /* temp */
      /* temp */
      if (remainingTimeOfCurrentWave > maxRemainingTimeForEachWave) {
        if (maxRemainingTimeForEachWave > 0) {
          remainingTimeOfCurrentWave = maxRemainingTimeForEachWave;
        }
      }
    }

    timeBetweenGenerateEnemy = (remainingTimeOfCurrentWave / numberOfEnemiesToGenerate) / 3.0f;

    // AR only
    timeBetweenGenerateEnemy = remainingTimeOfCurrentWave / numberOfEnemiesToGenerate;
    // AR only
  }

}
