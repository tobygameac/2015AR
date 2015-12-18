using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MessageDisplayer : MonoBehaviour {

  public int maxDisplayMessageNumber = 5;

  private Text text;

  void Start() {
    text = GetComponent<Text>();
  }

  void Update() {
    string messageToDisplay = "";
    bool lastMessage = true;
    for (int i = MessageManager.Messages.Count - 1; i >= Mathf.Max(0, MessageManager.Messages.Count - maxDisplayMessageNumber); --i) {
      if (lastMessage) {
        messageToDisplay += "<color=red>" + MessageManager.Messages[i] + "</color><color=#c1da9340><i>\n";
        lastMessage = false;
      } else {
        messageToDisplay += MessageManager.Messages[i] + "\n";
      }
    }
    messageToDisplay += "</i></color>";
    text.text = messageToDisplay;
  }
}
