using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class ItemSlotDisplayer : MonoBehaviour
    {
        [SerializeField] private Image[] m_WeaponIconImage;
        [SerializeField] private Image[] m_FocusSlotImage;

        private readonly Color m_OriginalColor = new(255, 255, 255, 0);
        private readonly Color m_FocusColor = new(255,255,255,50);
        private Image m_CurrentImage = null;
        private bool m_IsNull = true;

        public void UpdateWeaponSlotIcon(int slotNumber, Sprite sprite) 
            => m_WeaponIconImage[slotNumber].sprite = sprite;
        

        public void UpdateFocusSlot(int slotNumber)
        {
            if (slotNumber >= m_FocusSlotImage.Length)
            {
                if (m_IsNull) return;
                m_CurrentImage.color = m_OriginalColor;
                m_CurrentImage = null;
                m_IsNull = true;
                return;
            }
            if (!m_IsNull) m_CurrentImage.color = m_OriginalColor;

            m_FocusSlotImage[slotNumber].color = m_FocusColor;
            m_CurrentImage = m_FocusSlotImage[slotNumber];
            m_IsNull = false;
        }
    }
}
