using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSoundScriptable : ScriptableObject
{
    [System.Serializable]
    public class DelaySoundClip
    {
        public AudioClip audioClip;
        public float delayTime;
    }

    public AudioClip[] equipSound;
    public AudioClip[] unequipSound;
}
