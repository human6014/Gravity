using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scriptable
{
    [CreateAssetMenu(fileName = "WeaponSoundSetting", menuName = "Scriptable Object/WeaponSoundSettings", order = int.MaxValue - 5)]
    public class RangeWeaponSoundScriptable : ScriptableObject
    {
        [System.Serializable]
        public class DelaySoundClip
        {
            public AudioClip audioClip;
            public float delayTime;
        }

        public AudioClip[] fireSound;
        public AudioClip[] emptySound;
        public AudioClip[] changeModeSound;
        public AudioClip[] equipSound;
        public AudioClip[] unequipSound;

        public DelaySoundClip[] reloadSoundClips;
        public DelaySoundClip[] emptyReloadSoundClips;
    }
}
