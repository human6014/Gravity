using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class PlayerStatDisplayer : MonoBehaviour
    {
        [SerializeField] private Image m_HPImage;
        [SerializeField] private Image m_MPImage;

        public void Init(int maxHP,int maxMP, float realToAmountConst)
        {
            m_HPImage.fillAmount = maxHP * realToAmountConst;
            m_MPImage.fillAmount = maxMP * realToAmountConst;
        }

        public void UpdateHPImage(float value)
        {
            m_HPImage.fillAmount = value;
        }

        public void UpdateMPImage(float value)
        {
            m_MPImage.fillAmount = value;
        }
    }
}
