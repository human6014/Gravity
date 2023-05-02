using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class WeaponPropertyDisplayer : MonoBehaviour
    {
        private const int m_MaxBulletCount = 40;
        private int top = -1;       //top번째 배열 원소는 활성화 상태임

        [SerializeField] private GameObject m_ReloadNotification;
        [SerializeField] private Image m_FireModeImage;
        [SerializeField] private Image m_WeaponImage;
        [SerializeField] private Text m_RemainbulletText;
        [SerializeField] private Transform m_BulletGroup;
        
        [Header("Using FireMode Icon")]
        [SerializeField] private Sprite [] m_FireModeIcon;
        [SerializeField] private Sprite [] m_BulletIcon;

        public readonly GameObject[] m_BulletTemplate = new GameObject[m_MaxBulletCount];
        private Sprite currentBulletImage;

        public void Init()
        {
            m_ReloadNotification.SetActive(false);
            m_FireModeImage.enabled = false;
            m_WeaponImage.enabled = false;
            m_RemainbulletText.text = "0";
        }

        private void Awake()
        {
            int iter = 0;
            foreach(Transform g in m_BulletGroup)
            {
                m_BulletTemplate[iter] = g.gameObject;
                iter++;
            }
        }

        public void ChangeWeapon(int bulletType, int currentRemainBullet)
        {
            for(int i = top; i > currentRemainBullet; i--)
                m_BulletTemplate[i - 1].SetActive(false);
            
            if (bulletType <= 0 || bulletType >= 5) return;

            currentBulletImage = m_BulletIcon[bulletType - 1];
            for (int i = 0; i < currentRemainBullet; i++)
            {
                m_BulletTemplate[i].GetComponent<Image>().sprite = currentBulletImage;
                m_BulletTemplate[i].SetActive(true);
            }
            top = currentRemainBullet;
        }

        public void DisplayReloadImage(bool isActive)
        {
            m_ReloadNotification.SetActive(isActive);
        }

        public void UpdateFireMode(int index)
        {
            if (index == 0)
            {
                m_FireModeImage.enabled = false;
                return;
            }
            m_FireModeImage.enabled = true;
            m_FireModeImage.sprite = m_FireModeIcon[index - 1];
        }

        public void UpdateWeaponIcon(Sprite sprite)
        {
            m_WeaponImage.enabled = true;
            m_WeaponImage.sprite = sprite;
        }

        public void UpdateRemainBulletIcon(int value)
        {
            if (value >= top)
            {
                //Debug.Log(value);
                //Debug.Log(top);
                for (int i = top; i < value; i++) m_BulletTemplate[i].SetActive(true);  //바로 5번 무기 들고 공격 시 버그
                top = value;
            }
            else
            {
                for (int i = top; i > value; i--, top--) m_BulletTemplate[i - 1].SetActive(false);
            }
        }

        public void UpdateRemainBulletText(int value)
        {
            m_RemainbulletText.text = value.ToString();
        }
    }
}
