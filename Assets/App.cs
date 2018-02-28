public static class App {
  public static bool IsInit { get; private set; }

  public static void Init() {
    IsInit = true;
    RemoteServer server = new RemoteServer();
    server.Init();
    server.Create("log");
  }
}