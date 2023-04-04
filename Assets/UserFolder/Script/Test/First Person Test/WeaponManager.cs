using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

namespace Manager
{
    public class WeaponManager : MonoBehaviour
    {
        private ObjectPoolManager.PoolingObject[] m_EffectPoolingObjectArray;
        private ObjectPoolManager.PoolingObject[] m_CasingPoolingObjectArray;
        private ObjectPoolManager.PoolingObject[] m_MagazinePoolingObjectArray;

        private Dictionary<int, Weapon> m_WeaponDictionary = new Dictionary<int, Weapon>();

        private Weapon m_CurrentWeapon;
        [SerializeField] private int EquipingItemIndex = 6;
        public void SetObjectPool(
            ObjectPoolManager.PoolingObject[] effectPooling,
            ObjectPoolManager.PoolingObject[] casingPooling,
            ObjectPoolManager.PoolingObject[] magazinePooling)
        {
            m_EffectPoolingObjectArray = effectPooling;
            m_CasingPoolingObjectArray = casingPooling;
            m_MagazinePoolingObjectArray = magazinePooling;
        }

        public ObjectPoolManager.PoolingObject[] GetEffectPoolingObjects() => m_EffectPoolingObjectArray;
        public ObjectPoolManager.PoolingObject GetEffectPoolingObject(int index) => m_EffectPoolingObjectArray[index];
        public ObjectPoolManager.PoolingObject GetCasingPoolingObject(int index) => m_CasingPoolingObjectArray[index];
        public ObjectPoolManager.PoolingObject GetMagazinePoolingObject(int index) => m_MagazinePoolingObjectArray[index];

        private void Awake()
        {
            foreach(Transform child in transform)
            {
                if(!child.TryGetComponent(out Weapon weapon)) continue;
                m_WeaponDictionary.Add(weapon.GetItemIndex(),weapon);
            }
        }

        [ContextMenu("EquipWeapon")]
        public void EquipWeapon()
        {
            if (m_CurrentWeapon != null) m_CurrentWeapon.Dispose();

            m_WeaponDictionary.TryGetValue(EquipingItemIndex, out m_CurrentWeapon);
            m_CurrentWeapon.Init();
        }
    }
}
