using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControlSetting : Setting
{
    public float m_LookSensitivity { get; set; }
    public float m_AimSensitivity { get; set; }
    public int m_AimMode { get; set; }    //0 : Hold  1 : Toggle

    #region KeyBinding
    public KeyCode m_MoveForward { get; private set; }   //119
    public KeyCode m_MoveBack { get; private set; }      //115
    public KeyCode m_MoveLeft { get; private set; }      //97
    public KeyCode m_MoveRight { get; private set; }     //100

    public KeyCode m_Run { get; private set; }           //304
    public KeyCode m_Jump { get; private set; }          //32
    public KeyCode m_Crouch { get; private set; }        //306
    public KeyCode m_Reload { get; private set; }        //114

    public KeyCode m_TimeSlow { get; private set; }      //102
    public KeyCode m_Heal { get; private set; }          //101
    public KeyCode m_Change { get; private set; }        //110

    public KeyCode m_GravityX { get; private set; }      //122
    public KeyCode m_GravityY { get; private set; }      //120
    public KeyCode m_GravityZ { get; private set; }      //99
    #endregion

    public object this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return m_LookSensitivity;
                case 1: return m_AimSensitivity;
                case 2: return m_AimMode;

                case 3: return m_MoveForward;
                case 4: return m_MoveBack;
                case 5: return m_MoveLeft;
                case 6: return m_MoveRight;

                case 7: return m_Run;
                case 8: return m_Jump;
                case 9: return m_Crouch;
                case 10: return m_Reload;
                case 11: return m_TimeSlow;
                case 12: return m_Heal;
                case 13: return m_Change;

                case 14: return m_GravityX;
                case 15: return m_GravityY;
                case 16: return m_GravityZ;

                default: Debug.Log("Indexer name is null"); return null;
            }
        }
        set
        {
            switch (index)
            {
                case 0: m_LookSensitivity = (float)value; break;
                case 1: m_AimSensitivity = (float)value; break;
                case 2: m_AimMode = (int)value; break;

                case 3: m_MoveForward = (KeyCode)value; break;
                case 4: m_MoveBack = (KeyCode)value; break;
                case 5: m_MoveLeft = (KeyCode)value; break;
                case 6: m_MoveRight = (KeyCode)value; break;

                case 7: m_Run = (KeyCode)value; break;
                case 8: m_Jump = (KeyCode)value; break;
                case 9: m_Crouch = (KeyCode)value; break;
                case 10: m_Reload = (KeyCode)value; break;
                case 11: m_TimeSlow = (KeyCode)value; break;
                case 12: m_Heal = (KeyCode)value; break;
                case 13: m_Change = (KeyCode)value; break;

                case 14: m_GravityX = (KeyCode)value; break;
                case 15: m_GravityY = (KeyCode)value; break;
                case 16: m_GravityZ = (KeyCode)value; break;

                default: Debug.Log("Indexer name is null"); break;
            }
        }
    }

    #region Load & Save
    public override void LoadDefault()
    {
        Debug.Log("Load Default GameControlSettings");

        m_LookSensitivity = 2;          //2;
        m_AimSensitivity = 2;           //2;
        m_AimMode = 0;                  //0;
        
        m_MoveForward = KeyCode.W;      //KeyCode.W; 
        m_MoveBack = KeyCode.S;         //KeyCode.S; 
        m_MoveLeft = KeyCode.A;         //KeyCode.A; 
        m_MoveRight = KeyCode.D;        //KeyCode.D; 

        m_Run = KeyCode.LeftShift;      //KeyCode.LeftShift;
        m_Jump = KeyCode.Space;         //KeyCode.Space; 
        m_Crouch = KeyCode.LeftControl; //KeyCode.LeftControl;
        m_Reload = KeyCode.R;           //KeyCode.R;
        m_TimeSlow = KeyCode.F;         //KeyCode.F;
        m_Heal = KeyCode.E;             //KeyCode.E;
        m_Change = KeyCode.N;           //KeyCode.N;

        m_GravityX = KeyCode.Z;         //KeyCode.Z;
        m_GravityY = KeyCode.X;         //KeyCode.X;
        m_GravityZ = KeyCode.C;         //KeyCode.C;

    }

    public override void LoadData()
    {
        Debug.Log("Load GameControlSettings");

        m_LookSensitivity = PlayerPrefs.GetFloat("LookSensitivity");
        m_AimSensitivity = PlayerPrefs.GetFloat("AimSensitivity");
        m_AimMode = PlayerPrefs.GetInt("AimMode");

        //---------------Key---------------
        m_MoveForward = (KeyCode)PlayerPrefs.GetInt("MoveForward");
        m_MoveBack = (KeyCode)PlayerPrefs.GetInt("MoveBack");
        m_MoveLeft = (KeyCode)PlayerPrefs.GetInt("MoveLeft");
        m_MoveRight = (KeyCode)PlayerPrefs.GetInt("MoveRight");

        m_Run = (KeyCode)PlayerPrefs.GetInt("Run");
        m_Jump = (KeyCode)PlayerPrefs.GetInt("Jump");
        m_Crouch = (KeyCode)PlayerPrefs.GetInt("Crouch");
        m_Reload = (KeyCode)PlayerPrefs.GetInt("Reload");
        m_TimeSlow = (KeyCode)PlayerPrefs.GetInt("TimeSlow"); 
        m_Heal = (KeyCode)PlayerPrefs.GetInt("Heal");
        m_Change = (KeyCode)PlayerPrefs.GetInt("Change");

        m_GravityX = (KeyCode)PlayerPrefs.GetInt("GravityX");
        m_GravityY = (KeyCode)PlayerPrefs.GetInt("GravityY");
        m_GravityZ = (KeyCode)PlayerPrefs.GetInt("GravityZ");
    }

    public override void SaveData()
    {
        Debug.Log("Save GameControlSettings");

        PlayerPrefs.SetFloat("LookSensitivity", m_LookSensitivity);
        PlayerPrefs.SetFloat("AimSensitivity", m_AimSensitivity);
        PlayerPrefs.SetInt("AimMode", m_AimMode);

        //---------------Key---------------
        PlayerPrefs.SetInt("MoveForward", (int)m_MoveForward);
        PlayerPrefs.SetInt("MoveBack", (int)m_MoveBack);
        PlayerPrefs.SetInt("MoveLeft", (int)m_MoveLeft);
        PlayerPrefs.SetInt("MoveRight", (int)m_MoveRight);

        PlayerPrefs.SetInt("Run", (int)m_Run);
        PlayerPrefs.SetInt("Jump", (int)m_Jump);
        PlayerPrefs.SetInt("Crouch", (int)m_Crouch);
        PlayerPrefs.SetInt("Reload", (int)m_Reload);

        PlayerPrefs.SetInt("TimeSlow", (int)m_TimeSlow);
        PlayerPrefs.SetInt("Heal", (int)m_Heal);
        PlayerPrefs.SetInt("Change", (int)m_Change);

        PlayerPrefs.SetInt("GravityX", (int)m_GravityX);
        PlayerPrefs.SetInt("GravityY", (int)m_GravityY);
        PlayerPrefs.SetInt("GravityZ", (int)m_GravityZ);
        //PlayerPrefs.Save();
    }
    #endregion
    public void ChangeKey(int index, KeyCode key)
    {
        Debug.Log("ChangeKey");
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
            case 114:
                m_Reload = key;
                break;
            case 102:
                m_TimeSlow = key;
                break;
            case 101:
                m_Heal = key;
                break;
            case 110:
                m_Change = key;
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
        Debug.Log("m_AimMode : " + m_AimMode);

        Debug.Log("-------------KEY--------------");

        Debug.Log("m_MoveForward : " + m_MoveForward);
        Debug.Log("m_MoveBack : " + m_MoveBack);
        Debug.Log("m_MoveLeft : " + m_MoveLeft);
        Debug.Log("m_MoveRight : " + m_MoveRight);

        Debug.Log("m_Run : " + m_Run);
        Debug.Log("m_Jump : " + m_Jump);
        Debug.Log("m_Crouch : " + m_Crouch);
        Debug.Log("m_Reload : " + m_Reload);
        Debug.Log("m_TimeSlow : " + m_TimeSlow);
        Debug.Log("m_Heal : " + m_Heal);
        Debug.Log("m_Change" + m_Change);

        Debug.Log("m_GravityX : " + m_GravityX);
        Debug.Log("m_GravityY : " + m_GravityY);
        Debug.Log("m_GravityZ : " + m_GravityZ);
    }
}
