using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Contoller.Player;
using Manager;

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

        private WaitForSeconds m_WaitChangeEquipmentTime;
        protected bool m_IsEquip;
        /// <summary>
        /// ������� �Է��� �޴� ��ũ��Ʈ
        /// </summary>
        public PlayerInputController m_PlayerInputController { get; private set; }

        /// <summary>
        /// �÷��̾� ���� �ൿ�� �����ϴ� ��ũ��Ʈ
        /// </summary>
        protected FirstPersonController m_FirstPersonController { get; private set; }

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
        public virtual bool IsUnequiping() => false;
        public virtual bool IsEquiping() => false;
        public virtual bool IsFiring() => false;
        public virtual bool IsReloading() => false;

        protected virtual void Awake()
        {
            m_EquipmentAnimator = GetComponent<Animator>();

            //((Scriptable.RangeWeaponStatScriptable)m_WeaponStatScriptable).m_Damage = 5;
            //Debug.Log(((Scriptable.RangeWeaponStatScriptable)m_WeaponStatScriptable).m_Damage);

            m_WaitChangeEquipmentTime = new WaitForSeconds(0.5f);

            Transform rootTransform = transform.root;
            m_PlayerInputController = rootTransform.GetComponent<PlayerInputController>();
            m_FirstPersonController = rootTransform.GetComponent<FirstPersonController>();
 
            m_AudioSource = transform.parent.GetComponent<AudioSource>();
            m_WeaponManager = transform.parent.GetComponent<WeaponManager>();

            m_SurfaceManager = FindObjectOfType<SurfaceManager>();
            m_CrossHairController = FindObjectOfType<CrossHairController>();
        }

        public virtual void Init()
        {
            m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;
            
            gameObject.SetActive(true);

            m_ArmAnimator.SetTrigger("Equip");
            m_EquipmentAnimator.SetTrigger("Equip");

            m_IsEquip = true;
        }

        public virtual void Dispose()
        {
            m_IsEquip = false; 

            m_ArmAnimator.SetTrigger("Unequip");
            m_EquipmentAnimator.SetTrigger("Unequip");

            gameObject.SetActive(false);
        }

        private IEnumerator WaitChangeEquipment()
        {
            yield return new WaitForSeconds(0.5f);
        }
    }
}
