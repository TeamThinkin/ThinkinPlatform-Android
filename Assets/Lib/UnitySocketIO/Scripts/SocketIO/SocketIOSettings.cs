using UnityEngine;
using System.Collections;

namespace UnitySocketIO.SocketIO {
	[System.Serializable]
	public class SocketIOSettings {

		public bool autoConnect;
		public string url;
		public int port;

		public bool sslEnabled;

		public int reconnectTimeInSeconds;

		public int timeToDropAck;
		
		public int pingTimeout;
		public int pingInterval;

	}
}