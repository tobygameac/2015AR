using UnityEngine;
using System.Collections;

public class CursorDisplayer : MonoBehaviour {

  public Texture2D cursorTexture;

  void Start() {
    Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
  }

}
