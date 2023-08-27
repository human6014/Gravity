using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriptable;


namespace Manager
{
    public class SurfaceManager : MonoBehaviour
    {
        [SerializeField] private SurfaceScriptable[] surfaceInfo;

        private readonly HashSet<Material>[] surfaceMaterialHashSet = new HashSet<Material>[3];


        public SurfaceScriptable GetSurfaceInfo(int index) => surfaceInfo[index];

        public SurfaceType GetSurfaceType(int index) => surfaceInfo[index].surfaceType;

        public AudioClip[] GetSoftFootEffectSounds(int index) => surfaceInfo[index].m_SoftFootStepSounds;

        public AudioClip[] GetHardFootEffectSounds(int index) => surfaceInfo[index].m_HardFootStepSounds;

        public AudioClip[] GetBulletHitEffectSounds(int index) => surfaceInfo[index].m_BulletHitSounds;

        public AudioClip[] GetSlashHitEffectSounds(int index) => surfaceInfo[index].m_SlashHitSounds;


        private void Awake() => HasingSurfaceMaterials();
        
        private void HasingSurfaceMaterials()
        {
            for (int i = 0; i < surfaceInfo.Length; i++)
            {
                surfaceMaterialHashSet[i] = new HashSet<Material>();
                for (int j = 0; j < surfaceInfo[i].surfaceMaterials.Length; j++)
                    surfaceMaterialHashSet[i].Add(surfaceInfo[i].surfaceMaterials[j]);
            }
        }

        public int IsInMaterial(Material material)
        {
            for (int i = 0; i < surfaceMaterialHashSet.Length; i++)
            {
                if (surfaceMaterialHashSet[i].Contains(material)) return i;
            }
            return -1;
        }
    }
}
