#pragma strict

class Research extends System.ValueType {
  var cost : int;
  var name : String;
  public function Research (s : String, c : int) {
    this.name = s;
    this.cost = c;
  }
}

