using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static bool IsGameEnd { get; private set; } = false;
        public static void GameEnd()
        {

            Debug.Log("GameEnd");
        }
    }
}
