using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class DamageDisplayer : MonoBehaviour
    {
        [SerializeField] private Image m_BloodScreenImage;
        [SerializeField] private Image m_HitDirectionImage;
        [SerializeField] private RectTransform m_Indicator;

        [SerializeField] private float m_HitDirectionTimer = 1;
        [SerializeField][Range(0,255)] private int m_MaxAlpha;

        private Coroutine m_HitDirectionCoroutine;
        private Transform m_MainCamera;
        private Color32 color;
        private float m_HPToScreenAlpha;

        public void Init(int maxHP, int amountToRealConst)
        {
            m_MainCamera = Camera.main.transform;
            color = m_BloodScreenImage.color;
            m_HPToScreenAlpha = amountToRealConst / maxHP * m_MaxAlpha;
            m_Indicator.gameObject.SetActive(false);
        }

        public void ReCalcScreenAlpha(int maxHP, int amountToRealConst)
            => m_HPToScreenAlpha = amountToRealConst / maxHP * m_MaxAlpha;
        

        public void DisplayBloodScreen(float HPAmount)
        {
            float healthRatio = HPAmount * m_HPToScreenAlpha;
            int alpha = (int)(m_MaxAlpha - healthRatio);
            color.a = (byte)alpha;
            m_BloodScreenImage.color = color;
        }

        public void DisplayHitDirection(Transform target)
        {
            if (m_HitDirectionCoroutine != null)
            {
                StopCoroutine(m_HitDirectionCoroutine);
                StopDisplay();
            }
            m_HitDirectionCoroutine = StartCoroutine(CDisplayHitDirection(target));
        }

        private IEnumerator CDisplayHitDirection(Transform target)
        {
            Vector3 direction;
            Vector3 northDirection;
            Quaternion m_TargetRotation;
            float m_timer = 0;
            m_Indicator.gameObject.SetActive(true);
            while (m_timer < m_HitDirectionTimer)
            {
                direction = (m_MainCamera.position - target.position).normalized;

                m_TargetRotation = Quaternion.LookRotation(direction);
                m_TargetRotation.z = -m_TargetRotation.y;
                m_TargetRotation.x = 0;
                m_TargetRotation.y = 0;

                northDirection = new Vector3(0, 0, m_MainCamera.eulerAngles.y);
                m_Indicator.localRotation = m_TargetRotation * Quaternion.Euler(northDirection);

                m_timer += Time.deltaTime;
                yield return null;
            }
            StopDisplay();
        }

        private void StopDisplay()
        {
            m_Indicator.gameObject.SetActive(false);
        }
    }
}
