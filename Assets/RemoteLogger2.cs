using Byn.Net;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class RemoteLogger2 : IRemoteLogger {
  public IOkwyNetwork network = new OkwyNetwork();
  object Lock = new object();

  string offlineLogsPath = Path.Combine(
    Application.persistentDataPath,
    "RemoteLogging",
    "Offline.log");

  public IRemoteLogger StartServer(string name) {
    network.Init().Listen(name);

    network.OnEvent.Subscribe(_ => {
      if ((_.Type != NetEventType.ReliableMessageReceived
        && _.Type != NetEventType.UnreliableMessageReceived)
      ) return;

      string message = Encoding.UTF8.GetString(
        _.MessageData.Buffer,
        0,
        _.MessageData.ContentLength);

      Directory.CreateDirectory(Path.Combine(
        Application.persistentDataPath,
        "RemoteLogging"));

      using (var streamWriter = new StreamWriter(
        Path.Combine(
          Application.persistentDataPath,
          "RemoteLogging",
          _.ConnectionId + ".log"),
        append: true)
      ) streamWriter.WriteLine(message);

      Debug.Log($"{_.ConnectionId}: {message}");
    });
    return this;
  }

  public IRemoteLogger StartClient(
    string name,
    int delayMs = 5000
  ) {
    Directory.CreateDirectory(Path
      .GetDirectoryName(offlineLogsPath));

    network.Init().Connect(name);

    network.OnEvent.Subscribe(async _ => {
      if (_.Type == NetEventType.ConnectionFailed) {
        await Task.Delay(delayMs);

        network.Connect(name);
      }

      if (_.Type == NetEventType.NewConnection
        && File.Exists(offlineLogsPath)
      ) {
        foreach (var message in LoadMessages())
          Send(message);
      }
    });
    return this;
  }

  public void Disconnect() {
    network.Dispose();
  }

  public void Send(string message) {
    if (network.IsConnected()) {
      network.Send(message);
      return;
    }
    Save(message);
  }

  void Save(string message) {
    var Lock = this.Lock;

    lock (Lock) {
      using (var streamWriter = new StreamWriter(
        offlineLogsPath,
        append: true)
      ) streamWriter.WriteLine(message);
    }
  }

  List<string> LoadMessages() {
    var Lock = this.Lock;
    var list = new List<string>();

    lock (Lock) {
      using (var reader = new StreamReader(offlineLogsPath)) {
        while (!reader.EndOfStream)
          list.Add(reader.ReadLine());
      }
      File.WriteAllText(offlineLogsPath, "");
    }
    return list;
  }
}