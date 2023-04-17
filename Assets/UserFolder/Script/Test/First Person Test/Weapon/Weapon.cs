using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Contoller.Player;
using Manager;
using System.Threading.Tasks;
using System.Threading;

namespace Test
{
    public enum EquipingWeaponType
    {
        Melee,
        Pistol,
        MainRifle,
        SubRifle,
        Throwing
    }
    public abstract class Weapon : MonoBehaviour
    {
        [Header("Parent")]
        [Header("Index")]
        [SerializeField] protected int ItemIndex;   //������ ������ȣ

        [Header("Equiping Type")]
        [SerializeField] private EquipingWeaponType m_EquipingWeaponType;

        [Header("Weapon Animation")]
        [SerializeField] protected Animator m_ArmAnimator; //�� �ִϸ�����
        [SerializeField] protected AnimatorOverrideController m_ArmOverrideController = null;   // ����� �� �ִϸ��̼ǵ�

        [Header("Scriptable")]
        [SerializeField] protected Scriptable.WeaponStatScriptable m_WeaponStatScriptable;
        [SerializeField] protected Scriptable.WeaponSoundScriptable m_WeaponSoundScriptable;

        [Header("Pos Change")]
        [SerializeField] private Transform m_Pivot;                 //��ġ ������ �θ� ������Ʈ

        [Header("Weapon Sway")]
        [SerializeField] private WeaponSway m_WeaponSway;

        private UI.Game.CrossHairDisplayer m_CrossHairDisplayer;
        
        private WaitForSeconds m_WaitEquipingTime;
        private WaitForSeconds m_WaitUnequipingTime;


        protected PlayerState m_PlayerState { get; private set; }

        /// <summary>
        /// ������� �Է��� �޴� ��ũ��Ʈ
        /// </summary>
        protected PlayerInputController m_PlayerInputController { get; private set; }        

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

        protected Transform PivotTransform { get => m_Pivot; }

        public bool IsEquiping { get; private set; }
        public bool IsUnequiping { get; private set; }
        //public virtual bool IsFiring { get; private set; }
        //public virtual bool IsReloading { get; private set; }

        public virtual bool CanChangeWeapon { get => !IsEquiping && !IsUnequiping; }

        public int GetEquipingType() => (int)m_EquipingWeaponType;
        public int GetItemIndex() => ItemIndex;

        protected virtual void Awake()
        {
            m_EquipmentAnimator = GetComponent<Animator>();
            m_CrossHairDisplayer = FindObjectOfType<UI.Game.CrossHairDisplayer>();
            
            m_WaitEquipingTime = new WaitForSeconds(0.35f);
            m_WaitUnequipingTime = new WaitForSeconds(0.55f);

            Transform rootTransform = transform.root;
            m_PlayerInputController = rootTransform.GetComponent<PlayerInputController>();
            m_PlayerState = rootTransform.GetComponent<FirstPersonController>().m_PlayerState;
 
            m_AudioSource = transform.parent.GetComponent<AudioSource>();
            m_WeaponManager = transform.parent.GetComponent<WeaponManager>();

            m_MainCamera = Camera.main;
        }

        public void Init()
        {
            m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;

            gameObject.SetActive(true);
            AssignKeyAction();
            StartCoroutine(WaitEquip());
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
            StartCoroutine(WaitUnequip());
        }

        private IEnumerator WaitEquip()
        {
            IsEquiping = true;
            m_ArmAnimator.SetTrigger("Equip");
            m_EquipmentAnimator.SetTrigger("Equip");
            m_AudioSource.PlayOneShot(m_WeaponSoundScriptable.equipSound);
            m_CrossHairDisplayer.SetCrossHair((int)m_WeaponStatScriptable.m_DefaultCrossHair);

            yield return m_WaitEquipingTime;

            m_PlayerState.SetWeaponChanging(false);
            IsEquiping = false;
        }

        private IEnumerator WaitUnequip()
        {
            IsUnequiping = true;
            m_PlayerState.SetWeaponChanging(true);
            m_ArmAnimator.SetTrigger("Unequip");
            m_EquipmentAnimator.SetTrigger("Unequip");
            m_AudioSource.PlayOneShot(m_WeaponSoundScriptable.unequipSound);

            yield return m_WaitUnequipingTime;

            gameObject.SetActive(false);
            IsUnequiping = false;
            m_WeaponManager.ChangeWeapon();
        }
    }
}
