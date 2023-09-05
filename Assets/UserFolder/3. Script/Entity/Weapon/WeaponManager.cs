using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Object.Weapon;
using Entity.Object.Util;
using Controller.Player.Utility;
using EntityWeapon = Entity.Object.Weapon.Weapon;


namespace Manager.Weapon
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
        [SerializeField] private Transform m_Pivot;
        [SerializeField] private Syringe m_Syringe;
        [SerializeField] private FlashLight m_FlashLight; 

        [Header("Pooling")]
        [SerializeField] private Transform m_ActiveObjectPool;
        [SerializeField] private PoolableScript[] m_EffectPoolingObject;
        [SerializeField] private int[] m_PoolingCount;
        #endregion

        private readonly Dictionary<CustomKey, EntityWeapon> m_WeaponDictionary = new();
        private EntityWeapon m_CurrentWeapon = null;
        private PlayerData m_PlayerData;

        private int m_CurrentEquipIndex = -1; //현재 장착하고 있는 무기 슬롯 번호
        private const int m_ToolKitToEquipIndex = 5;
        private bool m_IsInteracting;

        public ObjectPoolManager.PoolingObject[] EffectPoolingObjectArray { get; private set; }
        public PlayerShakeController PlayerShakeController { get; private set; }

        public Vector3 OriginalPivotPosition { get; private set; }            //위치 조정용 부모 오브젝트 원래 위치
        public Quaternion OriginalPivotRotation { get; private set; }         //위치 조정용 부모 오브젝트 원래 각도
        public float OriginalFOV { get; private set; }

        public float DamageUpPercentage { get; private set; } = 1;  //배수 표시 -> 1배
        public float AttackSpeedUpPercentage { get; private set; } = 1;
        public float ReloadSpeedUpPercentage { get; private set; } = 1;

        private void Awake()
        {
            OriginalPivotPosition = m_Pivot.localPosition;
            OriginalPivotRotation = m_Pivot.localRotation;
            OriginalFOV = Camera.main.fieldOfView;

            foreach (Transform child in transform)
            {
                if(!child.TryGetComponent(out EntityWeapon weapon)) continue;
                m_WeaponDictionary.Add(new (weapon.EquipingType, weapon.GetItemIndex), weapon);
                weapon.PreAwake();
            }

            Transform root = transform.root;

            Controller.PlayerInputController playerInputController = root.GetComponent<Controller.PlayerInputController>();
            PlayerShakeController = root.GetComponent<PlayerShakeController>();
            m_PlayerData = root.GetComponent<PlayerData>();

            playerInputController.ChangeEquipment += TryWeaponChange;
            playerInputController.Heal += TryHealInteract;
            playerInputController.Light += () => TryWeaponChange(m_ToolKitToEquipIndex);
            m_PlayerData.WeaponDataFunc += RegisterWeapon;
            m_PlayerData.ReInit += () => ((ThrowingWeapon)m_CurrentWeapon).ReInit();
        }

        private void Start()
        {
            EffectPoolingObjectArray = new ObjectPoolManager.PoolingObject[m_EffectPoolingObject.Length];
            for (int i = 0; i < m_EffectPoolingObject.Length; i++)
            {
                EffectPoolingObjectArray[i] = ObjectPoolManager.Register(m_EffectPoolingObject[i], m_ActiveObjectPool);
                EffectPoolingObjectArray[i].GenerateObj(m_PoolingCount[i]);
            }
        }

        public WeaponData RegisterWeapon(int slotNumber, int weaponIndex)
        {
            m_WeaponDictionary.TryGetValue(new (slotNumber, weaponIndex), out EntityWeapon weapon);
            Sprite spriteIcon = weapon.WeaponIcon;
            int maxBullet = weapon.MaxBullet;
            int magazineBullet = maxBullet * 3;
            //if (slotNumber == 0)
            //{
            //    PlayerShakeController.ShakeCameraTransform(ShakeType.Changing);
            //    ChangeWeapon(slotNumber, weaponIndex);
            //}
            return new WeaponData(spriteIcon,maxBullet,magazineBullet);
        }

        public async void TryWeaponChange(int slotNumber)
        {
            if (m_IsInteracting) return;
            int index = m_PlayerData.Inventory.WeaponInfo[slotNumber].m_HavingWeaponIndex;
            if (index < 0) return;
            if (m_CurrentWeapon != null)
            {
                if (m_CurrentEquipIndex == slotNumber) return;
                if (!m_CurrentWeapon.CanChangeWeapon) return;
                await m_CurrentWeapon.UnEquip();
            }
            ChangeWeapon(slotNumber, index);
        }

        private void ChangeWeapon(int slotNumber, int index)
        {
            m_WeaponDictionary.TryGetValue(new(slotNumber, index), out m_CurrentWeapon);

            m_CurrentWeapon.Init();
            m_PlayerData.ChangeWeapon(m_CurrentWeapon.EquipingType, m_CurrentWeapon.BulletType, m_CurrentWeapon.CurrentFireMode, m_CurrentWeapon.WeaponIcon);
            m_CurrentEquipIndex = slotNumber;
        }

        private async void TryHealInteract()
        {
            if (m_PlayerData.Inventory.HealKitHavingCount < 1) return;
            if (m_PlayerData.IsSameMaxCurrentHP) return;
            if (m_IsInteracting) return;

            bool isNull = m_CurrentWeapon == null;
            bool canChangeWeapon = false;

            if (!isNull && !(canChangeWeapon = m_CurrentWeapon.CanChangeWeapon)) return;

            m_IsInteracting = true;
            if (isNull) await m_Syringe.TryHeal();
            else if (canChangeWeapon)
            {
                await m_CurrentWeapon.UnEquip();
                await m_Syringe.TryHeal();
                m_CurrentWeapon.Init();
            }
            m_PlayerData.UsingHealKit(-1);
            m_IsInteracting = false;
        }

        #region UnityEvent

        public void DamageUp(float amount)
        {
            DamageUpPercentage += amount;
        }

        public void AttackSpeedUp(float amount)
        {
            AttackSpeedUpPercentage += amount;
        }

        public void ReloadSpeedUp(float amount)
        {
            ReloadSpeedUpPercentage += amount;
        }
        #endregion
    }
}
