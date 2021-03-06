using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Network
{
    public class Client:MonoBehaviour
    {
        public delegate void OnMessageReceive(object message);
        public event OnMessageReceive onMessageReceive;
        private const int MAXCONNECTION = 10;
        private int _port = 0;
        private int _serverPort = 5805; 
        private int _hostID;
        private int _reliableChannel;
        private int _connectionID;
        private bool _isConnected = false;
        private byte _error;
        
        public void Connect() 
        {
            NetworkTransport.Init();
            ConnectionConfig connectionConfig = new ConnectionConfig();
            _reliableChannel = connectionConfig.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(connectionConfig, MAXCONNECTION);
            _hostID = NetworkTransport.AddHost(topology, _port);
            _connectionID = NetworkTransport.Connect(_hostID, "127.0.0.1", _serverPort,
                0, out _error);
            if ((NetworkError)_error == NetworkError.Ok) _isConnected = true;
            else
                Debug.Log((NetworkError)_error); 
        }
        
        public void Disconnect()
        {
            if (!_isConnected)
                return;
            NetworkTransport.Disconnect(_hostID, _connectionID, out _error);
            _isConnected = false;
        }

        private void Update()
        {
            if (!_isConnected)
                return;
            int recHostID;
            int connectionID;
            int channelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            string messageString = String.Empty;
            NetworkEventType recData = NetworkTransport.Receive(out recHostID, out connectionID, 
                out channelID, recBuffer, bufferSize, out dataSize, out _error);
            while (recData != NetworkEventType.Nothing)
            {
                switch (recData)
                {
                    case NetworkEventType.DataEvent:
                        messageString = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                        onMessageReceive?.Invoke(messageString);
                        Debug.Log(messageString);
                        break;
                    case NetworkEventType.ConnectEvent:
                        messageString = "You have been connected to server.";
                        onMessageReceive?.Invoke(messageString); 
                        Debug.Log(messageString);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        _isConnected = false;
                        messageString = "You have been disconnected from server.";
                        onMessageReceive?.Invoke(messageString); 
                        Debug.Log(messageString);
                        break;
                    case NetworkEventType.Nothing:
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                recData = NetworkTransport.Receive(out recHostID, out connectionID, out channelID, 
                    recBuffer, bufferSize, out dataSize, out _error);
            }
        }
        
        public void SendMessage(string message) 
        {
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(_hostID, _connectionID, _reliableChannel, buffer, 
                message.Length * sizeof(char), out _error);
            if ((NetworkError)_error != NetworkError.Ok) 
                Debug.Log((NetworkError)_error); 
        }
    }
}