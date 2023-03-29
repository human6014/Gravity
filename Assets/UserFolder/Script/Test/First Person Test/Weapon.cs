using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Contoller.Player;

namespace Test
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] protected PlayerInputController m_PlayerInputController;
        [SerializeField] protected FirstPersonController m_FirstPersonController;

        //ũ�ν� ��� �ѱ⺰�� ����
        [SerializeField] protected CrossHairController m_CrossHairController;   //�ѱ⺰ ũ�ν� ��� ������ ���� UI���� ��ũ��Ʈ

        //�ִϸ��̼�
        protected Animator m_EquipmentAnimator; //���� �ڽ��� ���� ���ϸ�����
        [SerializeField] protected Animator m_ArmAnimator; //�� �ִϸ�����
        [SerializeField] protected AnimatorOverrideController m_EquipmentOverrideController = null; // ����� ���� �ִϸ��̼ǵ�
        [SerializeField] protected AnimatorOverrideController m_ArmOverrideController = null;   // ����� �� �ִϸ��̼ǵ�

        [SerializeField] protected LayerMask m_AttackableLayer;     //�� �ǰ� ���̾�

        //�߻� + �ǰ� �Ҹ�
        [SerializeField] protected AudioSource m_AudioSource;               //�Ҹ� ���� ���� AudioSource 
        [SerializeField] protected Scriptable.RangeWeaponSoundScriptable m_WeaponSound;  //���� �Ҹ��� ���� ��ũ���ͺ�
        public virtual void Init()
        {
            m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;

            m_EquipmentAnimator.SetTrigger("Equip");
            m_ArmAnimator.SetTrigger("Equip");
        }

        public virtual void Dispose()
        {
            m_ArmAnimator.SetTrigger("Unequip");
            m_EquipmentAnimator.SetTrigger("Unequip");
        }
    }
}
