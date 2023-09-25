using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Michsky.UI.Dark
{
    [RequireComponent(typeof(Slider))]
    public class SliderManager : LoadableSettingComponent, IPointerEnterHandler, IPointerExitHandler
    {
        // Resources
        public Animator sliderAnimator;
        public Slider mainSlider;
        public TextMeshProUGUI valueText;
        public TextMeshProUGUI popupValueText;

        // Settings
        public bool usePercent = false;
        public bool showValue = true;
        public bool showPopupValue = true;
        public bool useRoundValue = false;
        public float valueMultiplier = 1;

        private void Awake()
        {
            if (mainSlider == null) mainSlider = GetComponent<Slider>();
        }

        void Start()
        {
            mainSlider.onValueChanged.AddListener(delegate
            {
                UpdateUI();
            });

            UpdateUI();
        }

        public override void LoadComponent(object value)
        {
            if (mainSlider == null) mainSlider = GetComponent<Slider>();
            mainSlider.value = (float)value;
            UpdateUI();
        }

        public void UpdateUI()
        {
            if (useRoundValue == true)
            {
                if (usePercent == true && valueText != null)
                    valueText.text = Mathf.Round(mainSlider.value * valueMultiplier).ToString() + "%";
                else if (usePercent == false && valueText != null)
                    valueText.text = Mathf.Round(mainSlider.value * valueMultiplier).ToString();
            }

            else
            {
                if (usePercent == true && valueText != null)
                    valueText.text = mainSlider.value.ToString("F1") + "%";
                else if (usePercent == false && valueText != null)
                    valueText.text = mainSlider.value.ToString("F1");
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (showPopupValue == true)
                sliderAnimator.Play("Value In");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (showPopupValue == true)
                sliderAnimator.Play("Value Out");
        }
    }
}