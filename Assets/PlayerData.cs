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

    private int m_CurrentPlayerHP = 100;
    private int m_CurrentPlayerMP = 100;

    private float m_AmountPlayerHP = 1;
    private float m_AmountPlayerMP = 1;

    private const float m_RealToAmountConst = 0.01f;
    private const int m_AmountToRealConst = 100;

    //private Test.EquipingWeaponType m_CurrentEquipingWeaponType;
    private WeaponInfo m_CurrentWeaponInfo;

    public Inventory GetInventory() => m_Inventory;

    public int PlayerMaxHP { get; } = 100;

    public int PlayerMaxMP { get; } = 100;

    public int PlayerHP 
    {
        get => m_CurrentPlayerHP;
        set
        {
            m_CurrentPlayerHP = value;
            m_AmountPlayerHP = m_CurrentPlayerHP * m_RealToAmountConst;
        } 
    }

    public int PlayerMP
    {
        get => m_CurrentPlayerMP;
        set
        {
            m_CurrentPlayerMP = value;
            m_AmountPlayerMP = m_CurrentPlayerMP * m_RealToAmountConst;
        }
    }

    private void Awake()
    {
        m_PlayerUIManager.Init(PlayerMaxHP, PlayerMaxMP, m_AmountToRealConst);
    }

    /// <summary>
    /// ���� ���� ��
    /// </summary>
    /// <param name="equipingWeaponType">���� Ÿ�� (���� ��ȣ�� ��ġ)</param>
    /// <param name="bulletType">���� �Ѿ� Ÿ�� (������ ǥ�ÿ�)</param>
    /// <param name="weaponImage">���� ������ �̹���</param>
    public void ChangeWeapon(int equipingWeaponType, BulletType bulletType, Sprite weaponImage)
    {
        //m_CurrentEquipingWeaponType = (Test.EquipingWeaponType)equipingWeaponType;
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

    private int GetBitPosition(int value)
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
            m_CurrentWeaponInfo.m_MagazineRemainBullet = 0;
            m_CurrentWeaponInfo.m_CurrentRemainBullet = totalBullet;
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
        PlayerHP = Mathf.Clamp(PlayerHP + HealAmount, 0, PlayerMaxHP);
        int remainHealKit = m_Inventory.HealKitHavingCount += value;
        m_PlayerUIManager.UsingHealKit(remainHealKit, m_AmountPlayerHP);
    }


    /// <summary>
    /// �÷��̾ �ǰݴ��ϰų� �ڿ�ȸ�� �Ǵ� ��Ŷ�� ��
    /// </summary>
    /// <param name="value">�ǰݽ� �Ǵ� ȸ���� ���</param>
    public void UpdatePlayerHP(int value)
    {
        PlayerHP = Mathf.Clamp(PlayerHP - value, 0, PlayerMaxMP);
        m_PlayerUIManager.UpdatePlayerHP(m_AmountPlayerHP);
        if (PlayerHP <= 0)
        {
            //GameEnd
        }
    }

    /// <summary>
    /// �÷��̾ Ư���� ������ ����ϰų� �ڿ�ȸ���� ��
    /// </summary>
    /// <param name="value">���۽� �Ǵ� ȸ���� ���</param>
    public void UpdatePlayerMP(int value)
    {
        m_CurrentPlayerMP -= value;
        m_AmountPlayerMP = m_CurrentPlayerMP / m_RealToAmountConst;
        m_PlayerUIManager.UpdatePlayerMP(m_AmountPlayerMP);
    }


    [ContextMenu("TestHit")]
    public void TestHit()
    {
        UpdatePlayerHP(10);
    }
}
