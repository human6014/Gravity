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

        private CrossHairController m_CrossHairController;
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

        public int GetEquipingType() => (int)m_EquipingWeaponType;
        public int GetItemIndex() => ItemIndex;

        public bool IsEquiping { get; private set; }
        public bool IsUnequiping { get; private set; }
        public virtual bool IsFiring { get; private set; }
        public virtual bool IsReloading { get; private set; }

        protected virtual void Awake()
        {
            m_EquipmentAnimator = GetComponent<Animator>();

            m_CrossHairController = FindObjectOfType<CrossHairController>();

            m_WaitEquipingTime = new WaitForSeconds(0.35f);
            m_WaitUnequipingTime = new WaitForSeconds(0.55f);

            Transform rootTransform = transform.root;
            m_PlayerInputController = rootTransform.GetComponent<PlayerInputController>();
            m_PlayerState = rootTransform.GetComponent<FirstPersonController>().m_PlayerState;
 
            m_AudioSource = transform.parent.GetComponent<AudioSource>();
            m_WeaponManager = transform.parent.GetComponent<WeaponManager>();
        }

        public virtual void Init()
        {
            m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;

            gameObject.SetActive(true);
            AssignKeyAction();
            StartCoroutine(WaitEquip());
        }

        protected abstract void AssignKeyAction();
        protected abstract void DischargeKeyAction();

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
            m_CrossHairController.SetCrossHair((int)m_WeaponStatScriptable.m_DefaultCrossHair);

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
