using System;
using System.Collections;
using System.Collections.Generic;
using Test;
using UnityEngine;

namespace Manager
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private PlayerInputController m_PlayerInputController;
        [SerializeField] private Transform m_Pivot;
        [Header("Pooling")]
        [SerializeField] private Transform m_ActiveObjectPool;
        [SerializeField] private PoolableScript[] m_EffectPoolingObject;
        [SerializeField] private int[] m_PoolingCount;

        [SerializeField] private int m_EquipingItemIndex = 6;


        [Header("Equip Index Test")]
        [SerializeField] private int HavingMeleeWeaponIndex;
        [SerializeField] private int HavingPistolWeaponIndex;
        [SerializeField] private int HavingAutoRifleWeaponIndex;
        [SerializeField] private int HavingSubRifleWeaponIndex;
        [SerializeField] private int HavingThrowingWeaponIndex;

        [Space(10)]
        [SerializeField] private int ThrowingWeaponHavingCount;

        public Vector3 m_OriginalPivotPosition { get; private set; }            //위치 조정용 부모 오브젝트 원래 위치
        public Quaternion m_OriginalPivotRotation { get; private set; }         //위치 조정용 부모 오브젝트 원래 각도
        public float m_OriginalFOV { get; private set; }

        //private readonly Dictionary<int, Weapon>[] m_WeaponDictionary = new Dictionary<int, Weapon>();
        private readonly Dictionary<Tuple<int, int>, Weapon> m_WeaponDictionary = new();
        [SerializeField] private Weapon[][] m_Weapon = new Weapon[5][];
        private const int m_EquipingTypeLength = 5;
        private Weapon m_CurrentWeapon;
        private Inventory m_Inventory;
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
                m_WeaponDictionary.Add(new Tuple<int,int>(weapon.GetEquipingType(),weapon.GetItemIndex()), weapon);
            }
            m_Inventory = new Inventory(5);
            m_PlayerInputController.ChangeEquipment += TryWeaponChange;

            m_Inventory.HavingWeaponIndex[0] = HavingMeleeWeaponIndex;
            m_Inventory.HavingWeaponIndex[1] = HavingPistolWeaponIndex;
            m_Inventory.HavingWeaponIndex[2] = HavingAutoRifleWeaponIndex;
            m_Inventory.HavingWeaponIndex[3] = HavingSubRifleWeaponIndex;
            m_Inventory.HavingWeaponIndex[4] = HavingThrowingWeaponIndex;

            m_Inventory.ThrowingWeaponHavingCount = ThrowingWeaponHavingCount;
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

        [ContextMenu("EquipWeapon")]
        public void EquipWeapon()
        {
            if (m_CurrentWeapon != null) m_CurrentWeapon.Dispose();

            //m_WeaponDictionary.TryGetValue(m_EquipingItemIndex, out m_CurrentWeapon);
            m_CurrentWeapon.Init();
        }

        private void TryWeaponChange(int index)
        {
            if (m_CurrentWeapon != null)
            {
                if (m_CurrentEquipIndex == index) return;
                if (m_CurrentWeapon.IsEquiping) return;
                if (m_CurrentWeapon.IsUnequiping) return;
                if (m_CurrentWeapon.IsReloading) return;
                m_CurrentWeapon.Dispose();
            }
            else
            {
                m_WeaponDictionary.TryGetValue(new Tuple<int, int>(index, m_Inventory.HavingWeaponIndex[index]), out m_CurrentWeapon);
                m_CurrentWeapon.Init();
            }
            m_CurrentEquipIndex = index;
        }

        public void ChangeWeapon()
        {
            m_WeaponDictionary.TryGetValue(new Tuple<int, int>(m_CurrentEquipIndex, m_Inventory.HavingWeaponIndex[m_CurrentEquipIndex]), out m_CurrentWeapon);
            m_CurrentWeapon.Init();
        }
    }
}
