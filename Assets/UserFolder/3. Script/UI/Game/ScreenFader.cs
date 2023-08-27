using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;


namespace UI.Game
{
    public class ScreenFader : MonoBehaviour
    {
        private Image m_ScreenImage;
        [SerializeField] private float m_FadeInTime = 2;
        private void Awake() => m_ScreenImage = GetComponent<Image>();

        private void Start()
        {
            Color color = m_ScreenImage.color;
            color.a = 1;
            m_ScreenImage.color = color;

            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            float currentTime = 0;
            float t;
            Color cureentColor = m_ScreenImage.color;
            Color newColor = m_ScreenImage.color;
            newColor.a = 0;
            while (currentTime < m_FadeInTime)
            {
                currentTime += Time.deltaTime;

                t = currentTime / m_FadeInTime;
                m_ScreenImage.color = Color.Lerp(cureentColor, newColor, t);

                yield return t;
            }
        }
    }
}
