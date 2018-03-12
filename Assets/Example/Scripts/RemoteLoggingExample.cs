using Byn.Net;
using UnityEngine;
public class RemoteLoggingExample : MonoBehaviour {
  public Server server;
  public Client client;

  public void Start() {
    var factory = WebRtcNetworkFactory.Instance;
    server.Init();
    client.Init();
  }
}