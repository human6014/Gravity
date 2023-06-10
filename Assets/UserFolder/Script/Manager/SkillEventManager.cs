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
        [Tooltip("이벤트가 일어날 순서")]
        public int m_EventCount;
        [Tooltip("이벤트 종류")]
        public Scriptable.UI.EventType m_EventType;
    }

    /// <summary>
    /// 정해진 타이밍에 정해진 스킬들
    /// </summary>
    [System.Serializable]
    public class FixedSpecificEvent
    {
        [Tooltip("이벤트가 일어날 순서")]
        public int m_EventCount;
        public SkillEvent[] m_SkillEvent;
    }

    [System.Serializable]
    public class SkillEventSet
    {
        [Tooltip("이벤트 종류")]
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
        //위 둘 EventCount 안곂쳐야 함

        [SerializeField] private SkillEventSet[] m_SkillEvent;
        #endregion

        private Animator m_Animator;

        private readonly Dictionary<int, FixedNonSpecificEvent> m_FixedNonSpecificDict = new();
        private readonly Dictionary<int, FixedSpecificEvent> m_FixedSpecificDict = new();
        private readonly List<SkillEvent> m_CurrentVisibleSkillEvents = new ();

        private const int m_DefaultDisplayCount = 3;
        private int m_GameEventCount = 0;

        private int[] m_EventCounts = new int[7];
        public bool m_IsOnEvent { get; private set; }

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
            m_IsOnEvent = true;
            m_SkillPos.gameObject.SetActive(true);
            m_Animator.SetTrigger("Show");

            m_SettingUIManager.PauseMode(true);
            //m_SettingUIManager.IsActivePauseUI = true; 

            //수정할것 버그 있음
            m_GameEventCount++;

            BatchSkillUI();
        }

        #region UI Batch
        private void BatchSkillUI()
        {
            int eventType;
            if (m_FixedNonSpecificDict.ContainsKey(m_GameEventCount)) eventType = BatchFixedNonSpecific();
            else if (m_FixedSpecificDict.ContainsKey(m_GameEventCount)) eventType = BatchFixedSpecific();
            else eventType = BatchRandom();

            m_EventCounts[eventType]++;

            for (int i = 0; i < m_CurrentVisibleSkillEvents.Count; i++)
                m_CurrentVisibleSkillEvents[i].PointerDownAction += OnPointerDown;
        }

        /// <summary>
        /// 타입만 정해지고 스킬은 랜덤
        /// </summary>
        private int BatchFixedNonSpecific()
        {
            Debug.Log("BatchFixedNonSpecificRandom");
            FixedNonSpecificEvent fnse = m_FixedNonSpecificDict[m_GameEventCount];
            int eventType = (int)fnse.m_EventType;
            SkillEventSet skillEventSet = m_SkillEvent[eventType];
            
            int[] randomNumber = GetRandomNumber(0, skillEventSet.m_SkillEvent.Length);
            foreach(int i in randomNumber)
            {
                m_CurrentVisibleSkillEvents.Add(skillEventSet.m_SkillEvent[i]);
                skillEventSet.m_SkillEvent[i].Init();
            }
            return eventType;
        }

        /// <summary>
        /// 타입, 스킬 모두 정해짐
        /// </summary>
        private int BatchFixedSpecific()
        {
            Debug.Log("BatchFixedSpecificRandom");
            FixedSpecificEvent fse = m_FixedSpecificDict[m_GameEventCount];
            int eventType = (int)fse.m_SkillEvent[0].EventType;
            for (int i = 0; i < fse.m_SkillEvent.Length; i++)
            {
                m_CurrentVisibleSkillEvents.Add(fse.m_SkillEvent[i]);
                fse.m_SkillEvent[i].Init();
            }
            return eventType;
        }

        /// <summary>
        /// 완전한 랜덤
        /// </summary>
        private int BatchRandom()
        {
            Debug.Log("BatchRandom");
            //GetWeapon 제외를 위해서 0번은 생략
            int randomType = Random.Range(1, (int)Scriptable.UI.EventType.Specific);
            SkillEventSet skillEventSet = m_SkillEvent[randomType];
            int[] randomNumber = GetRandomNumber(0, skillEventSet.m_SkillEvent.Length);

            foreach (int i in randomNumber)
            {
                m_CurrentVisibleSkillEvents.Add(skillEventSet.m_SkillEvent[i]);
                skillEventSet.m_SkillEvent[i].Init();
            }
            return randomType;
        }

        /// <summary>
        /// min부터 max까지 랜덤한 값 count개수만큼 반환
        /// </summary>
        /// <param name="min">min값을 포함해서 부터</param>
        /// <param name="max">max값을 제외까지</param>
        /// <param name="count">랜덤으로 추출할 개수</param>
        /// <returns>int 배열</returns>
        private int[] GetRandomNumber(int min, int max, int count = m_DefaultDisplayCount)
        {
            System.Random random = new System.Random();
            int[] randomElements = Enumerable.Range(min, max - min)
                                             .OrderBy(x => random.Next())
                                             .Take(count)
                                             .ToArray();
            return randomElements;
        }
        #endregion

        #region PointerAction
        private void OnPointerDown()
        {
            EndSkillEvent();
        }

        #endregion
        [ContextMenu("EndSkillEvent")]
        public void EndSkillEvent()
        {
            m_Animator.SetTrigger("Hide");
            m_SettingUIManager.PauseMode(false);
            //m_SettingUIManager.IsActivePauseUI = false; //수정할것

            for (int i = 0; i < m_CurrentVisibleSkillEvents.Count; i++)
            {
                m_CurrentVisibleSkillEvents[i].Dispose();
            }
            m_CurrentVisibleSkillEvents.Clear();
        }

        #region Animation Event
        public void EndHideAnimation()
        {
            m_IsOnEvent = false; 
            m_SkillPos.gameObject.SetActive(false);
        }
        #endregion
    }
}
