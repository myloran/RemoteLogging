using Byn.Net;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Okwy.Logging.Appenders {
  //Save logs in file system, so that they are not lost when internet or server is offline
  public class WebRtcAppender {
    RemoteServer remote = new RemoteServer();
    object Lock = new object();

    string offlineLogsPath = Path.Combine(
      Application.persistentDataPath,
      "RemoteLogging",
      "Offline.log");

    public void Connect(
      string name, 
      int delayMs = 5000
  ) {
      Directory.CreateDirectory(Path
        .GetDirectoryName(offlineLogsPath));

      remote.Connect(name);

      remote.OnEvent.Subscribe(async _ => {
        if (_.Type == NetEventType.ConnectionFailed) {
          await Task.Delay(delayMs);

          remote.Connect(name);
        }

        if (_.Type == NetEventType.NewConnection
          && File.Exists(offlineLogsPath)
      ) {
          foreach (var message in LoadMessages())
            Send(message);
        }
      });
    }

    public void Disconnect() {
      remote.Disconnect();
    }

    public void Send(string message) {
      if (remote.IsConnected()) {
        remote.Send(message);
        return;
      }
      Save(message);
    }

    void Save(string message) {
      var Lock = this.Lock;

      lock (Lock) {
        using (StreamWriter streamWriter = new StreamWriter(
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
}