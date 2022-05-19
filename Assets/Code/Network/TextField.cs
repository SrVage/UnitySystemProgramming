using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Network
{
    public class TextField:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Scrollbar _scrollbar;
        private List<string> _messages = new List<string>();

        private void Start() => 
            _scrollbar.onValueChanged.AddListener((float value)=>UpdateText());

        public void ReceiveMessage(object message)
        {
            _messages.Add(message.ToString());
            float value = (_messages.Count - 1) * _scrollbar.value;
            _scrollbar.value = Mathf.Clamp(value, 0, 1);
            UpdateText();
        }

        private void UpdateText()
        {
            string text = "";
            int index = (int)(_messages.Count * _scrollbar.value); 
            for (int i = index; i < _messages.Count; i++)
            {
                text += _messages[i] + "\n";
            }
            _messageText.text = text;
        }
    }
}