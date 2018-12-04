using System.Text;
using Byn.Net;
using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;

public class OkwyNetwork : IOkwyNetwork, IDisposable {
  #region Server config
  string signalingUrl = "wss://because-why-not.com:12777/chatapp";
  string iceServer = "stun:because-why-not.com:12779";
  string iceServerUser = "";
  string iceServerPassword = "";
  string iceServer2 = "stun:stun.l.google.com:19302";
  #endregion
  public IObservable<NetworkEvent> OnEvent => onEvent;
  public List<ConnectionId> Connections { get; } = new List<ConnectionId>();

  readonly Okwy.Logging.Logger log = Okwy.Logging.MainLog.GetLogger(typeof(OkwyNetwork).Name);
  Subject<NetworkEvent> onEvent = new Subject<NetworkEvent>();
  IBasicNetwork network;
  string name;

  public IOkwyNetwork Init() { //why WebRtcNetworkFactory.Instance must be called at start?
    network = WebRtcNetworkFactory.Instance.CreateDefault( //why it tries to connect on destroy?
      signalingUrl,
      new IceServer[] {
        new IceServer(
          iceServer,
          iceServerUser,
          iceServerPassword),
        new IceServer(iceServer2)
      });

    Observable
      .EveryFixedUpdate()
      .Subscribe(_ => HandleEvents());
    return this;
  }

  void HandleEvents() {
    network?.Update();
    NetworkEvent Event;

    while (network != null && network.Dequeue(out Event)) {
      if (Event.Type == NetEventType.ServerInitialized) {
        Debug.Log("ServerInitialized.name: " + name);

      } else if (Event.Type == NetEventType.ServerInitFailed) {
        Debug.Log("ServerInitFailed.name: " + name);

      } else if (Event.Type == NetEventType.ServerClosed) {
        Debug.Log("ServerClosed.name: " + name);

      } else if (Event.Type == NetEventType.NewConnection) {
        Debug.Log("NewConnection.name: " + name + ", id: " + Event.ConnectionId);
        Connections.Add(Event.ConnectionId);

      } else if (Event.Type == NetEventType.ConnectionFailed) {
        Debug.Log("ConnectionFailed.name: " + name + ", id: " + Event.ConnectionId);

      } else if (Event.Type == NetEventType.Disconnected) {
        Debug.Log("Disconnected.name: " + name + ", id: " + Event.ConnectionId);
        Connections.Remove(Event.ConnectionId);
      }
      onEvent.OnNext(Event);
    }

    network?.Flush();
  }

  public IOkwyNetwork Listen(string name) {
    this.name = name;
    network?.StartServer(name);
    return this;
  }

  public IOkwyNetwork Connect(string name) {
    this.name = name;
    network?.Connect(name);
    return this;
  }

  public void Send(string message, bool isReliable = true) {
    if (IsConnected()) {
      byte[] data = Encoding.UTF8.GetBytes(message);

      foreach (ConnectionId id in Connections)
        network.SendData(id, data, 0, data.Length, isReliable);
    }
  }

  public void Send(byte[] message, bool isReliable = true) {
    if (IsConnected()) {
      foreach (ConnectionId id in Connections)
        network.SendData(id, message, 0, message.Length, isReliable);
    }
  }

  public void Send(ConnectionId id, byte[] message, bool isReliable = true) {
    if (IsConnected())
      network.SendData(id, message, 0, message.Length, isReliable);
  }

  public bool IsConnected() {
    return network != null && Connections.Count > 0;
  }

  public void Dispose() {
    network?.Dispose();
    network = null;
  }
}