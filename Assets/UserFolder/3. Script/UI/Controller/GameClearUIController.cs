using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearUIController : MonoBehaviour
{
    [ContextMenu("SetGameClear")]
    public void SetGameClear()
    {
        gameObject.SetActive(true);
        if (DataManager.Instance != null)
        {
            GamePlaySetting gamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
            gamePlaySetting.m_HasHardClearData = 1;
            gamePlaySetting.SaveData();
        }


    }
}
