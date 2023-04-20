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
        [SerializeField] protected int ItemIndex;   //아이템 고유번호 -> 이름 바꾸자

        [Header("Equiping Type")]
        [SerializeField] protected EquipingWeaponType m_EquipingWeaponType;

        [Header("Weapon Animation")]
        [SerializeField] protected Animator m_ArmAnimator; //팔 애니메이터
        [SerializeField] protected AnimatorOverrideController m_ArmOverrideController = null;   // 덮어씌울 팔 애니메이션들

        [Header("Scriptable")]
        [SerializeField] protected Scriptable.WeaponStatScriptable m_WeaponStatScriptable;
        [SerializeField] protected Scriptable.WeaponSoundScriptable m_WeaponSoundScriptable;

        [Header("Pos Change")]
        [SerializeField] private Transform m_Pivot;                 //위치 조정용 부모 오브젝트

        [Header("Weapon Sway")]
        [SerializeField] private WeaponSway m_WeaponSway;

        [Header("Player Data")]
        [SerializeField] protected PlayerData m_PlayerData;

        private UI.Player.CrossHairDisplayer m_CrossHairDisplayer;
        
        private WaitForSeconds m_WaitEquipingTime;
        private WaitForSeconds m_WaitUnequipingTime;

        protected WeaponInfo m_WeaponInfo { get; private set; }

        protected PlayerState m_PlayerState { get; private set; }

        /// <summary>
        /// 사용자의 입력을 받는 스크립트
        /// </summary>
        protected PlayerInputController m_PlayerInputController { get; private set; }        

        /// <summary>
        /// 현재 자신의 무기 에니메이터
        /// </summary>
        protected Animator m_EquipmentAnimator { get; private set; } 

        /// <summary>
        /// 모든 Equipment의 공통 AudioSource
        /// </summary>
        protected AudioSource m_AudioSource { get; private set; }

        /// <summary>
        /// Weapon들을 관리하는 매니저 클래스
        /// </summary>
        protected WeaponManager m_WeaponManager { get; private set; }

        /// <summary>
        /// 메인 카메라
        /// </summary>
        protected Camera m_MainCamera { get; private set; }

        protected Transform PivotTransform { get => m_Pivot; }

        public Sprite WeaponIcon { get => m_WeaponStatScriptable.m_WeaponIcon; }
        
        public bool IsEquiping { get; private set; }
        public bool IsUnequiping { get; private set; }
        //public virtual bool IsFiring { get; private set; }
        //public virtual bool IsReloading { get; private set; }

        public virtual bool CanChangeWeapon { get => !IsEquiping && !IsUnequiping; }

        public BulletType m_BulletType { get => m_WeaponStatScriptable.m_BulletType; }
        public int EquipingType { get => (int)m_EquipingWeaponType; }
        public int GetItemIndex { get => ItemIndex; }

        protected virtual void Awake()
        {
            m_EquipmentAnimator = GetComponent<Animator>();
            m_CrossHairDisplayer = FindObjectOfType<UI.Player.CrossHairDisplayer>();
            
            m_WaitEquipingTime = new WaitForSeconds(0.35f);
            m_WaitUnequipingTime = new WaitForSeconds(0.55f);

            Transform rootTransform = transform.root;
            m_PlayerInputController = rootTransform.GetComponent<PlayerInputController>();
            m_PlayerState = rootTransform.GetComponent<FirstPersonController>().m_PlayerState;
 
            m_AudioSource = transform.parent.GetComponent<AudioSource>();
            m_WeaponManager = transform.parent.GetComponent<WeaponManager>();
            m_WeaponInfo = m_PlayerData.GetInventory().WeaponInfo[(int)m_EquipingWeaponType];
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
