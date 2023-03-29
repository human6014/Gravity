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

        //크로스 헤어 총기별로 설정
        [SerializeField] protected CrossHairController m_CrossHairController;   //총기별 크로스 헤어 설정을 위한 UI관리 스크립트

        //애니메이션
        protected Animator m_EquipmentAnimator; //현재 자신의 무기 에니메이터
        [SerializeField] protected Animator m_ArmAnimator; //팔 애니메이터
        [SerializeField] protected AnimatorOverrideController m_EquipmentOverrideController = null; // 덮어씌울 무기 애니메이션들
        [SerializeField] protected AnimatorOverrideController m_ArmOverrideController = null;   // 덮어씌울 팔 애니메이션들

        [SerializeField] protected LayerMask m_AttackableLayer;     //총 피격 레이어

        //발사 + 피격 소리
        [SerializeField] protected AudioSource m_AudioSource;               //소리 내기 위한 AudioSource 
        [SerializeField] protected Scriptable.RangeWeaponSoundScriptable m_WeaponSound;  //각종 소리를 담은 스크립터블
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
