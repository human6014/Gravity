using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Scriptable
{
    public enum SurfaceType
    {
        Concrete = 0,
        Metal = 1,
        Wood = 2
    }

    [CreateAssetMenu(fileName = "SurfaceSetting", menuName = "Scriptable Object/SurfaceSettings", order = int.MaxValue - 3)]
    public class SurfaceScriptable : ScriptableObject
    {
        [Header("Type")]
        [Tooltip("표면 타입")]
        public SurfaceType surfaceType;

        [Space(10)]
        [Tooltip("Material")]
        public Material [] surfaceMaterials;

        [Space(10)]
        [Header("Sound & Effect")]
        public AudioClip[] m_SoftFootStepSounds;

        [Space(10)]
        public AudioClip[] m_HardFootStepSounds;

        [Space(10)]
        public AudioClip[] m_BulletHitSounds;

        [Space(10)]
        public AudioClip[] m_SlashHitSounds;
    }
}
