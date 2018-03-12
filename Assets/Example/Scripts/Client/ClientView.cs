using UnityEngine;
using UnityEngine.UI;

public class ClientView : MonoBehaviour {
  readonly Okwy.Logging.Logger log = Okwy.Logging.MainLog.GetLogger(typeof(Client).Name);
  public InputField messageInput;

  public Button
    startButton,
    sendButton;

  public void Toggle(bool isInit) {
    sendButton.interactable = isInit;
    messageInput.interactable = isInit;

    if (isInit)
      startButton.GetComponentInChildren<Text>().text = "Stop client";
    else
      startButton.GetComponentInChildren<Text>().text = "Start client";
  }
}