using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Equipment
{
    [CreateAssetMenu(fileName = "RangeWeaponSoundSetting", menuName = "Scriptable Object/RangeWeaponSoundSettings", order = int.MaxValue - 5)]
    public class RangeWeaponSoundScriptable : WeaponSoundScriptable
    {
        [Space(15)]
        [Header("Child")]
        [Header("Fire")]
        public AudioClip[] fireSound;
        public AudioClip[] fireTailSound;
        public AudioClip[] emptySound;

        [Header("Change Fire Mode")]
        public AudioClip[] changeModeSound;

        [Header("Reload")]
        public DelaySoundClip[] reloadSoundClips;
        public DelaySoundClip[] emptyReloadSoundClips;
        public DelaySoundClip[] instantReloadSoundClips;

        [Header("Additional Reload")]
        public DelaySoundClip[] reloadStartSoundClips;
        public DelaySoundClip[] reloadEndSoundClips;
    }
}
