using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "SurfaceSetting", menuName = "Scriptable Object/SurfaceSettings", order = int.MaxValue - 3)]
    public class SurfaceScriptable : ScriptableObject
    {
        [Serializable]
        public class EffectPair
        {
            [Tooltip("Sound")]
            public AudioClip[] audioClips;

            [Tooltip("Effect")]
            public GameObject effectObject;
        }

        [Header("Type")]
        [Tooltip("표면 타입")]
        public EnumType.SurfaceType surfaceType;

        [Space(10)]
        [Tooltip("Texture")]
        public Texture[] surfaceTextures;

        [Space(10)]
        [Header("Sound & Effect")]
        public EffectPair softFootStepEffect;

        [Space(10)]
        public EffectPair hardFootStepEffect;

        [Space(10)]
        public EffectPair bulletHitEffect;

        [Space(10)]
        public EffectPair SlashEffect;
    }
}
