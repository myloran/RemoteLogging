using Byn.Net;
using Okwy.Logging;
using Okwy.Logging.Formatters;
using System.Text;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour {
  readonly Okwy.Logging.Logger log = Okwy.Logging.MainLog.GetLogger(typeof(Client).Name);
  public ClientView view;

  RemoteLogger logger;
  CompositeDisposable disposable;
  bool isInit;

  public void Init() {
    view.startButton.Sub(_ => ToggleInit());
    view.sendButton.Sub(_ => SendMessage());
  }

  void SendMessage() {
    if (view.messageInput.text != "") {
      log.Info("SendMessage: " + view.messageInput.text);
      view.messageInput.text = "";
    }
  }

  void ToggleInit() {
    isInit = !isInit;

    if (isInit) {
      logger = new RemoteLogger();
      MainLog.AddAppender(GetWebRtcAppender());
      logger.Connect("log");

      disposable = new CompositeDisposable();
      logger.remote.OnEvent
        .Where(_ => _.Type == NetEventType.Disconnected)
        .Subscribe(_ => {
          ToggleInit();
          ToggleInit();
        })
        .AddTo(disposable);

      logger.remote.OnEvent
        .Where(_ => _.Type == NetEventType.NewConnection)
        .Subscribe(_ => view.startButton
          .GetComponentInChildren<Text>().text = "Stop client")
        .AddTo(disposable);
    } else {
      MainLog.RemoveAppender(GetWebRtcAppender());
      logger.Disconnect();
      disposable.Dispose();
    }

    view.Toggle(isInit);
  }

  LogDelegate GetWebRtcAppender() {
    return ((logger, logLevel, message) => {
      this.logger.Send(new FullFormatter()
        .FormatMessage(logger, logLevel, message));
    });
  }
}