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
    private WeaponInfo m_CurrentWeaponInfo;
    public Inventory GetInventory() => m_Inventory;
    public int GetPlayerHP() => m_PlayerHP;
    public int GetPlayerMP() => m_PlayerMP;


    /// <summary>
    /// ���� ���� ��
    /// </summary>
    /// <param name="equipingWeaponType">���� Ÿ�� (���� ��ȣ�� ��ġ)</param>
    /// <param name="bulletType">���� �Ѿ� Ÿ�� (������ ǥ�ÿ�)</param>
    /// <param name="weaponImage">���� ������ �̹���</param>
    public void ChangeWeapon(int equipingWeaponType, BulletType bulletType, Sprite weaponImage)
    {
        m_CurrentEquipingWeaponType = (Test.EquipingWeaponType)equipingWeaponType;
        m_CurrentWeaponInfo = m_Inventory.WeaponInfo[equipingWeaponType];
        m_PlayerUIManager.ChangeWeapon(equipingWeaponType, (int)bulletType, m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, weaponImage);

        bool isActive = IsActiveReloadImage(m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, 30);
        m_PlayerUIManager.DisplayReloadImage(isActive);
    }

    /// <summary>
    /// ���Ÿ� ���� ��ݸ�� ���� ��
    /// </summary>
    /// <param name="fireMode">��� ���</param>
    public void ChangeFireMode(Test.FireMode fireMode)
    {
        int arrayIndex = GetBitPosition((int)fireMode);
        m_PlayerUIManager.ChangeFireMode(arrayIndex);
    }

    int GetBitPosition(int value)
    {
        int position = 0;
        while ((value & 1) == 0)
        {
            value >>= 1;
            position++;
        }
        return position;
    }

    /// <summary>
    /// ���Ÿ� ���� ��� ��
    /// </summary>
    public void RangeWeaponFire(int maxBullet)
    {
        //m_Inventory.GetCurrentRemainBullet((int)m_CurrentEquipingWeaponType)
        int currentRemainBullet = --m_CurrentWeaponInfo.m_CurrentRemainBullet;
        bool isActiveReloadImage = IsActiveReloadImage(currentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, maxBullet);

        m_PlayerUIManager.RangeWeaponFire(currentRemainBullet, isActiveReloadImage);
    }

    public void RangeWeaponReload(int maxBullet)
    {
        int totalBullet = m_CurrentWeaponInfo.m_CurrentRemainBullet + m_CurrentWeaponInfo.m_MagazineRemainBullet;
        
        if (totalBullet > maxBullet)
        {
            m_CurrentWeaponInfo.m_MagazineRemainBullet -= maxBullet - m_CurrentWeaponInfo.m_CurrentRemainBullet;
            m_CurrentWeaponInfo.m_CurrentRemainBullet = maxBullet;
        }
        else
        {
            m_CurrentWeaponInfo.m_CurrentRemainBullet = totalBullet;
            m_CurrentWeaponInfo.m_MagazineRemainBullet = 0;
        }
        bool isActiveReloadImage = IsActiveReloadImage(m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, maxBullet);
        m_PlayerUIManager.RangeWeaponReload(m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, isActiveReloadImage);
    }

    private bool IsActiveReloadImage(int currentRemainBullet, int currentMagazineBullet, int maxBullet)
    {
        if (maxBullet != 0 && currentRemainBullet < maxBullet * 0.5f && currentMagazineBullet != 0) return true;
        return false;
    }

    /// <summary>
    /// ���⸦ ����� ��
    /// </summary>
    /// <param name="equipingWeaponType">���� Ÿ�� (���� ��ȣ�� ��ġ)</param>
    public void GetWeapon(int equipingWeaponType, Sprite sprite, int weaponIndex, int maxBullet)    //���� �ȴ�
    {
        m_Inventory.WeaponInfo[equipingWeaponType].m_HavingWeaponIndex = weaponIndex;
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
