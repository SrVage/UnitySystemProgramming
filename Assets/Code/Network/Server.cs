using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Network
{
    public class Server:MonoBehaviour
    {
        private const int MAXCONNECTION = 10;
        private int _port = 5805;
        private int _reliableChannel;
        private int _hostID;
        private bool _isStarted;
        private byte _error;
        private List<int> _connectionIDs = new List<int>();
        private Dictionary<int, string> _usersName = new Dictionary<int, string>();

        public void StartServer()
        {
            NetworkTransport.Init();
            ConnectionConfig connectionConfig = new ConnectionConfig();
            _reliableChannel = connectionConfig.AddChannel(QosType.Reliable);
            HostTopology topology = new HostTopology(connectionConfig, MAXCONNECTION);
            _hostID = NetworkTransport.AddHost(topology, _port);
            _isStarted = true;
        }

        public void ShutDownServer()
        {
            if (!_isStarted) 
                return;
            NetworkTransport.RemoveHost(_hostID);
            NetworkTransport.Shutdown();
            _isStarted = false;
        }
        
        public void SendMessage(string message, int connectionID) 
        {
            byte[] buffer = Encoding.Unicode.GetBytes(message);
            NetworkTransport.Send(_hostID, connectionID, _reliableChannel, buffer,
                message.Length * sizeof(char), out _error);
            if ((NetworkError)_error != NetworkError.Ok) 
                Debug.Log((NetworkError)_error); 
        }
        
        public void SendMessageToAll(string message)
        {
            foreach (var userName in _usersName) 
                SendMessage(message, userName.Key);
        }

        private void Update()
        {
            if (!_isStarted) 
                return;
            int recHostId;
            int connectionId;
            int channelId;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            string messageString = String.Empty;
            NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId,
                out channelId, recBuffer, bufferSize, out dataSize, out _error);
            while (recData != NetworkEventType.Nothing)
            {
                switch (recData)
                {
                    case NetworkEventType.DataEvent:
                        if (int.TryParse(Encoding.Unicode.GetString(recBuffer, 0, 2), out int type))
                        {
                            Debug.Log(type);
                            string message = Encoding.Unicode.GetString(recBuffer, 2, dataSize-2);
                            Debug.Log(message);
                            if (type == 0)
                            {
                                _usersName[connectionId] = message;
                                Debug.Log(messageString);
                            }

                            if (type == 1)
                            {
                                messageString = $"{_usersName[connectionId]}: {message} \n";
                                SendMessageToAll(messageString); 
                                Debug.Log(messageString);
                            }
                        }
                        
                        /*string message = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                        if (string.IsNullOrEmpty(_usersName[connectionId]))
                        {
                            _usersName[connectionId] = message;
                        }
                        else
                        {
                            messageString = $"{_usersName[connectionId]}: {message} \n";
                            SendMessageToAll(messageString); 
                            Debug.Log(messageString);
                        }*/
                        break;
                    case NetworkEventType.ConnectEvent:
                        //_connectionIDs.Add(connectionId);
                        _usersName.Add(connectionId, String.Empty);
                        messageString = $"Player {connectionId} has connected.";
                        SendMessageToAll(messageString);
                        Debug.Log(messageString);
                        break;
                    case NetworkEventType.DisconnectEvent:
                        _usersName.Remove(connectionId);
                        messageString = $"Player {connectionId} has disconnected.";
                        SendMessageToAll(messageString);
                        Debug.Log(messageString);
                        break;
                    case NetworkEventType.Nothing:
                        break;
                    case NetworkEventType.BroadcastEvent:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out _error);
            }

        }
    }
}