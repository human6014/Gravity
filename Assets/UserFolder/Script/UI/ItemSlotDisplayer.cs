using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class ItemSlotDisplayer : MonoBehaviour
    {
        [SerializeField] private Image[] m_WeaponIconImage;

        public void UpdateWeaponSlotIcon(int slotNumber, Sprite sprite)
        {
            m_WeaponIconImage[slotNumber].sprite = sprite;
        }
    }
}
