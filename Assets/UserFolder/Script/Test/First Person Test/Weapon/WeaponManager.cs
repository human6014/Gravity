using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

namespace Manager
{
    public struct CustomKey
    {
        private readonly int keyX;
        private readonly int keyY;
        public CustomKey(int keyX, int keyY)
        {
            this.keyX = keyX;
            this.keyY = keyY;
        }
    }

    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private PlayerData m_PlayerData;
        [SerializeField] private PlayerInputController m_PlayerInputController;
        [SerializeField] private Transform m_Pivot;
        [Header("Pooling")]
        [SerializeField] private Transform m_ActiveObjectPool;
        [SerializeField] private PoolableScript[] m_EffectPoolingObject;
        [SerializeField] private int[] m_PoolingCount;

        [SerializeField] private int m_EquipingItemIndex = 6;

        public Vector3 m_OriginalPivotPosition { get; private set; }            //위치 조정용 부모 오브젝트 원래 위치
        public Quaternion m_OriginalPivotRotation { get; private set; }         //위치 조정용 부모 오브젝트 원래 각도
        public float m_OriginalFOV { get; private set; }

        private readonly Dictionary<CustomKey, Weapon> m_WeaponDictionary = new();
        private const int m_EquipingTypeLength = 5;
        private Weapon m_CurrentWeapon;

        //현재 장착하고 있는 무기 슬롯 번호
        private int m_CurrentEquipIndex = -1;

        public ObjectPoolManager.PoolingObject[] m_EffectPoolingObjectArray { get; private set; }

        private void Awake()
        {
            m_OriginalPivotPosition = m_Pivot.localPosition;
            m_OriginalPivotRotation = m_Pivot.localRotation;
            m_OriginalFOV = Camera.main.fieldOfView;

            foreach (Transform child in transform)
            {
                if(!child.TryGetComponent(out Weapon weapon)) continue;
                m_WeaponDictionary.Add(new CustomKey(weapon.GetEquipingType(),weapon.GetItemIndex()), weapon);
            }
            m_PlayerInputController.ChangeEquipment += TryWeaponChange;
        }

        private void Start()
        {
            m_EffectPoolingObjectArray = new ObjectPoolManager.PoolingObject[m_EffectPoolingObject.Length];
            for (int i = 0; i < m_EffectPoolingObject.Length; i++)
            {
                m_EffectPoolingObjectArray[i] = ObjectPoolManager.Register(m_EffectPoolingObject[i], m_ActiveObjectPool);
                m_EffectPoolingObjectArray[i].GenerateObj(m_PoolingCount[i]);
            }
        }

        public void RegisterWeapon(int slotNumber, int index)
            => m_PlayerData.GetInventory().SetHavingWeaponIndex(slotNumber, index);
        

        public void AddThrowingWeapon(int value)
            => m_PlayerData.GetInventory().AddThrowingWeapon(value);
        

        public void AddHealKit(int value)
            => m_PlayerData.GetInventory().AddThrowingWeapon(value);
        

        private void TryWeaponChange(int slotNumber)
        {
            int index = m_PlayerData.GetInventory().GetHavingWeaponIndex(slotNumber);
            if (index == -1) return;
            if (m_CurrentWeapon != null)
            {
                if (m_CurrentEquipIndex == slotNumber) return;
                if (!m_CurrentWeapon.CanChangeWeapon) return;
                m_CurrentWeapon.Dispose();
            }
            else
            {
                
                m_WeaponDictionary.TryGetValue(new CustomKey(slotNumber, index), out m_CurrentWeapon);
                m_CurrentWeapon.Init();
            }
            m_CurrentEquipIndex = slotNumber;
        }

        public void ChangeWeapon()
        {
            m_WeaponDictionary.TryGetValue(new CustomKey(m_CurrentEquipIndex, m_PlayerData.GetInventory().GetHavingWeaponIndex(m_CurrentEquipIndex)), out m_CurrentWeapon);
            m_CurrentWeapon.Init();
        }
    }
}
