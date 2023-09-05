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

            [Tooltip("���� Stage | Wave ���� �� ���޵Ǵ� SkillPoint")]
            public int[] m_Point;

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
        [SerializeField] private float m_InfinityWaveTiming;

        [Tooltip("���� ���̺� �ð��� ������ ��ų ����Ʈ")]
        [SerializeField] private int m_InfinityWavePoint;

        [SerializeField] private UnityEngine.Events.UnityEvent<int> WaveChangeEvnet;

        private StageInfo m_CurrentStageInfo;

        private float m_WaveTimer;
        private float m_StatMultiplier = 0;

        private int m_CurrentStage;
        private int m_CurrentWave;

        #region Property
        public System.Action<int> SpawnSpecialAction { get; set; }

        private int CurrentStage
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

        private int CurrentWave
        {
            get => m_CurrentWave;
            set
            {
                m_CurrentWave = value;
                m_StatMultiplier = (m_CurrentStage - 1) + ((m_CurrentWave - 1) * 0.5f);
                m_WaveTimer = 0;
            }
        }

        public float StatMultiplier { get => m_StatMultiplier; }
        #endregion

        private void Awake()
        {
            CurrentStage = 1;
            m_CurrentStageInfo = m_StageInfo[CurrentStage - 1];
        }

        private void Update()
        {
            StageWaveCheck();
        }

        private void StageWaveCheck()
        {
            m_WaveTimer += Time.deltaTime;

            if (CurrentStage - 1 >= m_StageInfo.Length)
            {
                if (m_InfinityWaveTiming <= m_WaveTimer)
                {
                    CurrentWave++;
                    WaveChangeEvnet?.Invoke(m_InfinityWavePoint);
                }
                return;
            }

            if (m_CurrentStageInfo.m_WaveTiming[CurrentWave - 1] <= m_WaveTimer)
            {
                CurrentWave++;
                if (CurrentWave - 1 == m_CurrentStageInfo.m_SpawnSpecialWave - 1) SpawnSpecialAction?.Invoke(CurrentStage);
                if (m_CurrentStageInfo.m_WaveTiming.Length <= CurrentWave - 1)
                {
                    CurrentStage++;
                    if (CurrentStage - 1 < m_StageInfo.Length) 
                        m_CurrentStageInfo = m_StageInfo[CurrentStage - 1];
                }
                if (CurrentStage - 1 < m_StageInfo.Length)
                    WaveChangeEvnet?.Invoke(m_CurrentStageInfo.m_Point[m_CurrentWave - 1]);
            }
        }

        public float GetStageTime(int endStage, int endWave)
        {
            float sum = 0;
            float curSum;

            int startStage = CurrentStage - 1;
            int startWave = CurrentWave - 1;
            int begin, end;
            for (int i = startStage; i < endStage; i++)
            {
                begin = (i == startStage) ? startWave : 0;
                end = (i == endStage - 1) ? endWave : m_StageInfo[i].m_WaveTiming.Length;

                curSum = m_StageInfo[i].GetWaveTime(begin, end);

                if (curSum == -1) return -1;
                sum += curSum;

            }
            return sum;
        }
    }
}
