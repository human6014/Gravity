using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private UI.Manager.SkillEventManager m_SkillEventManager;

        [SerializeField] private float m_SP1SpawnTiming = 10;
        [SerializeField] private float m_SkillEventTiming = 30; 
        //스킬 발동 조건
        //유닛 능력치 업 + 소환 주기 제어 명령
        //특수몹 생성 조건 + 명령

        private SpawnManager m_SpawnManager;

        private float m_GameTime;   //Pause 걸면 Stop
        private float m_SkillEventTimer;
        private const float m_SkillEventWaitTime = 3;
        public static bool IsGameEnd { get; private set; } = false;

        private void Awake()
        {
            m_SpawnManager = GetComponent<SpawnManager>();
        }

        public static void GameEnd()
        {
            Debug.Log("GameEnd");
        }

        private void Update()
        {
            m_GameTime += Time.deltaTime;
            if(!m_SkillEventManager.IsOnEvent) m_SkillEventTimer += Time.deltaTime;

            if (!m_SpawnManager.IsSP1MonsterSpawned && m_GameTime >= m_SP1SpawnTiming) m_SpawnManager.SpawnSpecialMonster1();

            if (m_SkillEventTimer >= m_SkillEventTiming)
            {
                m_SkillEventTimer = 0;
                m_SkillEventManager.OccurSkillEvent();
            }
        }
    }
}
