using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public void OnClickedGameStart()
    {
        LoadingSceneController.LoadScene("GameScene");
    }
}
