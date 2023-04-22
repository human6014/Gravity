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

        [SerializeField] private float m_MaxTimer;

        [SerializeField] private Transform test;

        private float m_timer; 
        private Transform m_MainCamera;
        private Quaternion m_TargetRotation;
        private Vector3 m_TargetPosition;
        

        private void Awake() => m_MainCamera = Camera.main.transform;
        
        public void Start()
        {
            DisplayHitDirection(test);
        }

        public void DisplayBloodScreen(float hpAmount)
        {

        }

        public void DisplayHitDirection(Transform target)
        {
            StartCoroutine(CDisplayHitDirection(target));
        }

        private IEnumerator CDisplayHitDirection(Transform target)
        {
            Vector3 direction;
            Vector3 northDirection;
            while (m_timer < m_MaxTimer)
            {
                m_TargetRotation = target.rotation;
                m_TargetPosition = target.position;
                direction = (m_MainCamera.position - m_TargetPosition).normalized;

                m_TargetRotation = Quaternion.LookRotation(direction);
                m_TargetRotation.z = -m_TargetRotation.y;
                m_TargetRotation.x = 0;
                m_TargetRotation.y = 0;

                northDirection = new Vector3(0,0, m_MainCamera.eulerAngles.y);
                m_Indicator.localRotation = m_TargetRotation * Quaternion.Euler(northDirection);

                m_timer += Time.deltaTime;
                yield return null;
            }
            m_timer = 0;
        }


    }
}
