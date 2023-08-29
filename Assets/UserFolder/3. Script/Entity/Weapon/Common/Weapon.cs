using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.Weapon;
using System.Threading.Tasks;
using Scriptable.Equipment;
using Controller.Util;
using Controller.Player.Utility;

public enum EquipingWeaponType
{
    Melee = 0,
    Pistol,
    MainRifle,
    SubRifle,
    Throwing,
    Other
}

[System.Flags]
public enum FireMode
{
    None = 1,
    Auto = 2,
    Burst = 4,
    Semi = 8,
}

namespace Entity.Object.Weapon
{
    public class Weapon : MonoBehaviour
    {
        #region SerializeField
        [Header("Parent")]
        [Header("Index")]
        [SerializeField] protected int ItemIndex;
        [SerializeField] protected EquipingWeaponType m_EquipingWeaponType;

        [Header("Fire mode")]
        [CustomAttribute.MultiEnum] [SerializeField] protected FireMode m_FireMode = FireMode.None;

        [Header("Weapon Animation")]
        [SerializeField] protected Animator m_ArmAnimator; //팔 애니메이터
        [SerializeField] protected AnimatorOverrideController m_ArmOverrideController = null;   // 덮어씌울 팔 애니메이션들

        [Header("Scriptable")]
        [SerializeField] protected WeaponStatScriptable m_WeaponStatScriptable;
        [SerializeField] protected WeaponSoundScriptable m_WeaponSoundScriptable;

        [Header("Pos Change")]
        [SerializeField] protected Transform m_Pivot;                 //위치 조정용 부모 오브젝트

        [Header("Weapon Sway")]
        [SerializeField] private WeaponSway m_WeaponSway;

        [Header("Arm")]
        [SerializeField] protected ArmController m_ArmController;

        #endregion


        private UI.Player.CrossHairDisplayer m_CrossHairDisplayer;
        private WaitForSeconds m_WaitEquipingTime;
        private const int m_UnequipingTime = 550;


        #region Property
        protected PlayerData PlayerData { get; private set; }

        /// <summary>
        /// 인벤토리의 Weapon들을 담는 클래스
        /// </summary>
        protected WeaponInfo WeaponInfo { get; private set; }

        /// <summary>
        /// 사용자의 입력을 받는 스크립트
        /// </summary>
        protected Controller.PlayerInputController PlayerInputController { get; private set; }

        /// <summary>
        /// 현재 자신의 무기 에니메이터
        /// </summary>
        protected Animator EquipmentAnimator { get; private set; }

        /// <summary>
        /// 모든 Equipment의 공통 AudioSource
        /// </summary>
        protected AudioSource AudioSource { get; private set; }

        /// <summary>
        /// Weapon들을 관리하는 매니저 클래스
        /// </summary>
        protected WeaponManager WeaponManager { get; private set; }

        /// <summary>
        /// 메인 카메라
        /// </summary>
        protected Camera MainCamera { get; private set; }

        /// <summary>
        /// 무기 아이콘
        /// </summary>
        public Sprite WeaponIcon { get => m_WeaponStatScriptable.m_WeaponIcon; }

        public virtual int MaxBullet { get => 0; set => MaxBullet = value; }
        public virtual bool CanChangeWeapon { get => !IsEquiping && !IsUnequiping; }
        public bool IsEquiping { get; private set; }
        public bool IsUnequiping { get; private set; }

        public FireMode CurrentFireMode { get; protected set; } = FireMode.None;
        public AttackType BulletType { get => m_WeaponStatScriptable.m_BulletType; }

        public int EquipingType { get => (int)m_EquipingWeaponType; }
        public int GetItemIndex { get => ItemIndex; }
        
        #endregion

        public virtual void PreAwake()
        {
            PlayerData = transform.root.GetComponent<PlayerData>();
        }

        protected virtual void Awake()
        {
            EquipmentAnimator = GetComponent<Animator>();
            m_CrossHairDisplayer = FindObjectOfType<UI.Player.CrossHairDisplayer>();

            m_WaitEquipingTime = new WaitForSeconds(0.35f);

            Transform rootTransform = transform.root;
            Transform parentTransform = transform.parent;

            PlayerInputController = rootTransform.GetComponent<Controller.PlayerInputController>();

            AudioSource = parentTransform.GetComponent<AudioSource>();
            WeaponManager = parentTransform.GetComponent<WeaponManager>();

            WeaponInfo = PlayerData.Inventory.WeaponInfo[(int)m_EquipingWeaponType];
            MainCamera = Camera.main;
        }

        public virtual void Init()
        {
            m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;

            gameObject.SetActive(true);
            DoAppearObject();
            AssignKeyAction();
            StartCoroutine(WaitEquip());
        }

        protected virtual void DoAppearObject()
        {
            m_ArmController.AppearArms(true);
        }

        protected virtual void AssignKeyAction()
        {
            PlayerInputController.MouseMovement += m_WeaponSway.Sway;
        }
        protected virtual void DischargeKeyAction()
        {
            PlayerInputController.MouseMovement -= m_WeaponSway.Sway;
        }

        public virtual void Dispose()
        {
            DischargeKeyAction();
        }

        public virtual async Task UnEquip()
        {
            DischargeKeyAction();
            IsUnequiping = true;
            PlayerData.PlayerState.SetWeaponChanging(true);
            m_ArmAnimator.SetTrigger("Unequip");
            EquipmentAnimator.SetTrigger("Unequip");
            AudioSource.PlayOneShot(m_WeaponSoundScriptable.unequipSound);
            WeaponManager.PlayerShakeController.ShakeAllTransform(ShakeType.Changing);

            await Task.Delay(m_UnequipingTime);

            gameObject.SetActive(false);
            IsUnequiping = false;
        }

        private IEnumerator WaitEquip()
        {
            IsEquiping = true;
            m_ArmAnimator.SetTrigger("Equip");
            EquipmentAnimator.SetTrigger("Equip");
            AudioSource.PlayOneShot(m_WeaponSoundScriptable.equipSound);
            m_CrossHairDisplayer.SetCrossHair((int)m_WeaponStatScriptable.m_DefaultCrossHair);

            yield return m_WaitEquipingTime;
            PlayerData.PlayerState.SetWeaponChanging(false);

            IsEquiping = false;
        }
    }
}

