using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterStats : MonoBehaviour {

  [SerializeField]
  private GameObject _rangeDisplayer;
  public GameObject RangeDisplayer {
    get {
      if (_rangeDisplayer == null) {
        _rangeDisplayer = transform.Find("RangeDisplayer").gameObject;
      }
      return _rangeDisplayer;
    }
    set {
      _rangeDisplayer = value;
    }
  }

  public string description;

  [SerializeField]
  private GameConstants.BuildingID _buildingID;
  public GameConstants.BuildingID BuildingID {
    get {
      return _buildingID;
    }
  }

  [SerializeField]
  private GameConstants.EnemyID _enemyID;
  public GameConstants.EnemyID EnemyID {
    get {
      return _enemyID;
    }
  }

  // HP
  [SerializeField]
  private float _basicHP;
  private float BasicHP {
    get {
      return _basicHP;
    }
  }

  private float _currentHP;
  public float CurrentHP {
    get {
      return _currentHP;
    }
    set {
      _currentHP = (value >= MaxHP) ? MaxHP : value;
    }
  }

  private float _maxHP;
  public float MaxHP {
    get {
      return _maxHP;
    }
    set {
      _maxHP = value;
    }
  }

  [SerializeField]
  private float _hPModifier;
  public float HPModifier {
    get {
      return _hPModifier;
    }
    set {
      _hPModifier = value;
      UpdateHP();
    }
  }

  private float _hPScaler = 1.0f;
  public float HPScaler {
    get {
      return _hPScaler;
    }
    set {
      _hPScaler = value;
      UpdateHP();
    }
  }

  private void UpdateHP() {
    if (MaxHP <= 0) {
      return;
    }
    float currentHPPercent = CurrentHP / MaxHP;
    MaxHP = BasicHP * (1 + HPModifier) * HPScaler;
    CurrentHP = currentHPPercent * MaxHP;
  }

  // Damage
  [SerializeField]
  private float _basicDamage;
  public float BasicDamage {
    get {
      return _basicDamage;
    }
  }

  private float _damage;
  public float Damage {
    get {
      return _damage;
    }
    set {
      _damage = value;
    }
  }

  [SerializeField]
  private float _damageModifier;
  public float DamageModifier {
    get {
      return _damageModifier;
    }
    set {
      _damageModifier = value;
      Damage = BasicDamage * (1 + value);
    }
  }

  // Attacking speed
  [SerializeField]
  private float _basicAttackingSpeed;
  public float BasicAttackingSpeed {
    get {
      return _basicAttackingSpeed;
    }
  }

  private float _attackingSpeed;
  public float AttackingSpeed {
    get {
      return _attackingSpeed;
    }
    set {
      _attackingSpeed = value;
    }
  }

  [SerializeField]
  private float _attackingSpeedModifier;
  public float AttackingSpeedModifier {
    get {
      return _attackingSpeedModifier;
    }
    set {
      AttackingSpeed = BasicAttackingSpeed * (1 + _attackingSpeedModifier);
    }
  }

  // Moving speed
  [SerializeField]
  private float _basicMovingSpeed;
  public float BasicMovingSpeed {
    get {
      return _basicMovingSpeed;
    }
  }

  private float _movingSpeed;
  public float MovingSpeed {
    get {
      return _movingSpeed;
    }
    set {
      _movingSpeed = value;
    }
  }

  [SerializeField]
  private float _movingSpeedModifier;
  public float MovingSpeedModifier {
    get {
      return _movingSpeedModifier;
    }
    set {
      _movingSpeedModifier = (value < GameConstants.MINIMUM_LOCAL_MOVING_SPEED_MODIFIER) ? GameConstants.MINIMUM_LOCAL_MOVING_SPEED_MODIFIER : value;
      MovingSpeed = BasicMovingSpeed * (1 + _movingSpeedModifier);
    }
  }

  //
  [SerializeField]
  private int _cost;
  public int Cost {
    get {
      return _cost;
    }
    set {
      _cost = value;
    }
  }

  [SerializeField]
  private int _attackingRange;
  public int AttackingRange {
    get {
      return _attackingRange;
    }
    set {
      _attackingRange = value;
    }
  }

  private int _unitKilled;
  public int UnitKilled {
    get {
      return _unitKilled;
    }
    set {
      _unitKilled = value;
    }
  }

  [SerializeField]
  private GameObject _nextLevel;
  public GameObject NextLevel {
    get {
      return _nextLevel;
    }
    set {
      _nextLevel = value;
    }
  }

  [SerializeField]
  private List<Vector3> _path;
  public List<Vector3> Path {
    get {
      return _path;
    }
    set {
      _path = value;
    }
  }

  private void InitializeStats() {
    CurrentHP = MaxHP = BasicHP;
    Damage = BasicDamage * (1 + DamageModifier);
    AttackingSpeed = BasicAttackingSpeed * (1 + MovingSpeedModifier);
    MovingSpeed = BasicMovingSpeed * (1 + MovingSpeedModifier);
  }

  void Awake() {
    InitializeStats();
  }

}
