using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit.Normal
{
    public enum NormalMonsterBehaviorState
    {
        Idle,
        Walking,
        Running,
        Crawling,
        Attacking,
        GettingUp
    }
    public class NormalMonsterState
    {
        private readonly NormalMonsterAnimController m_NormalMonsterAnimController;

        #region Property
        public NormalMonsterBehaviorState BehaviorState { get; private set; }
        private NormalMonsterBehaviorState BeforeBehaviorState { get; set; }
        public bool CanAttackState 
        {
            get => BehaviorState != NormalMonsterBehaviorState.Crawling &&
                   BehaviorState != NormalMonsterBehaviorState.GettingUp;
        }

        public bool CanMoveState
        {
            get => BehaviorState != NormalMonsterBehaviorState.Attacking &&
                   BehaviorState != NormalMonsterBehaviorState.GettingUp;
        }
        #endregion
        public NormalMonsterState(NormalMonsterAnimController normalMonsterAnimController)
        {
            m_NormalMonsterAnimController = normalMonsterAnimController;
            Init();
        }

        public void Init()
        {
            BehaviorState = NormalMonsterBehaviorState.Idle;
        }

        #region Bool
        public void SetBoolIdle()
        {
            BehaviorState = NormalMonsterBehaviorState.Idle;
            m_NormalMonsterAnimController.PlayIdle();
        }

        public void SetBoolWalking()
        {
            if (CanMoveState)
            {
                BehaviorState = NormalMonsterBehaviorState.Walking;
                m_NormalMonsterAnimController.PlayWalk(true);
            }
        }

        public void SetBoolRunning()
        {
            if (CanMoveState)
            {
                BehaviorState = NormalMonsterBehaviorState.Running;
                m_NormalMonsterAnimController.PlayRun(true);
            }
        }

        public void SetBoolCrawling()
        {
            if (CanMoveState)
            {
                BehaviorState = NormalMonsterBehaviorState.Crawling;
                m_NormalMonsterAnimController.PlayCrawl(true);
            }
        }
        #endregion
        #region Trigger
        public async void SetTriggerAttacking()
        {
            //조건검사 호출하기 전에 함
            BeforeBehaviorState = BehaviorState;
            BehaviorState = NormalMonsterBehaviorState.Attacking;
            await m_NormalMonsterAnimController.PlayAttack();
            BehaviorState = BeforeBehaviorState;
        }

        public async void SetTriggerGettingUp()
        {
            if (BehaviorState == NormalMonsterBehaviorState.Crawling ||
                BehaviorState == NormalMonsterBehaviorState.Idle)
            {
                BeforeBehaviorState = BehaviorState;
                BehaviorState = NormalMonsterBehaviorState.GettingUp;
                await m_NormalMonsterAnimController.PlayGettingUp();
                BehaviorState = BeforeBehaviorState;
            }
        }
        #endregion
    }
}
