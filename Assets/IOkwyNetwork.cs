using Byn.Net;
using System;
using System.Collections.Generic;

public interface IOkwyNetwork {
  IObservable<NetworkEvent> OnEvent { get; }
  List<ConnectionId> Connections { get; }
  IOkwyNetwork Init();
  IOkwyNetwork Listen(string name);
  IOkwyNetwork Connect(string name);
  void Send(string message, bool isReliable = true);
  void Send(byte[] message, bool isReliable = true);
  void Send(ConnectionId id, byte[] message, bool isReliable = true);
  bool IsConnected();
  void Dispose();
}