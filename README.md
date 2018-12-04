# RemoteLogging
Remote logging using signal server and webrtc protocol

	Usage:
RemoteLogging.ConnectTo("log");
RemoteLogging.StartServer("log");
	
var logger = new RemoteLogger();
logger.StartServer("log");

var logger = new RemoteLogger();
logger.StartClient("log");