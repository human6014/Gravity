using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UI.Manager;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int m_FrameRate = 60;


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

            //Application.targetFrameRate = m_FrameRate;
        }
    }
}
