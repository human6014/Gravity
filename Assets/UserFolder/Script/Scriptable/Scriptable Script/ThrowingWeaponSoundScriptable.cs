using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable {

    [CreateAssetMenu(fileName = "ThrowingWeaponSoundSetting", menuName = "Scriptable Object/ThrowingWeaponSoundSettings", order = int.MaxValue - 9)]
    public class ThrowingWeaponSoundScriptable : WeaponSoundScriptable
    {
        [Space(15)]
        [Header("Child")]
        public DelaySoundClip[] throwSound;
    }
}
