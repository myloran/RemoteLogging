using Byn.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;

namespace Okwy.Logging.Appenders {
  //Save logs in file system, so that they are not lost when internet or server is offline
  public class WebRtcAppender {
    static readonly Logger log = MainLog.GetLogger(typeof(WebRtcAppender).Name);
    readonly List<OfflineLog> logs = new List<OfflineLog>();
    RemoteServer remote = new RemoteServer();

    public void Connect(string name) {
      remote.Init();
      remote.Connect(name);
      remote.OnEvent.Subscribe(async _ => {
        if (_ == NetEventType.ConnectionFailed) {
          await Task.Delay(5000);
          remote.Connect(name);
        }

        if (_ == NetEventType.NewConnection && logs.Count > 0) {
          Send(log, LogLevel.Debug, "Flush history - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
          foreach (OfflineLog item in logs) {
            Send(item.logger, item.logLevel, item.message);
          }
          Send(log, LogLevel.Debug, "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
          logs.Clear();
        }
      });
    }

    public void Start(string name) {
      remote.Init();
      remote.Create(name);
    }

    public void Disconnect() {
      remote.Disconnect();
    }

    public void Send(Logger logger, LogLevel logLevel, string message) {
      if (remote.IsConnected()) {
        remote.Send(message);
        return;
      }
      logs.Add(new OfflineLog(logger, logLevel, message));
    }

    class OfflineLog {
      public readonly Logger logger;
      public readonly LogLevel logLevel;
      public readonly string message;

      public OfflineLog(Logger logger, LogLevel logLevel, string message) {
        this.logger = logger;
        this.logLevel = logLevel;
        this.message = message;
      }
    }
  }
}