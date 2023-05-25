using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Event;
using System.Linq;

namespace UI.Manager
{
    /// <summary>
    /// 정해진 타이밍에 분류가 같은 정해지지 않은 스킬들
    /// </summary>
    [System.Serializable]
    public class FixedNonSpecificEvent
    {
        public int m_EventCount;
        public Scriptable.UI.EventType m_EventType;
    }

    /// <summary>
    /// 정해진 타이밍에 정해진 스킬들
    /// </summary>
    [System.Serializable]
    public class FixedSpecificEvent
    {
        public int m_EventCount;
        public SkillEvent[] m_SkillEvent;
    }

    [System.Serializable]
    public class SkillEventSet
    {
        public Scriptable.UI.EventType m_EventType;
        public SkillEvent[] m_SkillEvent;
    }

    public class SkillEventManager : MonoBehaviour
    {
        #region SerializeField
        [SerializeField] private SettingUIManager m_SettingUIManager;
        [SerializeField] private PlayerData m_PlayerData;
        [SerializeField] private RectTransform m_SkillPos;

        [SerializeField] private FixedNonSpecificEvent[] m_FixedNonSpecificEvents;
        [SerializeField] private FixedSpecificEvent[] m_FixedSpecificEvents;
        //위 둘 타이밍 안곂쳐야 함

        [SerializeField] private SkillEventSet[] m_SkillEvent;
        #endregion

        private Animator m_Animator;

        private readonly Dictionary<int, FixedNonSpecificEvent> m_FixedNonSpecificDict = new();
        private readonly Dictionary<int, FixedSpecificEvent> m_FixedSpecificDict = new();

        private const int m_DefaultDisplayCount = 3;
        private int m_EventCount = 0;

        private void Awake()
        {
            m_Animator = m_SkillPos.GetComponent<Animator>();
            SetFixedEvent();
        }

        private void SetFixedEvent()
        {
            foreach (FixedNonSpecificEvent fnse in m_FixedNonSpecificEvents)
                m_FixedNonSpecificDict.Add(fnse.m_EventCount, fnse);

            foreach (FixedSpecificEvent fse in m_FixedSpecificEvents)
                m_FixedSpecificDict.Add(fse.m_EventCount, fse);
        }

        [ContextMenu("OccurSkillEvent")]
        public void OccurSkillEvent()
        {
            m_SkillPos.gameObject.SetActive(true);
            m_Animator.SetTrigger("Show");
            m_SettingUIManager.PauseMode(true);
            m_EventCount++;

            BatchSkillUI();
        }

        private void BatchSkillUI()
        {
            if (m_FixedNonSpecificDict.ContainsKey(m_EventCount)) BatchFixedNonSpecific();
            else if (m_FixedSpecificDict.ContainsKey(m_EventCount)) BatchFixedSpecific();
            else BatchRandom();
        }

        /// <summary>
        /// 타입만 정해지고 스킬은 랜덤
        /// </summary>
        private void BatchFixedNonSpecific()
        {
            FixedNonSpecificEvent fnse = m_FixedNonSpecificDict[m_EventCount];
            SkillEventSet skillEventSet = m_SkillEvent[(int)fnse.m_EventType];

            int[] randomNumber = GetRandomNumber(0, skillEventSet.m_SkillEvent.Length);

            foreach(int i in randomNumber)
            {
                skillEventSet.m_SkillEvent[i].Init();
            }

        }

        private int[] GetRandomNumber(int min, int max, int count = m_DefaultDisplayCount)
        {
            System.Random random = new System.Random();
            int[] randomElements = Enumerable.Range(min, max - min + 1)
                                             .OrderBy(x => random.Next())
                                             .Take(count)
                                             .ToArray();
            return randomElements;
        }

        /// <summary>
        /// 타입, 스킬 모두 정해짐
        /// </summary>
        private void BatchFixedSpecific()
        {
            FixedSpecificEvent fse = m_FixedSpecificDict[m_EventCount];

            for(int i = 0; i < fse.m_SkillEvent.Length; i++) fse.m_SkillEvent[i].Init();
        }

        /// <summary>
        /// 완전한 랜덤
        /// </summary>
        private void BatchRandom()
        {

        }

        [ContextMenu("EndSkillEvent")]
        public void EndSkillEvent()
        {
            m_Animator.SetTrigger("Hide");
            m_SettingUIManager.PauseMode(false);
        }

        public void EndHideAnimation() => m_SkillPos.gameObject.SetActive(false);
        
    }
}
