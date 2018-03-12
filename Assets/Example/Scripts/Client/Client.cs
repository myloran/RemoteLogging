using Okwy.Logging;
using Okwy.Logging.Appenders;
using Okwy.Logging.Formatters;
using UniRx;
using UnityEngine;

public class Client : MonoBehaviour {
  readonly Okwy.Logging.Logger log = Okwy.Logging.MainLog.GetLogger(typeof(Client).Name);
  public ClientView view;

  WebRtcAppender webRtc;
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
      webRtc = new WebRtcAppender();
      MainLog.AddAppender(GetWebRtcAppender());
      webRtc.Connect("log");
    } else {
      MainLog.RemoveAppender(GetWebRtcAppender());
      webRtc.Disconnect();
    }

    view.Toggle(isInit);
  }

  LogDelegate GetWebRtcAppender() {
    return ((logger, logLevel, message) => {
      webRtc.Send(new FullFormatter()
        .FormatMessage(logger, logLevel, message));
    });
  }
}