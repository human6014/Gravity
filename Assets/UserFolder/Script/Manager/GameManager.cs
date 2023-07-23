using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UI.Manager;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SkillEventManager m_SkillEventManager;

        
        [SerializeField] private float m_SkillEventTiming = 30; 
        [SerializeField] private int m_FrameRate = 60;
        //스킬 발동 조건
        //유닛 능력치 업 + 소환 주기 제어 명령
        //특수몹 생성 조건 + 명령

        public static bool IsGameEnd { get; private set; }
        private float EventTimer { get; set; }

        public static void GameClear()
        {
            if (IsGameEnd) return;
            IsGameEnd = true;
            Debug.Log("GameClear");
        }
        public static void GameEnd()
        {
            if (IsGameEnd) return;
            IsGameEnd = true;
            Debug.Log("GameEnd");
        }

        private void Awake()
        {
            IsGameEnd = false;

            Application.targetFrameRate = m_FrameRate;
        }

        private void Update()
        {
            EventTimer += Time.deltaTime;

            if (EventTimer >= m_SkillEventTiming)
            {
                EventTimer = 0;
                m_SkillEventManager.OccurSkillEvent();
            }
        }
    }
}
