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

        public void DisplayBloodScreen()
        {

        }

        public IEnumerator DisplayHitDirection()
        {
            yield return null;
        }
    }
}
