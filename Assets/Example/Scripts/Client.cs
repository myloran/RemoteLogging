using Okwy.Logging;
using Okwy.Logging.Appenders;
using Okwy.Logging.Formatters;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour {
  readonly Okwy.Logging.Logger log = Okwy.Logging.MainLog.GetLogger(typeof(Client).Name);
  public InputField messageInput;

  public Button
    startButton,
    sendButton;

  WebRtcAppender webRtc;
  bool isInit;

  public void Init() {
    startButton
      .OnClickAsObservable()
      .Subscribe(_ => ToggleInit())
      .AddTo(startButton);

    sendButton
      .OnClickAsObservable()
      .Subscribe(_ => SendMessage())
      .AddTo(startButton);
  }

  void SendMessage() {
    if (messageInput.text != "") {
      log.Info("Send.message: " + messageInput.text);
      messageInput.text = "";
    }
  }

  void ToggleInit() {
    isInit = !isInit;
    sendButton.interactable = isInit;
    messageInput.interactable = isInit;

    if (isInit) {
      MainLog.AddAppender(GetWebRtcAppender());
      webRtc = new WebRtcAppender();
      webRtc.Connect("log");
      startButton.GetComponentInChildren<Text>().text = "Stop client";
    } else {
      MainLog.RemoveAppender(GetWebRtcAppender());
      webRtc.Disconnect();
      startButton.GetComponentInChildren<Text>().text = "Start client";
    }
  }

  LogDelegate GetWebRtcAppender() {
    return ((logger, logLevel, message) => {
      webRtc.Send(new FullFormatter()
        .FormatMessage(logger, logLevel, message));
    });
  }
}