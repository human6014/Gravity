using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    public class WeaponSoundScriptable : ScriptableObject
    {
        [System.Serializable]
        public class DelaySoundClip
        {
            public AudioClip audioClip;
            public float delayTime;
        }

        [Header("Parent")]
        public AudioClip equipSound;
        public AudioClip unequipSound;
    }
}
