using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class WeaponPropertyDisplayer : MonoBehaviour
    {
        [SerializeField] private GameObject m_ReloadNotification;
        [SerializeField] private Image m_FireModeImage;
        [SerializeField] private Image m_WeaponImage;
        [SerializeField] private Image m_BulletTemplateImage;
        [SerializeField] private Text m_RemainbulletText;
        //BulletTemplate 추가 안됨
        
        [Header("Using FireMode Icon")]
        [SerializeField] private Sprite [] m_FireModeIcon;
        public void DisplayReloadImage(bool isActive)
        {
            m_ReloadNotification.SetActive(isActive);
        }

        public void UpdateFireMode(int index)
        {
            m_FireModeImage.sprite = m_FireModeIcon[index];
        }

        public void UpdateWeaponIcon(Sprite sprite)
        {
            m_WeaponImage.sprite = sprite;
        }

        public void UpdateRemainBulletIcon(int value)
        {
            //대기
        }

        public void UpdateRemainBulletText(int value)
        {
            m_RemainbulletText.text = value.ToString();
        }

        public void ChangeWeapon(Sprite sprite)
        {
            m_WeaponImage.sprite = sprite;
            //더
        }
    }
}
