using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "CrossHairSetting", menuName = "Scriptable Object/CrossHairSettings", order = int.MaxValue - 4)]
    public class CrossHairScripatble : ScriptableObject
    {
        public AnimatorOverrideController m_AnimatorController;
        public Sprite [] crossHairSprite;

        public float m_CrouchAccurancy = 0;
        public float m_IdleAccuracy = 0.05f;
        public float m_WalkAccuracy = 0.1f;
        public float m_JumpAccuracy = 0.2f;
    }
}
