using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    //데이터 처리하는건 여기서
    //UIManager한테 보낼 껀 다 계산된 데이터임
    [SerializeField] private UI.Manager.PlayerUIManager m_PlayerUIManager;
    [SerializeField] private Inventory m_Inventory;
    [SerializeField] private int HealAmount;

    private int m_PlayerHP;
    private int m_PlayerMP;
    private float m_AmountPlayerHP;
    private float m_AmountPlayerMP;
    private const int m_ToAmountConst = 1000;

    private Test.EquipingWeaponType m_CurrentEquipingWeaponType;

    public Inventory GetInventory() => m_Inventory;
    public int GetPlayerHP() => m_PlayerHP;
    public int GetPlayerMP() => m_PlayerMP;

    /// <summary>
    /// 무기 변경 시
    /// </summary>
    /// <param name="equipingWeaponType">무기 타입 (슬롯 번호랑 일치)</param>
    /// <param name="weaponImage">무기 아이콘 이미지</param>
    public void ChangeWeapon(Test.EquipingWeaponType equipingWeaponType, UnityEngine.UI.Image weaponImage)
    {
        m_CurrentEquipingWeaponType = equipingWeaponType;
        m_PlayerUIManager.ChangeWeapon(equipingWeaponType, weaponImage);
    }


    /// <summary>
    /// 원거리 무기 사격모드 변경 시
    /// </summary>
    /// <param name="fireMode">사격 모드</param>
    public void ChangeFireMode(Test.FireMode fireMode)
    {
        int arrayIndex = (int)Mathf.Log((int)fireMode);
        Debug.Log("FireMode 로그값 : " + arrayIndex);
        m_PlayerUIManager.ChangeFireMode(arrayIndex);
    }


    /// <summary>
    /// 원거리 무기 사격 시
    /// </summary>
    public void RangeWeaponFire()   //아직 안댐
    {
        //m_Inventory.GetCurrentRemainBullet((int)m_CurrentEquipingWeaponType)
        m_PlayerUIManager.RangeWeaponFire();
    }


    /// <summary>
    /// 무기를 얻었을 때
    /// </summary>
    /// <param name="equipingWeaponType">무기 타입 (슬롯 번호랑 일치)</param>
    public void GetWeapon(Test.EquipingWeaponType equipingWeaponType, Sprite sprite, int weaponIndex)    //아직 안댐
    {
        m_Inventory.SetHavingWeaponIndex((int)equipingWeaponType, weaponIndex);
        m_PlayerUIManager.UpdateWeaponSlot(equipingWeaponType, sprite);
    }


    /// <summary>
    /// 투척 무기를 사용하거나 얻었을 때
    /// </summary>
    /// <param name="value">사용하거나 얻은 개수 (음수 가능)</param>
    public void UpdateRemainThrowingWeapon(int value)
    {
        int remainThrowingWeapon = m_Inventory.AddThrowingWeapon(value);
        m_PlayerUIManager.UpdateRemainThrowingWeapon(remainThrowingWeapon);
    }


    /// <summary>
    /// 힐킷을 사용하거나 얻었을 때
    /// </summary>
    /// <param name="value">사용하거나 얻은 개수 (음수 가능)</param>
    public void UsingHealKit(int value)
    {
        m_PlayerHP += HealAmount;
        m_AmountPlayerHP = m_PlayerHP / m_ToAmountConst;

        int remainHealKit = m_Inventory.AddHealKit(value);
        m_PlayerUIManager.UsingHealKit(remainHealKit, m_AmountPlayerHP);
    }


    /// <summary>
    /// 플레이어가 피격당하거나 자연회복 또는 힐킷을 때
    /// </summary>
    /// <param name="value">피격시 또는 회복할 계수</param>
    public void UpdatePlayerHP(int value)
    {
        m_PlayerHP -= value;
        m_AmountPlayerHP = m_PlayerHP / m_ToAmountConst;
        m_PlayerUIManager.UpdatePlayerHP(m_AmountPlayerHP);
    }


    /// <summary>
    /// 플레이어가 특수한 동작을 사용하거나 자연회복할 때
    /// </summary>
    /// <param name="value">동작시 또는 회복할 계수</param>
    public void UpdatePlayerMP(int value)
    {
        m_PlayerMP -= value;
        m_AmountPlayerMP = m_PlayerMP / m_ToAmountConst;
        m_PlayerUIManager.UpdatePlayerMP(m_AmountPlayerMP);
    }
}
