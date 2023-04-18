using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    //������ ó���ϴ°� ���⼭
    //UIManager���� ���� �� �� ���� ��������
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
    /// ���� ���� ��
    /// </summary>
    /// <param name="equipingWeaponType">���� Ÿ�� (���� ��ȣ�� ��ġ)</param>
    /// <param name="weaponImage">���� ������ �̹���</param>
    public void ChangeWeapon(Test.EquipingWeaponType equipingWeaponType, UnityEngine.UI.Image weaponImage)
    {
        m_CurrentEquipingWeaponType = equipingWeaponType;
        m_PlayerUIManager.ChangeWeapon(equipingWeaponType, weaponImage);
    }


    /// <summary>
    /// ���Ÿ� ���� ��ݸ�� ���� ��
    /// </summary>
    /// <param name="fireMode">��� ���</param>
    public void ChangeFireMode(Test.FireMode fireMode)
    {
        int arrayIndex = (int)Mathf.Log((int)fireMode);
        Debug.Log("FireMode �αװ� : " + arrayIndex);
        m_PlayerUIManager.ChangeFireMode(arrayIndex);
    }


    /// <summary>
    /// ���Ÿ� ���� ��� ��
    /// </summary>
    public void RangeWeaponFire()   //���� �ȴ�
    {
        //m_Inventory.GetCurrentRemainBullet((int)m_CurrentEquipingWeaponType)
        m_PlayerUIManager.RangeWeaponFire();
    }


    /// <summary>
    /// ���⸦ ����� ��
    /// </summary>
    /// <param name="equipingWeaponType">���� Ÿ�� (���� ��ȣ�� ��ġ)</param>
    public void GetWeapon(Test.EquipingWeaponType equipingWeaponType, Sprite sprite, int weaponIndex)    //���� �ȴ�
    {
        m_Inventory.SetHavingWeaponIndex((int)equipingWeaponType, weaponIndex);
        m_PlayerUIManager.UpdateWeaponSlot(equipingWeaponType, sprite);
    }


    /// <summary>
    /// ��ô ���⸦ ����ϰų� ����� ��
    /// </summary>
    /// <param name="value">����ϰų� ���� ���� (���� ����)</param>
    public void UpdateRemainThrowingWeapon(int value)
    {
        int remainThrowingWeapon = m_Inventory.AddThrowingWeapon(value);
        m_PlayerUIManager.UpdateRemainThrowingWeapon(remainThrowingWeapon);
    }


    /// <summary>
    /// ��Ŷ�� ����ϰų� ����� ��
    /// </summary>
    /// <param name="value">����ϰų� ���� ���� (���� ����)</param>
    public void UsingHealKit(int value)
    {
        m_PlayerHP += HealAmount;
        m_AmountPlayerHP = m_PlayerHP / m_ToAmountConst;

        int remainHealKit = m_Inventory.AddHealKit(value);
        m_PlayerUIManager.UsingHealKit(remainHealKit, m_AmountPlayerHP);
    }


    /// <summary>
    /// �÷��̾ �ǰݴ��ϰų� �ڿ�ȸ�� �Ǵ� ��Ŷ�� ��
    /// </summary>
    /// <param name="value">�ǰݽ� �Ǵ� ȸ���� ���</param>
    public void UpdatePlayerHP(int value)
    {
        m_PlayerHP -= value;
        m_AmountPlayerHP = m_PlayerHP / m_ToAmountConst;
        m_PlayerUIManager.UpdatePlayerHP(m_AmountPlayerHP);
    }


    /// <summary>
    /// �÷��̾ Ư���� ������ ����ϰų� �ڿ�ȸ���� ��
    /// </summary>
    /// <param name="value">���۽� �Ǵ� ȸ���� ���</param>
    public void UpdatePlayerMP(int value)
    {
        m_PlayerMP -= value;
        m_AmountPlayerMP = m_PlayerMP / m_ToAmountConst;
        m_PlayerUIManager.UpdatePlayerMP(m_AmountPlayerMP);
    }
}
