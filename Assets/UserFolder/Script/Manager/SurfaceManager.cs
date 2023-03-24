using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriptable;


namespace Manager
{
    public class SurfaceManager : MonoBehaviour
    {
        [SerializeField] private SurfaceScriptable [] surfaceInfo;

        private readonly HashSet<Material>[] surfaceMaterialHashSet = new HashSet<Material>[3];

        
        public SurfaceScriptable GetSurfaceInfo(int index) => surfaceInfo[index];

        public EnumType.SurfaceType GetSurfaceType(int index) => surfaceInfo[index].surfaceType;

        public EffectPair GetSoftFootEffectPair(int index) => surfaceInfo[index].softFootStepEffect;

        public EffectPair GetHardFootEffectPair(int index) => surfaceInfo[index].hardFootStepEffect;

        public EffectPair GetBulletHitEffectPair(int index) => surfaceInfo[index].bulletHitEffect;

        public EffectPair GetSlashHitEffectPair(int index) => surfaceInfo[index].SlashHitEffect;

        private void Awake()
        {
            HasingSurfaceMaterials();
        }

        private void HasingSurfaceMaterials()
        {
            for(int i=0;i<surfaceInfo.Length;i++)
            {
                surfaceMaterialHashSet[i] = new HashSet<Material>();
                for (int j = 0; j < surfaceInfo[i].surfaceMaterials.Length; j++)
                    surfaceMaterialHashSet[i].Add(surfaceInfo[i].surfaceMaterials[j]);
            }
        }

        public int IsInMaterial(Material material)
        {
            for(int i=0;i< surfaceMaterialHashSet.Length; i++)
            {
                if (surfaceMaterialHashSet[i].Contains(material)) return i;
            }
            return -1;
        }

        public GameObject GetSurfaceBulletEffectObject(Material material)
        {
            for(int i = 0; i < surfaceMaterialHashSet.Length; i++)
            {
                if (surfaceMaterialHashSet[i].Contains(material)) return surfaceInfo[i].bulletHitEffect.effectObject;
            }
            return null;
        }

        public GameObject GetSurfaceSlashEffectObject(Material material)
        {
            for (int i = 0; i < surfaceMaterialHashSet.Length; i++)
            {
                if (surfaceMaterialHashSet[i].Contains(material)) return surfaceInfo[i].SlashHitEffect.effectObject;
            }
            return null;
        }
    }
}
