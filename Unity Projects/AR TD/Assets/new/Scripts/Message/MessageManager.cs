using System.Collections;
using System.Collections.Generic;

public class MessageManager {

  private static readonly int MAX_MESSAGES_COUNT = 100;

  private static MessageManager _instance;
  public static MessageManager Instance {
    get {
      return (_instance == null) ? (_instance = new MessageManager()) : _instance;
    }
  }

  private static List<string> _messages;
  public static List<string> Messages {
    get {
      if (_instance == null) {
        _instance = Instance;
      }
      return _messages;
    }
  }

  private MessageManager() {
    _messages = new List<string>();
  }

  public static void AddMessage(string message) {
    if (_instance == null) {
      _instance = Instance;
    }
    _messages.Add(message);
    if (_messages.Count >= MAX_MESSAGES_COUNT) {
      _messages.RemoveAt(0);
    }
  }

  public static void ClearAllMessages() {
    if (_instance == null) {
      _instance = Instance;
    }
    _messages.Clear();
  }
}
