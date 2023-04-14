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
        [SerializeField] protected int ItemIndex;   //������ ������ȣ

        [Header("Equiping Type")]
        [SerializeField] private EquipingWeaponType m_EquipingWeaponType;

        [Header("Weapon Animation")]
        [SerializeField] protected Animator m_ArmAnimator; //�� �ִϸ�����
        [SerializeField] protected AnimatorOverrideController m_ArmOverrideController = null;   // ����� �� �ִϸ��̼ǵ�

        private WaitForSeconds m_WaitEquipingTime;
        private WaitForSeconds m_WaitUnequipingTime;
        protected PlayerState m_PlayerState;
        protected bool m_IsEquip;
        /// <summary>
        /// ������� �Է��� �޴� ��ũ��Ʈ
        /// </summary>
        public PlayerInputController m_PlayerInputController { get; private set; }

        /// <summary>
        /// �ѱ⺰ ũ�ν� ��� ������ ���� UI���� ��ũ��Ʈ
        /// </summary>
        protected CrossHairController m_CrossHairController { get; private set; }

        /// <summary>
        /// �ǰݴ��� ǥ���� Material�� �������� ������ �Ǻ����ִ� ��ũ��Ʈ
        /// </summary>
        protected SurfaceManager m_SurfaceManager { get; private set; }

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
