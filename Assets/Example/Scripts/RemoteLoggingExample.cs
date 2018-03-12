using Byn.Net;
using UnityEngine;
public class RemoteLoggingExample : MonoBehaviour {
  public Server server;
  public Client client;

  public void Start() {
    var factory = WebRtcNetworkFactory.Instance; //we need to call it on start to create instance in time
    server.Init();
    client.Init();
  }
}