using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject MenuPanel;
        private bool isMenuPanelActive;
        public void InputMenuPanelKey()
        {
            isMenuPanelActive = !isMenuPanelActive;
            MenuPanel.SetActive(isMenuPanelActive);

            if (isMenuPanelActive)
            {
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void OnClickedExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
