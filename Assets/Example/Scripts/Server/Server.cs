using Byn.Net;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Server : MonoBehaviour {
  public ServerView view;

  RemoteLogger logger;
  CompositeDisposable disposable;
  bool isInit;

  public void Init() {    
    view.startButton.Sub(_ => ToggleInit());
  }

  void ToggleInit() {
    isInit = !isInit;

    if (isInit) {
      logger = new RemoteLogger();
      logger.StartServer("log");

      disposable = new CompositeDisposable();
      logger.remote.OnEvent.Subscribe(_ => {
        if (_.Type != NetEventType.ReliableMessageReceived
          && _.Type != NetEventType.UnreliableMessageReceived
        ) return;

        string message = Encoding.UTF8.GetString(
          _.MessageData.Buffer,
          0,
          _.MessageData.ContentLength);

        view.SpawnText(_.ConnectionId + ". " + message);        
      }).AddTo(disposable);

      logger.remote.OnEvent
        .Where(_ => _.Type == NetEventType.ServerInitialized)
        .Subscribe(_ => view.startButton
          .GetComponentInChildren<Text>().text = "Stop server")
        .AddTo(disposable);
    } else {
      logger.Disconnect();
      disposable.Dispose();
    }

    view.Toggle(isInit);
  }
}