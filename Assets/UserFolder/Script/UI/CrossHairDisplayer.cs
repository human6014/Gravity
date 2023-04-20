using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scriptable;

namespace UI.Player
{
    public class CrossHairDisplayer : MonoBehaviour
    {
        [SerializeField] private CrossHairScripatble[] crossHairInfo;

        private CrossHairScripatble m_CurrentCrossHairScripatble;
        private PlayerState m_PlayerState;
        private Animator m_Animator;
        private Image[] crossHairImage = new Image[5];

        #region State string
        private const string m_RunState = "IsRunning";
        private const string m_WalkState = "IsWalking";
        private const string m_IdleState = "IsIdle";
        private const string m_CrouchState = "IsCrouching";
        private const string m_JumpState = "IsJumping";
        private const string m_AimState = "IsAiming";
        private const string m_CrouchFire = "CrouchFire";
        private const string m_IdleFire = "IdleFire";
        private const string m_WalkFire = "WalkFire";
        private const string m_JumpFire = "JumpFire";
        #endregion
        private string m_CurrentAnimState;
        public CrossHairScripatble GetCrossHairInfo(int index) => crossHairInfo[index];

        private void Awake()
        {
            m_PlayerState = FindObjectOfType<Contoller.Player.FirstPersonController>().m_PlayerState;
            m_Animator = GetComponent<Animator>();
            crossHairImage = GetComponentsInChildren<Image>();
        }

        /// <summary>
        /// 총기 종류에 따라 크로스 해어 설정
        /// </summary>
        /// <param name="index">0 : 없음, 1: 점, 2: 십자선, 3 : 원형</param>
        public void SetCrossHair(int index)
        {
            m_CurrentCrossHairScripatble = crossHairInfo[index];
            m_Animator.runtimeAnimatorController = m_CurrentCrossHairScripatble.m_AnimatorController;

            for (int i = 0; i < crossHairImage.Length; i++)
            {
                if (m_CurrentCrossHairScripatble.crossHairSprite[i] == null)
                {
                    crossHairImage[i].enabled = false;
                    continue;
                }
                crossHairImage[i].sprite = m_CurrentCrossHairScripatble.crossHairSprite[i];
                crossHairImage[i].enabled = true;
            }
        }

        private void FixedUpdate() => SetBehaviorState();

        private void SetBehaviorState()
        {
            if (m_PlayerState.PlayerWeaponState == PlayerWeaponState.Aiming) CrossHairSetBool(m_AimState);
            else if (m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Crouching)
            {
                if (m_PlayerState.PlayerWeaponState == PlayerWeaponState.Firing) CrossHairSetTrigger(m_CrouchFire);
                else CrossHairSetBool(m_CrouchState);
            }
            else if (m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Idle)
            {
                if (m_PlayerState.PlayerWeaponState == PlayerWeaponState.Firing) CrossHairSetTrigger(m_IdleFire);
                else CrossHairSetBool(m_IdleState);
            }
            else if (m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Walking)
            {
                if (m_PlayerState.PlayerWeaponState == PlayerWeaponState.Firing) CrossHairSetTrigger(m_WalkFire);
                else CrossHairSetBool(m_WalkState);
            }
            else if (m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Jumping)
            {
                if (m_PlayerState.PlayerWeaponState == PlayerWeaponState.Firing) CrossHairSetTrigger(m_JumpFire);
                else CrossHairSetBool(m_JumpState);
            }
            else if (m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running) CrossHairSetBool(m_RunState);
        }

        public void CrossHairSetTrigger(string state)
        {
            m_Animator.SetTrigger(state);
            m_PlayerState.SetBack();
        }

        public void CrossHairSetBool(string state)
        {
            if (state == m_CurrentAnimState) return;
            m_Animator.SetBool(m_CurrentAnimState, false);
            if (state != m_IdleState) m_Animator.SetBool(state, true);
            m_CurrentAnimState = state;
        }

        public float GetCurrentAccurancy()
        {
            float currentAccurancy = 0;
            switch (m_PlayerState.PlayerBehaviorState)
            {
                case PlayerBehaviorState.Crouching:
                    currentAccurancy = m_CurrentCrossHairScripatble.m_CrouchAccurancy;
                    break;
                case PlayerBehaviorState.Jumping:
                    currentAccurancy = m_CurrentCrossHairScripatble.m_JumpAccuracy;
                    break;
                case PlayerBehaviorState.Walking:
                    currentAccurancy = m_CurrentCrossHairScripatble.m_WalkAccuracy;
                    break;
                case PlayerBehaviorState.Idle:
                    currentAccurancy = m_CurrentCrossHairScripatble.m_IdleAccuracy;
                    break;
            }

            return currentAccurancy;
        }
    }
}
