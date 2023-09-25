using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Michsky.UI.Dark
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Animator))]
    public class SwitchManager : LoadableSettingComponent
    {
        // Events
        public UnityEvent onEvents;
        public UnityEvent offEvents;

        // Settings
        private bool isOn = false;
        public bool invokeAtStart = false;

        // Resources
        public Animator switchAnimator;
        public Button switchButton;

        void Awake()
        {
            if (switchAnimator == null)  switchAnimator = GetComponent<Animator>();
            if (switchButton == null)
            {
                switchButton = GetComponent<Button>();
                switchButton.onClick.AddListener(AnimateSwitch);
            }
        }

        public override void LoadComponent(object value)
        {
            if (switchAnimator == null) switchAnimator = GetComponent<Animator>();
            Debug.Log("LoadComponent");
            isOn = (bool)value;
            if (isOn == true) switchAnimator.Play("Switch On");
            else switchAnimator.Play("Switch Off");
        }

        void OnEnable()
        {
            if (switchAnimator == null) switchAnimator = GetComponent<Animator>();

            if (isOn == true) switchAnimator.Play("Switch On");
            else switchAnimator.Play("Switch Off");
        }

        public void AnimateSwitch()
        {
            Debug.Log("AnimateSwitch");
            if (isOn == true)
            {
                switchAnimator.Play("Switch Off");
                isOn = false;
                offEvents?.Invoke();
            }
            else
            {
                switchAnimator.Play("Switch On");
                isOn = true;
                onEvents?.Invoke();
            }
        }
    }
}