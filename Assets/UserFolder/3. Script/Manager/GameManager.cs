using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UI.Manager;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private bool m_IsFixedFrameRate;
        [SerializeField] private int m_FrameRate = 60;

        private DataManager m_DataManager;

        public static bool IsGameEnd { get; private set; }

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
            m_DataManager = FindObjectOfType<DataManager>();
            if (m_DataManager == null) return;
            Debug.Log("DataManager");

        }

        private void Start()
        {
            if (m_IsFixedFrameRate) Application.targetFrameRate = m_FrameRate;
        }
    }
}
