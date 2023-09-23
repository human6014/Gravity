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
        public bool isOn = true;
        public bool invokeAtStart = true;

        // Resources
        public Animator switchAnimator;
        public Button switchButton;

        void Awake()
        {
            if (switchAnimator == null) { switchAnimator = gameObject.GetComponent<Animator>(); }
            if (switchButton == null)
            {
                switchButton = gameObject.GetComponent<Button>();
                switchButton.onClick.AddListener(AnimateSwitch);
            }
        }

        public override void LoadComponent(object value)
        {
            isOn = (bool)value;
            if (isOn == true)
            {
                switchAnimator.Play("Switch On");
                isOn = true;
            }
            else
            {
                switchAnimator.Play("Switch Off");
                isOn = false;
            }
        }

        //void Start()
        //{
        //    if (isOn == true)
        //    {
        //        switchAnimator.Play("Switch On");
        //        isOn = true;
        //    }
        //    else
        //    {
        //        switchAnimator.Play("Switch Off");
        //        isOn = false;
        //    }

        //    if (invokeAtStart == true && isOn == true)
        //        onEvents?.Invoke();
        //    else if (invokeAtStart == true && isOn == false)
        //        offEvents?.Invoke();
        //}

        //void OnEnable()
        //{
        //    if (switchAnimator == null)
        //        return;
        //    if (isOn == true)
        //    {
        //        switchAnimator.Play("Switch On");
        //        isOn = true;
        //    }
        //    else
        //    {
        //        switchAnimator.Play("Switch Off");
        //        isOn = false;
        //    }
        //}

        public void AnimateSwitch()
        {
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