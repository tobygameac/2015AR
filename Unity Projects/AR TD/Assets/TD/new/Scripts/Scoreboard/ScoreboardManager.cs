using UnityEngine;
using System.Collections;

public class ScoreboardManager : MonoBehaviour {

  private static ScoreboardManager instance;

  public static ScoreboardManager GetInstance() {
    return instance;
  }

  void Awake() {
    if (instance != null && instance != this) {
      Destroy(this.gameObject);
      return;
    }
    instance = this;
  }

  private static string scoreboardText;
  public static string ScoreboardText {
    get {
      return scoreboardText;
    }
  }

  private static bool isGettingScores;
  public static bool IsGettingScores {
    get {
      return isGettingScores;
    }
  }

  private static string secretKey = "tobygameac";

  private static string Md5Sum(string stringToEncrypt) {
    System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
    byte[] bytes = ue.GetBytes(stringToEncrypt);

    // encrypt bytes
    System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
    byte[] hashBytes = md5.ComputeHash(bytes);

    // Convert the encrypted bytes back to a string (base 16)
    string hashString = "";

    for (int i = 0; i < hashBytes.Length; i++) {
      hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
    }
    return hashString.PadLeft(32, '0');
  }

  public static IEnumerator GetScores(GameConstants.GameMode gameMode) {

    isGettingScores = true;

    string scoreboardUrl;
    if (gameMode == GameConstants.GameMode.SURVIVAL_NORMAL) {
      scoreboardUrl = "http://nanotd.tobygameac.com/SurvivalNormalScoreboard.php";
    } else {
      yield break;
    }

    WWW scoreboardWWW = new WWW(scoreboardUrl);
    yield return scoreboardWWW;

    scoreboardText = scoreboardWWW.text;

    isGettingScores = false;
  }

  public static IEnumerator PostScore(GameConstants.GameMode gameMode, string name, int score, bool loadScoreboard) {

    name = name.Trim();
    name = name.Replace("小雞雞", "帥哥");

    string postScoreUrl;
    if (gameMode == GameConstants.GameMode.SURVIVAL_NORMAL) {
      postScoreUrl = "http://nanotd.tobygameac.com/postSurvivalNormalScore.php";
    } else {
      yield break;
    }

    string hash = Md5Sum(name + score.ToString() + secretKey);

    string realPostScoreUrl = postScoreUrl + "?name=" + WWW.EscapeURL(name) + "&score=" + score.ToString() + "&hash=" + hash;

    WWW hs_post = new WWW(realPostScoreUrl);
    yield return hs_post;
    if (hs_post.error != null) {
      print("There was an error posting the score: " + hs_post.error);
    }

    if (loadScoreboard) {
      Application.LoadLevel("Scoreboard");
    }
  }
}
