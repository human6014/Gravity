using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Equipment
{
    [CreateAssetMenu(fileName = "FlashLightSoundSetting", menuName = "Scriptable Object/FlashLightSoundSettings", order = int.MaxValue - 13)]
    public class FlashLightSoundScriptable : WeaponSoundScriptable
    {
        public AudioClip m_SwitchOnSound;
        public AudioClip m_SwitchOffSound;
    }
}
