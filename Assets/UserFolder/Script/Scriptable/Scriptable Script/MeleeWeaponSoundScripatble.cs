using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeWeaponSoundSetting", menuName = "Scriptable Object/MeleeWeaponSoundSettings", order = int.MaxValue - 5)]
public class MeleeWeaponSoundScripatble : WeaponSoundScriptable
{
    public AudioClip[] m_LightAttackSound;
    public AudioClip[] m_HeavyAttackSound;

}
