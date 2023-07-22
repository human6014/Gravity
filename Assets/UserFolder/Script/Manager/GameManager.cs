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
        //��ų �ߵ� ����
        //���� �ɷ�ġ �� + ��ȯ �ֱ� ���� ���
        //Ư���� ���� ���� + ���

        private SpawnManager m_SpawnManager;
        private SettingUIManager m_SettingUIManager;

        
        public static bool IsGameEnd { get; private set; } = false;
        private float EventTimer { get; set; }
        private float SpecialSpawnTimer { get; set; }

        public static void GameEnd()
        {
            IsGameEnd = true;
            Debug.Log("GameEnd");
        }

        private void Awake()
        {
            IsGameEnd = false;
            m_SpawnManager = GetComponent<SpawnManager>();
            m_SettingUIManager = FindObjectOfType<SettingUIManager>();

            Application.targetFrameRate = m_FrameRate;
        }

        private void Update()
        {
            if (m_SettingUIManager.IsActivePauseUI) return;
            if (m_SettingUIManager.IsActiveSkillEventUI) return;
            EventTimer += Time.deltaTime;

            if (EventTimer >= m_SkillEventTiming)
            {
                EventTimer = 0;
                m_SkillEventManager.OccurSkillEvent();
            }
        }
    }
}
