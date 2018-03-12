using Byn.Net;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Server : MonoBehaviour {
  public ScrollRect logsScroll;
  public Button startButton;
  public Text logPrefab;

  RemoteServer server;
  bool isInit;

  public void Init() {    
    startButton
      .OnClickAsObservable()
      .Subscribe(_ => ToggleInit())
      .AddTo(startButton);
  }

  void ToggleInit() {
    isInit = !isInit;

    if (isInit) {
      server = new RemoteServer();
      server.Create("log");

      server.OnEvent.Subscribe(_ => {
        if (_.Type != NetEventType.ReliableMessageReceived
          && _.Type != NetEventType.UnreliableMessageReceived
        ) return;

        var data = _.MessageData;

        string message = Encoding.UTF8.GetString(
          data.Buffer,
          0,
          data.ContentLength);

        var log = Instantiate(logPrefab, logsScroll.content);
        log.text = message;
      }).AddTo(startButton);

      startButton.GetComponentInChildren<Text>().text = "Stop server";
    } else {
      server.Disconnect();

      startButton.GetComponentInChildren<Text>().text = "Start server";
    }
  }
}