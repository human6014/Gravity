using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "RangeWeaponSoundSetting", menuName = "Scriptable Object/RangeWeaponSoundSettings", order = int.MaxValue - 5)]
    public class RangeWeaponSoundScriptable : WeaponSoundScriptable
    {
        public AudioClip[] fireSound;
        public AudioClip[] fireTailSound;
        public AudioClip[] emptySound;
        public AudioClip[] changeModeSound;

        public DelaySoundClip[] reloadSoundClips;
        public DelaySoundClip[] emptyReloadSoundClips;
    }
}
