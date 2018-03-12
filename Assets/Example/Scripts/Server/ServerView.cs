using UnityEngine;
using UnityEngine.UI;

public class ServerView : MonoBehaviour {
  public ScrollRect logsScroll;
  public Button startButton;
  public Text logPrefab;

  public void Toggle(bool isInit) {
    if (isInit)
      startButton.GetComponentInChildren<Text>().text = "Starting server...";
    else
      startButton.GetComponentInChildren<Text>().text = "Start server";
  }

  internal void SpawnText(string message) {
    var log = Instantiate(logPrefab, logsScroll.content);
    log.text = message;
  }
}