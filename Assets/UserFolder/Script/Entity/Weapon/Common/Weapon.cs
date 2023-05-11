using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.Weapon;
using System.Threading.Tasks;
using Scriptable.Equipment;
using Contoller.Util;

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
        [SerializeField] protected Animator m_ArmAnimator; //�� �ִϸ�����
        [SerializeField] protected AnimatorOverrideController m_ArmOverrideController = null;   // ����� �� �ִϸ��̼ǵ�

        [Header("Scriptable")]
        [SerializeField] protected WeaponStatScriptable m_WeaponStatScriptable;
        [SerializeField] protected WeaponSoundScriptable m_WeaponSoundScriptable;

        [Header("Pos Change")]
        [SerializeField] protected Transform m_Pivot;                 //��ġ ������ �θ� ������Ʈ

        [Header("Weapon Sway")]
        [SerializeField] private WeaponSway m_WeaponSway;

        [Header("Arm")]
        [SerializeField] protected ArmController m_ArmController;

        #endregion


        private UI.Player.CrossHairDisplayer m_CrossHairDisplayer;
        private WaitForSeconds m_WaitEquipingTime;
        private const int m_UnequipingTime = 550;


        #region Property
        protected PlayerData m_PlayerData { get; private set; }

        /// <summary>
        /// �κ��丮�� Weapon���� ��� Ŭ����
        /// </summary>
        protected WeaponInfo m_WeaponInfo { get; private set; }

        /// <summary>
        /// ������� �Է��� �޴� ��ũ��Ʈ
        /// </summary>
        protected Contoller.PlayerInputController m_PlayerInputController { get; private set; }

        /// <summary>
        /// ���� �ڽ��� ���� ���ϸ�����
        /// </summary>
        protected Animator m_EquipmentAnimator { get; private set; }

        /// <summary>
        /// ��� Equipment�� ���� AudioSource
        /// </summary>
        protected AudioSource m_AudioSource { get; private set; }

        /// <summary>
        /// Weapon���� �����ϴ� �Ŵ��� Ŭ����
        /// </summary>
        protected WeaponManager m_WeaponManager { get; private set; }

        /// <summary>
        /// ���� ī�޶�
        /// </summary>
        protected Camera m_MainCamera { get; private set; }

        /// <summary>
        /// ���� ������
        /// </summary>
        public Sprite WeaponIcon { get => m_WeaponStatScriptable.m_WeaponIcon; }


        public bool IsEquiping { get; private set; }
        public bool IsUnequiping { get; private set; }
        public virtual bool CanChangeWeapon { get => !IsEquiping && !IsUnequiping; }

        public FireMode m_CurrentFireMode { get; protected set; } = FireMode.None;
        //�̰� �̻��ѵ�?
        public AttackType m_BulletType { get => m_WeaponStatScriptable.m_BulletType; }

        public int EquipingType { get => (int)m_EquipingWeaponType; }
        public int GetItemIndex { get => ItemIndex; }

        #endregion

        protected virtual void Awake()
        {
            m_EquipmentAnimator = GetComponent<Animator>();
            m_CrossHairDisplayer = FindObjectOfType<UI.Player.CrossHairDisplayer>();

            m_WaitEquipingTime = new WaitForSeconds(0.35f);

            Transform rootTransform = transform.root;
            Transform parentTransform = transform.parent;

            m_PlayerInputController = rootTransform.GetComponent<Contoller.PlayerInputController>();
            m_PlayerData = rootTransform.GetComponent<PlayerData>();

            m_AudioSource = parentTransform.GetComponent<AudioSource>();
            m_WeaponManager = parentTransform.GetComponent<WeaponManager>();

            m_WeaponInfo = m_PlayerData.Inventory.WeaponInfo[(int)m_EquipingWeaponType];
            m_MainCamera = Camera.main;
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
            m_PlayerInputController.MouseMovement += m_WeaponSway.Sway;
        }
        protected virtual void DischargeKeyAction()
        {
            m_PlayerInputController.MouseMovement -= m_WeaponSway.Sway;
        }

        public virtual void Dispose()
        {
            DischargeKeyAction();
        }

        public virtual async Task UnEquip()
        {
            DischargeKeyAction();
            IsUnequiping = true;
            m_PlayerData.m_PlayerState.SetWeaponChanging(true);
            m_ArmAnimator.SetTrigger("Unequip");
            m_EquipmentAnimator.SetTrigger("Unequip");
            m_AudioSource.PlayOneShot(m_WeaponSoundScriptable.unequipSound);

            await Task.Delay(m_UnequipingTime);

            gameObject.SetActive(false);
            IsUnequiping = false;
        }

        private IEnumerator WaitEquip()
        {
            IsEquiping = true;
            m_ArmAnimator.SetTrigger("Equip");
            m_EquipmentAnimator.SetTrigger("Equip");
            m_AudioSource.PlayOneShot(m_WeaponSoundScriptable.equipSound);
            m_CrossHairDisplayer.SetCrossHair((int)m_WeaponStatScriptable.m_DefaultCrossHair);

            yield return m_WaitEquipingTime;
            m_PlayerData.m_PlayerState.SetWeaponChanging(false);

            IsEquiping = false;
        }
    }
}

