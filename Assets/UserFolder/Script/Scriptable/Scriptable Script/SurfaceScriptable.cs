using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "SurfaceSetting", menuName = "Scriptable Object/SurfaceSettings", order = int.MaxValue - 3)]
    public class SurfaceScriptable : ScriptableObject
    {
        [Header("Type")]
        [Tooltip("표면 타입")]
        public EnumType.SurfaceType surfaceType;

        [Space(10)]
        [Tooltip("Material")]
        public Material [] surfaceMaterials;

        [Space(10)]
        [Header("Sound & Effect")]
        public EffectPair softFootStepEffect;

        [Space(10)]
        public EffectPair hardFootStepEffect;

        [Space(10)]
        public EffectPair bulletHitEffect;

        [Space(10)]
        public EffectPair SlashHitEffect;
    }

    [Serializable]
    public class EffectPair
    {
        [Tooltip("Sound")]
        public AudioClip[] audioClips;

        [Tooltip("Effect")]
        public GameObject effectObject;
    }
}
