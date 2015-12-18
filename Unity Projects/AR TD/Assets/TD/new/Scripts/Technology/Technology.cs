using UnityEngine;

[System.Serializable]
public class Technology {

  private GameConstants.TechnologyID _id;
  public GameConstants.TechnologyID ID {
    get {
      return _id;
    }
  }

  private string _name;
  public string Name {
    get {
      return _name;
    }
  }

  [SerializeField]
  private int _cost;
  public int Cost {
    get {
      return _cost;
    }
  }

  [SerializeField]
  private bool _repeatable;
  public bool Repeatable {
    get {
      return _repeatable;
    }
  }

  [SerializeField]
  private GameConstants.TechnologyID[] _requiredTechnology;
  public GameConstants.TechnologyID[] RequiredTechnology {
    get {
      return _requiredTechnology;
    }
  }

  public Technology(GameConstants.TechnologyID id, int cost, GameConstants.TechnologyID[] requiredTechnology) : this(id, cost, false, requiredTechnology) {
  }

  public Technology(GameConstants.TechnologyID id, int cost, bool repeatable = false, GameConstants.TechnologyID[] requiredTechnology = null) {
    _id = id;
    _name = GameConstants.NameOfTechnologyID[(int)id];
    _cost = cost;
    _repeatable = repeatable;
    _requiredTechnology = requiredTechnology;
  }

}
