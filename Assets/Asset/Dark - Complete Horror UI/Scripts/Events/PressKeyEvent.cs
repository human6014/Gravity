using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Michsky.UI.Dark
{
    public class PressKeyEvent : MonoBehaviour
    {
        // Settings
        public InputAction hotkey;

        // Events
        public UnityEvent onPressEvent;

        private bool isDisable;

        public void DisableHotKey(bool isDisable)
        {
            this.isDisable = isDisable;
        }

        void Start()
        {
            hotkey.Enable();
        }

        void Update()
        {
            if (hotkey.triggered && !isDisable)
            {
                onPressEvent.Invoke();
                Debug.Log("PressHotKey");
            }
        }
    }
}