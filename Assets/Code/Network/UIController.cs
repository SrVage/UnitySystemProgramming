using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Network
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Button _startServerButton;
        [SerializeField] private Button _shutDownServerButton;
        [SerializeField] private Button _connectClientButton;
        [SerializeField] private Button _disconnectClientButton;
        [SerializeField] private Button _sendMessageButton;
        [SerializeField] private TMP_InputField _chatInputField;
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private LogPresenter _textField;
        [SerializeField] private Server _server;
        [SerializeField] private Client _client;

        private void Start()
        {
            _startServerButton.onClick.AddListener(StartServer);
            _shutDownServerButton.onClick.AddListener(ShutDownServer);
            _connectClientButton.onClick.AddListener(Connect);
            _disconnectClientButton.onClick.AddListener(Disconnect);
            _sendMessageButton.onClick.AddListener(SendMessage);
            _chatInputField.onEndEdit.AddListener((text) =>SendMessage());
            _client.onMessageReceive += ReceiveMessage;
            _client.onConnectReceive += SendName;
        }

        private void SendName()
        {
            _client.SendMessage(_nameInputField.text, MessageType.PlayerName);
        }

        private void StartServer() =>    
            _server.StartServer();
    
        private void ShutDownServer() =>    
            _server.ShutDownServer();
    
        private void Connect()
        {
            if (string.IsNullOrEmpty(_nameInputField.text))
            {
                _textField.Log("Please, enter player's name \n");
                return;
            }
            _client.Connect();
            //_client.SendMessage(_nameInputField.text);
        }

        private void Disconnect() =>    
            _client.Disconnect();    

        private void SendMessage()
        {
            if (string.IsNullOrEmpty(_chatInputField.text))
                return;
            _client.SendMessage(_chatInputField.text);
            _chatInputField.text = "";
        }

        private void ReceiveMessage(object message) =>    
            _textField.Log(message.ToString());
    
    }
}
