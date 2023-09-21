using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControlSetting : Setting
{
    public static float m_LookSensitivity = 1;
    public static float m_AimSensitivity = 1;

    public static int m_RunMode = 0;   //0 : Hold  1 : Toggle
    public static int m_AimMode = 0;   //0 : Hold  1 : Toggle

    #region KeyBinding
    public static KeyCode m_MoveForward { get; private set; }   //119
    public static KeyCode m_MoveBack { get; private set; }      //115
    public static KeyCode m_MoveLeft { get; private set; }      //97
    public static KeyCode m_MoveRight { get; private set; }     //100

    public static KeyCode m_Run { get; private set; }           //304
    public static KeyCode m_Jump { get; private set; }          //32
    public static KeyCode m_Crouch { get; private set; }        //306

    public static KeyCode m_TimeSlow { get; private set; }      //102

    public static KeyCode m_GravityX { get; private set; }      //122
    public static KeyCode m_GravityY { get; private set; }      //120
    public static KeyCode m_GravityZ { get; private set; }      //99
    #endregion

    #region Load & Save
    public override void LoadDefault()
    {
        m_LookSensitivity = 1;
        m_AimSensitivity = 1;
        m_RunMode = 0;
        m_AimMode = 0;

        m_MoveForward = KeyCode.W;      //119
        m_MoveBack = KeyCode.S;         //115
        m_MoveLeft = KeyCode.A;         //97
        m_MoveRight = KeyCode.D;        //100
        m_Run = KeyCode.LeftShift;      //304
        m_Jump = KeyCode.Space;         //32
        m_Crouch = KeyCode.LeftControl; //306
        m_TimeSlow = KeyCode.F;         //102
        m_GravityX = KeyCode.Z;         //122
        m_GravityY = KeyCode.X;         //120
        m_GravityZ = KeyCode.C;         //99
    }

    public override void LoadData()
    {
        m_LookSensitivity = PlayerPrefs.GetFloat("LookSensitivity");
        m_AimSensitivity = PlayerPrefs.GetFloat("AimSensitivity");

        m_RunMode = PlayerPrefs.GetInt("RunMode");
        m_AimMode = PlayerPrefs.GetInt("AimMode");

        m_MoveForward = (KeyCode)PlayerPrefs.GetInt("MoveForward");
        m_MoveBack = (KeyCode)PlayerPrefs.GetInt("MoveBack");
        m_MoveLeft = (KeyCode)PlayerPrefs.GetInt("MoveLeft");
        m_MoveRight = (KeyCode)PlayerPrefs.GetInt("MoveRight");

        m_Run = (KeyCode)PlayerPrefs.GetInt("Run");
        m_Jump = (KeyCode)PlayerPrefs.GetInt("Jump");
        m_Crouch = (KeyCode)PlayerPrefs.GetInt("Crouch");

        m_TimeSlow = (KeyCode)PlayerPrefs.GetInt("TimeSlow");

        m_GravityX = (KeyCode)PlayerPrefs.GetInt("GravityX");
        m_GravityY = (KeyCode)PlayerPrefs.GetInt("GravityY");
        m_GravityZ = (KeyCode)PlayerPrefs.GetInt("GravityZ");
    }

    public override void SaveData()
    {
        PlayerPrefs.SetFloat("LookSensitivity", m_LookSensitivity);
        PlayerPrefs.SetFloat("AimSensitivity", m_AimSensitivity);

        PlayerPrefs.SetInt("RunMode", m_RunMode);
        PlayerPrefs.SetInt("AimMode", m_AimMode);

        PlayerPrefs.SetInt("MoveForward", (int)m_MoveForward);
        PlayerPrefs.SetInt("MoveBack", (int)m_MoveBack);
        PlayerPrefs.SetInt("MoveLeft", (int)m_MoveLeft);
        PlayerPrefs.SetInt("MoveRight", (int)m_MoveRight);

        PlayerPrefs.SetInt("Run", (int)m_Run);
        PlayerPrefs.SetInt("Jump", (int)m_Jump);
        PlayerPrefs.SetInt("Crouch", (int)m_Crouch);

        PlayerPrefs.SetInt("TimeSlow", (int)m_TimeSlow);

        PlayerPrefs.SetInt("GravityX", (int)m_GravityX);
        PlayerPrefs.SetInt("GravityY", (int)m_GravityY);
        PlayerPrefs.SetInt("GravityZ", (int)m_GravityZ);
    }
    #endregion
    public static void ChangeKey(int index, KeyCode key)
    {
        switch (index)
        {
            case 119:
                m_MoveForward = key;
                break;
            case 115:
                m_MoveBack = key;
                break;
            case 97:
                m_MoveLeft = key;
                break;
            case 100:
                m_MoveRight = key;
                break;

            case 304:
                m_Run = key;
                break;
            case 32:
                m_Jump = key;
                break;
            case 306:
                m_Crouch = key;
                break;

            case 102:
                m_TimeSlow = key;
                break;

            case 122:
                m_GravityX = key;
                break;
            case 120:
                m_GravityY = key;
                break;
            case 99:
                m_GravityZ = key;
                break;
        }
    }

    public void DebugAllSetting()
    {
        Debug.Log("m_LookSensitivity : " + m_LookSensitivity);
        Debug.Log("m_AimSensitivity : " + m_AimSensitivity);
        Debug.Log("m_RunMode : " + m_RunMode);
        Debug.Log("m_AimMode : " + m_AimMode);
        Debug.Log("-------------KEY--------------");
        Debug.Log("m_MoveForward : " + m_MoveForward);
        Debug.Log("m_MoveBack : " + m_MoveBack);
        Debug.Log("m_MoveLeft : " + m_MoveLeft);
        Debug.Log("m_MoveRight : " + m_MoveRight);
        Debug.Log("m_Run : " + m_Run);
        Debug.Log("m_Jump : " + m_Jump);
        Debug.Log("m_Crouch : " + m_Crouch);
        Debug.Log("m_TimeSlow : " + m_TimeSlow);
        Debug.Log("m_GravityX : " + m_GravityX);
        Debug.Log("m_GravityY : " + m_GravityY);
        Debug.Log("m_GravityZ : " + m_GravityZ);
    }
}
