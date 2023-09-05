using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Manager
{
    public class StatisticsManager : MonoBehaviour
    {
        private int m_UrbanZombieKillCount;
        private int m_OldmanZombieKillCount;
        private int m_WomenZombieKillCount;
        private int m_BigZombieKillCount;
        private int m_GiantZombieKillCount;

        private int m_RangeFlyingMonsterKillCount;

        private int m_AllKillCount;

        public int UrbanZombieKillCount 
        { 
            get => m_UrbanZombieKillCount;
            set
            {
                m_UrbanZombieKillCount = value;
                m_AllKillCount++;
                MonsterKillEvent?.Invoke(0, 0, m_AllKillCount);
            }
        }

        public int OldmanZombieKillCount 
        { 
            get => m_OldmanZombieKillCount;
            set
            {
                m_OldmanZombieKillCount = value;
                m_AllKillCount++;
                MonsterKillEvent?.Invoke(0, 1, m_AllKillCount);
            }
        }
        public int WomenZombieKillCount
        {
            get => m_WomenZombieKillCount;
            set
            {
                m_WomenZombieKillCount = value;
                m_AllKillCount++;
                MonsterKillEvent?.Invoke(0, 2, m_AllKillCount);
            }
        }

        public int BigZombieKillCount 
        { 
            get => m_BigZombieKillCount;
            set
            {
                m_BigZombieKillCount = value;
                m_AllKillCount++;
                MonsterKillEvent?.Invoke(0, 3, m_AllKillCount);
            }
        }

        public int GiantZombieKillCount 
        { 
            get => m_GiantZombieKillCount;
            set
            {
                m_GiantZombieKillCount = value;
                m_AllKillCount++;
                MonsterKillEvent?.Invoke(0,4, m_AllKillCount);
            }
        }

        public int RangeFlyingMonsterKillCount 
        { 
            get => m_RangeFlyingMonsterKillCount;
            set
            {
                m_RangeFlyingMonsterKillCount = value;
                m_AllKillCount++;
                MonsterKillEvent?.Invoke(1,0, m_AllKillCount);
            }
        }

        [SerializeField] private UnityEvent<int, int, int> MonsterKillEvent;

        public void NormalMonsterKillCount(int type)
        {
            switch (type)
            {
                case 0:
                    UrbanZombieKillCount++;
                    break;
                case 1:
                    OldmanZombieKillCount++;
                    break;
                case 2:
                    WomenZombieKillCount++;
                    break;
                case 3:
                    BigZombieKillCount++;
                    break;
                case 4:
                    GiantZombieKillCount++;
                    break;
            }
        }

        public void FlyingMonsterKillCount(int type)
        {
            switch (type)
            {
                case 0:
                    RangeFlyingMonsterKillCount++;
                    break;
            }
        }
    }
}
