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
    public class Weapon : MonoBehaviour
    {        
        //[SerializeField] private Scriptable.WeaponStatScriptable m_WeaponStatScriptable;
        //[SerializeField] private Scriptable.RangeWeaponStatScriptable m_RangeWeaponStatScriptable;

        [Header("Parent")]
        [Header("Index")]
        [SerializeField] protected int ItemIndex;   //아이템 고유번호

        [Header("Equiping Type")]
        [SerializeField] private EquipingWeaponType m_EquipingWeaponType;

        [Header("Weapon Animation")]
        [SerializeField] protected Animator m_ArmAnimator; //팔 애니메이터
        [SerializeField] protected AnimatorOverrideController m_ArmOverrideController = null;   // 덮어씌울 팔 애니메이션들

        private WaitForSeconds m_WaitEquipingTime;
        private WaitForSeconds m_WaitUnequipingTime;
        protected PlayerState m_PlayerState;
        protected bool m_IsEquip;
        /// <summary>
        /// 사용자의 입력을 받는 스크립트
        /// </summary>
        public PlayerInputController m_PlayerInputController { get; private set; }

        /// <summary>
        /// 총기별 크로스 헤어 설정을 위한 UI관리 스크립트
        /// </summary>
        protected CrossHairController m_CrossHairController { get; private set; }

        /// <summary>
        /// 피격당한 표면의 Material을 종류별로 가지고 판별해주는 스크립트
        /// </summary>
        protected SurfaceManager m_SurfaceManager { get; private set; }

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

        public int GetEquipingType() => (int)m_EquipingWeaponType;
        public int GetItemIndex() => ItemIndex;

        public bool IsEquiping { get; private set; }
        public bool IsUnequiping { get; private set; }
        public virtual bool IsFiring { get; private set; }
        public virtual bool IsReloading { get; private set; }

        protected virtual void Awake()
        {
            m_EquipmentAnimator = GetComponent<Animator>();

            //((Scriptable.RangeWeaponStatScriptable)m_WeaponStatScriptable).m_Damage = 5;
            //Debug.Log(((Scriptable.RangeWeaponStatScriptable)m_WeaponStatScriptable).m_Damage);
            m_CrossHairController = FindObjectOfType<CrossHairController>();

            m_WaitEquipingTime = new WaitForSeconds(0.35f);
            m_WaitUnequipingTime = new WaitForSeconds(0.55f);

            Transform rootTransform = transform.root;
            m_PlayerInputController = rootTransform.GetComponent<PlayerInputController>();
            m_PlayerState = rootTransform.GetComponent<FirstPersonController>().m_PlayerState;
 
            m_AudioSource = transform.parent.GetComponent<AudioSource>();
            m_WeaponManager = transform.parent.GetComponent<WeaponManager>();

            m_SurfaceManager = FindObjectOfType<SurfaceManager>();
        }

        public virtual void Init()
        {
            m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;

            gameObject.SetActive(true);
            StartCoroutine(WaitEquip());
        }

        public virtual void Dispose()
        {
            StartCoroutine(WaitUnequip());
        }

        private IEnumerator WaitEquip()
        {
            IsEquiping = true;
            
            m_ArmAnimator.SetTrigger("Equip");
            m_EquipmentAnimator.SetTrigger("Equip");

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

            yield return m_WaitUnequipingTime;

            gameObject.SetActive(false);
            IsUnequiping = false;
            m_WeaponManager.ChangeWeapon();
        }
    }
}
