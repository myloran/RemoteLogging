public static class App {
  public static bool IsInit { get; private set; }

  static IRemoteLogger server;

  public static void Init() {
    IsInit = true;
    server = new RemoteLogger2();
    server.StartServer("devsync_log");
  }
}