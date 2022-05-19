using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Network
{
    public class LogPresenter:MonoBehaviour
    {
        [SerializeField] private RectTransform _textTransform;
        [SerializeField] private RectTransform _contentTransform;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Scrollbar _scrollbar;

        public void Log(string log)
        {
            Debug.Log(log);
            _text.text += log;
            var size = _contentTransform.sizeDelta;
            size.y = _textTransform.sizeDelta.y+100;
            _contentTransform.sizeDelta = size;
            _scrollbar.value = 0;
        }
    }
}