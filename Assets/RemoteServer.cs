﻿using System.Text;
using Byn.Net;
using System.Collections.Generic;
using System;
using UniRx;
using System.IO;
using UnityEngine;

public class RemoteServer : IDisposable {
  public string signalingUrl = "wss://because-why-not.com:12777/chatapp";
  public string iceServer = "stun:because-why-not.com:12779";
  public string iceServerUser = "";
  public string iceServerPassword = "";
  public string iceServer2 = "stun:stun.l.google.com:19302";
  public IObservable<NetworkEvent> OnEvent => onEvent;
  public bool IsServer => isServer;

  List<ConnectionId> connections = new List<ConnectionId>();
  IBasicNetwork network;
  const int max_code_length = 256;
  bool isServer;
  Subject<NetworkEvent> onEvent = new Subject<NetworkEvent>();
  bool isInit;

  void Init() { //why WebRtcNetworkFactory.Instance must be called at start?
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
  }

  public void Disconnect() {
    isServer = false;
    connections = new List<ConnectionId>();
    Cleanup();
  }

  void Cleanup() {
    network?.Dispose();
    network = null;
  }

  void HandleEvents() {
    if (network == null) return;

    network.Update();
    NetworkEvent Event;

    while (network != null && network.Dequeue(out Event)) {
      if (Event.Type == NetEventType.ServerInitialized) {
        isServer = true;

      } else if (Event.Type == NetEventType.ServerInitFailed) {
        Disconnect();

      } else if (Event.Type == NetEventType.ServerClosed) {
        isServer = false;

      } else if (Event.Type == NetEventType.NewConnection) {
        connections.Add(Event.ConnectionId);

      } else if (Event.Type == NetEventType.ConnectionFailed) {
        Disconnect();

      } else if (Event.Type == NetEventType.Disconnected) {
        connections.Remove(Event.ConnectionId);

        if (isServer == false) Disconnect();
      } 
      onEvent.OnNext(Event);
    }

    if (network != null) network.Flush();
  }

  public void Create(string name) {
    Init();
    network.StartServer(name);
  }

  public void Connect(string name) {
    Init();
    network.Connect(name);
  }

  public void Send(string message, bool isReliable = true) {
    if (IsConnected()) {
      byte[] data = Encoding.UTF8.GetBytes(message);

      foreach (ConnectionId id in connections)
        network.SendData(id, data, 0, data.Length, isReliable);
    }
  }

  public bool IsConnected() {
    return network != null && connections.Count > 0;
  }

  public void Dispose() {
    if (network != null) Cleanup();
  }
}