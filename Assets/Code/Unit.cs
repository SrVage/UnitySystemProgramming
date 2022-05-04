using System.Collections;
using UnityEngine;

namespace Code
{
    public class Unit:MonoBehaviour
    {
        [SerializeField] private int _health;
        [SerializeField] private int _maxHealth;
        [SerializeField] private int _receiveHealth;
        [SerializeField] private float _receiveTime;
        [SerializeField] private float _rechargeTime;
        private float _timer;
        private Coroutine _receiveHealthCoroutine;

        public void ReceiveHealth()
        {
            if (_receiveHealthCoroutine!=null)
                StopCoroutine(_receiveHealthCoroutine);
            if (_health < _maxHealth)
                _receiveHealthCoroutine = StartCoroutine(nameof(ReceiveHealthCoroutine));
        }

        private IEnumerator ReceiveHealthCoroutine()
        {
            _timer = _receiveTime;
            while (_timer>0)
            {
                _health += _receiveHealth;
                if (_health >= _maxHealth)
                {
                    _health = _maxHealth;
                    yield break;
                }
                _timer -= _rechargeTime;
                yield return new WaitForSeconds(_rechargeTime);
            }
        }
    }
}