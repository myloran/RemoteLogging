public interface IRemoteLogger {
  IRemoteLogger StartServer(string name);
  IRemoteLogger StartClient(string name, int delayMs = 5000);
  void Disconnect();
  void Send(string message);
}