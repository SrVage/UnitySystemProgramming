using UnityEngine;

namespace Code
{
    public class Control:MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                _unit.ReceiveHealth();
        }
    }
}