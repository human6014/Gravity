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
        //��ų �ߵ� ����
        //���� �ɷ�ġ �� + ��ȯ �ֱ� ���� ���
        //Ư���� ���� ���� + ���

        private SpawnManager m_SpawnManager;

        private float m_GameTime;

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

            if (!m_SpawnManager.IsSP1MonsterSpawned && m_GameTime >= m_SP1SpawnTiming) m_SpawnManager.SpawnSpecialMonster1();
        }
    }
}
