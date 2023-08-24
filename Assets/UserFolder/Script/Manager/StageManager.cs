using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class StageManager : MonoBehaviour
    {
        [System.Serializable]
        public class StageInfo
        {
            [Tooltip("���� ������������ Ư�� ���Ͱ� �����ϴ� Wave")]
            public int m_SpawnSpecialWave;

            [Tooltip("���� Wave�� �Ѿ�� ���� �ð�")]
            public float[] m_WaveTiming;

            public float GetWaveTime(int startWave, int endWave)
            {
                if (endWave > m_WaveTiming.Length) return -1;
                float sum = 0;
                for (int i = startWave; i < endWave; i++)
                    sum += m_WaveTiming[i];
                return sum;
            }
        }

        [Tooltip("��������, ���̺� ����")]
        [SerializeField] private StageInfo[] m_StageInfo;

        [Tooltip("���� ���̺� �ð�")]
        [SerializeField] private float m_InfintyWaveTiming;

        private float m_WaveTimer;
        private float m_StatMultiplier = 0;

        private int m_CurrentStage;
        private int m_CurrentWave;

        public int CurrentStage
        {
            get => m_CurrentStage;
            set
            {
                m_CurrentStage = value;
                m_CurrentWave = 1;
                m_StatMultiplier = (m_CurrentStage - 1) + ((m_CurrentWave - 1) * 0.5f);
                m_WaveTimer = 0;
            }
        }

        public int CurrentWave
        {
            get => m_CurrentWave;
            set
            {
                m_CurrentWave = value;
                m_StatMultiplier = (m_CurrentStage - 1) + ((m_CurrentWave - 1) * 0.5f);
                m_WaveTimer = 0;
            }
        }

        private void StageWaveCheck()
        {
            m_WaveTimer += Time.deltaTime;

            if (CurrentStage - 1 >= m_StageInfo.Length)
            {
                //if (IsSP3MonsterEnd) GameManager.GameClear();
                //else if (m_InfintyWaveTiming <= m_WaveTimer) CurrentWave++;
                return;
            }
            StageInfo currentStageInfo = m_StageInfo[CurrentStage - 1];
            if (currentStageInfo.m_WaveTiming[CurrentWave - 1] <= m_WaveTimer)
            {
                CurrentWave++;
                //if (CurrentWave - 1 == currentStageInfo.m_SpawnSpecialWave - 1) SpawnSpecialMonster();
                if (currentStageInfo.m_WaveTiming.Length <= CurrentWave - 1) CurrentStage++;
            }
        }

        public float GetStageTime(int startStage, int endStage, int startWave, int endWave)
        {
            float sum = 0;
            float curSum;
            int begin, end;
            for (int i = startStage; i < endStage; i++)
            {
                if (i == startStage) begin = startWave;
                else begin = 0;

                if (i == endStage - 1) end = endWave;
                else end = m_StageInfo[i].m_WaveTiming.Length;

                curSum = m_StageInfo[i].GetWaveTime(begin, end);

                if (curSum == -1) return -1;
                sum += curSum;

            }
            return sum;
        }
    }
}
