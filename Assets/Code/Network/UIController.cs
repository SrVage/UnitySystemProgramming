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
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private LogPresenter _textField;
        [SerializeField] private Server _server;
        [SerializeField] private Client _client;

        private void Start()
        {
            _startServerButton.onClick.AddListener(() => StartServer());
            _shutDownServerButton.onClick.AddListener(() => ShutDownServer());
            _connectClientButton.onClick.AddListener(() => Connect());
            _disconnectClientButton.onClick.AddListener(() => Disconnect());
            _sendMessageButton.onClick.AddListener(() => SendMessage());
            _inputField.onEndEdit.AddListener((text) =>SendMessage());
            _client.onMessageReceive += ReceiveMessage;
        }

        private void StartServer() =>    
            _server.StartServer();
    
        private void ShutDownServer() =>    
            _server.ShutDownServer();
    
        private void Connect() =>    
            _client.Connect();    

        private void Disconnect() =>    
            _client.Disconnect();    

        private void SendMessage()
        {
            _client.SendMessage(_inputField.text);
            _inputField.text = "";
        }

        private void ReceiveMessage(object message) =>    
            _textField.Log(message.ToString());
    
    }
}
