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

        #region Test Blood Effect
        [SerializeField] private Transform m_ActiveObjectPool;
        [SerializeField] private BloodEffectController BloodAttach;
        [SerializeField] private BloodEffectController[] BloodFX;

        [SerializeField] private int m_BloodAttachPoolingCount;
        [SerializeField] private int m_BloodFXPoolingCount;

        private ObjectPoolManager.PoolingObject m_BloodAttachPool;
        private ObjectPoolManager.PoolingObject[] m_BloodFXPool;

        private void Start()
        {
            RegisterPoolingObject();
        }

        private void RegisterPoolingObject()
        {
            m_BloodFXPool = new ObjectPoolManager.PoolingObject[BloodFX.Length];
            for (int i = 0; i < BloodFX.Length; i++)
            {
                m_BloodFXPool[i] = ObjectPoolManager.Register(BloodFX[i], m_ActiveObjectPool);
                m_BloodFXPool[i].GenerateObj(m_BloodFXPoolingCount);
            }
            m_BloodAttachPool = ObjectPoolManager.Register(BloodAttach, m_ActiveObjectPool);
            m_BloodAttachPool.GenerateObj(m_BloodAttachPoolingCount);
        }

        private Transform GetNearestObject(Transform hit, Vector3 hitPos)
        {
            float closestPos = float.MaxValue;
            Transform closestBone = null;
            Transform[] childs = hit.GetComponentsInChildren<Transform>();

            foreach (var child in childs)
            {
                float dist = Vector3.SqrMagnitude(child.position - hitPos);
                if (dist < closestPos)
                {
                    closestPos = dist;
                    closestBone = child;
                }
            }

            return closestBone;
        }

        public void InstanceBloodEffect(ref RaycastHit hit, int effectIndex)
        {
            Quaternion rotation = Quaternion.LookRotation(hit.normal, -GravityManager.GravityVector) * Quaternion.Euler(0, -90, 0);
            if(effectIndex >= BloodFX.Length)
            {
                effectIndex = Random.Range(0, BloodFX.Length);
                Debug.LogWarning("Effect index out of range");
            }

            BloodEffectController bloodFX = (BloodEffectController)m_BloodFXPool[effectIndex].GetObject(false);
            bloodFX.transform.SetPositionAndRotation(hit.point, rotation);
            bloodFX.gameObject.SetActive(true);
            bloodFX.Init(m_BloodFXPool[effectIndex]);

            Transform nearestBone = GetNearestObject(hit.transform.root, hit.point);
            if (nearestBone != null)
            {
                BloodEffectController attachBloodInstance = (BloodEffectController)m_BloodAttachPool.GetObject(false, false);
                Transform bloodT = attachBloodInstance.transform;

                bloodT.position = hit.point;
                bloodT.localRotation = Quaternion.identity;
                bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
                bloodT.LookAt(hit.point + hit.normal);
                bloodT.Rotate(90, 0, 0);
                bloodT.parent = nearestBone;

                attachBloodInstance.gameObject.SetActive(true);
                attachBloodInstance.Init(m_BloodAttachPool);
            }
        }

        #endregion
    }
}
