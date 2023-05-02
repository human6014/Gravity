using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Equipment
{
    [CreateAssetMenu(fileName = "MeleeWeaponSoundSetting", menuName = "Scriptable Object/MeleeWeaponSoundSettings", order = int.MaxValue - 6)]
    public class MeleeWeaponSoundScripatble : WeaponSoundScriptable
    {
        [Header("Child")]
        public AudioClip[] m_LightAttackSound;
        public AudioClip[] m_HeavyAttackSound;
    }
}
