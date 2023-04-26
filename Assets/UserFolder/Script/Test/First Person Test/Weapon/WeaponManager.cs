using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        #region SerializeField
        [SerializeField] private PlayerData m_PlayerData;
        [SerializeField] private PlayerInputController m_PlayerInputController;
        [SerializeField] private Transform m_Pivot;
        [SerializeField] private Syringe m_Syringe;
        [SerializeField] private FlashLight m_FlashLight; 

        [Header("Pooling")]
        [SerializeField] private Transform m_ActiveObjectPool;
        [SerializeField] private PoolableScript[] m_EffectPoolingObject;
        [SerializeField] private int[] m_PoolingCount;
        #endregion

        private readonly Dictionary<CustomKey, Weapon> m_WeaponDictionary = new();
        private Weapon m_CurrentWeapon = null;
        private int m_CurrentEquipIndex = -1; //현재 장착하고 있는 무기 슬롯 번호
        private const int m_ToolKitToEquipIndex = 5;
        private bool IsInteracting;

        public Vector3 m_OriginalPivotPosition { get; private set; }            //위치 조정용 부모 오브젝트 원래 위치
        public Quaternion m_OriginalPivotRotation { get; private set; }         //위치 조정용 부모 오브젝트 원래 각도
        public float m_OriginalFOV { get; private set; }
        public ObjectPoolManager.PoolingObject[] m_EffectPoolingObjectArray { get; private set; }

        private void Awake()
        {
            m_OriginalPivotPosition = m_Pivot.localPosition;
            m_OriginalPivotRotation = m_Pivot.localRotation;
            m_OriginalFOV = Camera.main.fieldOfView;

            foreach (Transform child in transform)
            {
                if(!child.TryGetComponent(out Weapon weapon)) continue;
                m_WeaponDictionary.Add(new CustomKey(weapon.EquipingType, weapon.GetItemIndex), weapon);
            }
            m_PlayerInputController.ChangeEquipment += TryWeaponChange;
            m_PlayerInputController.Heal += TryHealInteract;
            m_PlayerInputController.Light += () => TryWeaponChange(m_ToolKitToEquipIndex);
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
            => m_PlayerData.GetInventory().WeaponInfo[slotNumber].m_HavingWeaponIndex = index;

        private async void TryWeaponChange(int slotNumber)
        {
            if (IsInteracting) return;
            int index = m_PlayerData.GetInventory().WeaponInfo[slotNumber].m_HavingWeaponIndex;
            if (m_CurrentWeapon != null)
            {
                if (m_CurrentEquipIndex == slotNumber) return;
                if (!m_CurrentWeapon.CanChangeWeapon) return;
                await m_CurrentWeapon.UnEquip();
                ChangeWeapon(slotNumber, index);
            }
            else
            {
                m_WeaponDictionary.TryGetValue(new CustomKey(slotNumber, index), out m_CurrentWeapon);
                m_PlayerData.ChangeWeapon(m_CurrentWeapon.EquipingType, m_CurrentWeapon.m_BulletType, m_CurrentWeapon.WeaponIcon);
                m_CurrentWeapon.Init();
            }
            m_CurrentEquipIndex = slotNumber;
        }

        public void ChangeWeapon(int slotNumber, int index)
        {
            CustomKey key = new(slotNumber, index);
            m_WeaponDictionary.TryGetValue(key, out m_CurrentWeapon);

            m_CurrentWeapon.Init();
            
            m_PlayerData.ChangeWeapon(m_CurrentWeapon.EquipingType, m_CurrentWeapon.m_BulletType, m_CurrentWeapon.WeaponIcon);
        }

        private async void TryHealInteract()
        {
            if (m_PlayerData.GetInventory().HealKitHavingCount < 1) return;
            if (m_PlayerData.PlayerMaxHP <= m_PlayerData.PlayerHP) return;
            if (IsInteracting) return;

            IsInteracting = true;
            if (m_CurrentWeapon == null) await m_Syringe.TryHeal();
            else if (m_CurrentWeapon.CanChangeWeapon)
            {
                await m_CurrentWeapon.UnEquip();
                await m_Syringe.TryHeal();
                m_CurrentWeapon.Init();
            }
            m_PlayerData.UsingHealKit(-1);
            IsInteracting = false;
        }
    }
}
