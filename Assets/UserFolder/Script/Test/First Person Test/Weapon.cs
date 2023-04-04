using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Contoller.Player;
using Manager;

namespace Test
{
    public class Weapon : MonoBehaviour
    {
        /// <summary>
        /// ������� �Է��� �޴� ��ũ��Ʈ
        /// </summary>
        protected PlayerInputController m_PlayerInputController { get; private set; }

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

        public int GetItemIndex() => ItemIndex;

        [Header("Parent")]
        [Header("Index")]
        [SerializeField] protected int ItemIndex;   //������ ������ȣ

        [Header("Weapon Animation")]
        [SerializeField] protected Animator m_ArmAnimator; //�� �ִϸ�����
        [SerializeField] protected AnimatorOverrideController m_ArmOverrideController = null;   // ����� �� �ִϸ��̼ǵ�

        [Header("Weapon Attack Layer")]
        [SerializeField] protected LayerMask m_AttackableLayer;     //�� �ǰ� ���̾�

        [Header("Weapon Sound")]
        

        protected bool m_IsEquip;

        protected virtual void Awake()
        {
            m_EquipmentAnimator = GetComponent<Animator>();

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
            gameObject.SetActive(true);

            m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;

            m_EquipmentAnimator.SetTrigger("Equip");
            m_ArmAnimator.SetTrigger("Equip");

            m_IsEquip = true;
        }

        public virtual void Dispose()
        {
            m_IsEquip = false; 

            m_ArmAnimator.SetTrigger("Unequip");
            m_EquipmentAnimator.SetTrigger("Unequip");

            gameObject.SetActive(false);
        }
    }
}
