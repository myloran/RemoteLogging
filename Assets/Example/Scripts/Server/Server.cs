using Byn.Net;
using System.Text;
using UniRx;
using UnityEngine;

public class Server : MonoBehaviour {
  public ServerView view;

  RemoteServer server;
  bool isInit;

  public void Init() {    
    view.startButton.Sub(_ => ToggleInit());
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

        string message = Encoding.UTF8.GetString(
          _.MessageData.Buffer,
          0,
          _.MessageData.ContentLength);

        view.SpawnText(message);        
      }).AddTo(this);
    } else {
      server.Disconnect();
    }

    view.Toggle(isInit);
  }
}