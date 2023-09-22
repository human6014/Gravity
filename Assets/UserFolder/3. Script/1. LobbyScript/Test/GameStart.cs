using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public void OnClickedGameStart(int difficultyIndex)
    {
        GamePlaySetting.m_DifficultyIndex = difficultyIndex;
        LoadingSceneController.LoadScene("GameScene");
    }
}
