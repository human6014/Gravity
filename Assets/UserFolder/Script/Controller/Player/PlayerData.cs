using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PlayerData : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private UI.Manager.PlayerUIManager m_PlayerUIManager;
    [SerializeField] private Inventory m_Inventory;

    [Tooltip("�� Ŷ ȸ����")]
    [SerializeField] private int m_HealKitAmount = 400;

    [Tooltip("�ִ� HP")]
    [SerializeField] private int m_MaxPlayerHP = 1000;

    [Tooltip("�ִ� MP")]
    [SerializeField] private int m_MaxPlayerMP = 1000;


    [Tooltip("Ư�� �ð��� ȸ�� �� HP���")]
    [SerializeField] private int m_AutoHPHealAmount = 2;

    [Tooltip("Ư�� �ð��� ȸ�� �� MP���")]
    [SerializeField] private int m_AutoMPHealAmount = 12;

    [Tooltip("HP ȸ�� �� �ð�")]
    [SerializeField] private float m_AutoHPHealTime = 0.1f;

    [Tooltip("MP ȸ�� �� �ð�")]
    [SerializeField] private float m_AutoMPHealTime = 0.1f;

    [Tooltip("�޸��⸦ ������ �ּ� MP")]
    [SerializeField] private int m_StartRunningMP = 100;

    [Tooltip("�޸��� MP �Ҹ�")]
    [SerializeField] private int m_RunningMP = 4;

    [Tooltip("���� MP �Ҹ�")]
    [SerializeField] private int m_JumpingMP = 180;
    #endregion

    private int m_CurrentPlayerHP;
    private int m_CurrentPlayerMP;
    private int m_CallCount;

    private float m_AmountPlayerHP = 1;
    private float m_AmountPlayerMP = 1;

    private float m_HPTimer;
    private float m_MPTimer;

    private const float m_RealToAmountConst = 0.001f;
    private const int m_AmountToRealConst = 1000;

    private WeaponInfo m_CurrentWeaponInfo;

    #region Property
    public Inventory Inventory { get => m_Inventory; }
    public PlayerState m_PlayerState { get; } = new PlayerState();
    public Action<bool> GrabAction { get; set; }
    public Action<Transform, Transform, Transform, Transform> GrabPoint {get;set;}
    public Func<int, int, WeaponData> WeaponDataFunc { get; set; }

    public bool IsAlive { get; set; } = true;
    public int PlayerMaxHP { get; } = 1000;
    public int PlayerMaxMP { get; } = 1000;
    public int PlayerHP 
    {
        get => m_CurrentPlayerHP;
        set
        {
            m_CurrentPlayerHP = Mathf.Clamp(value, 0, m_MaxPlayerHP);
            m_AmountPlayerHP = m_CurrentPlayerHP * m_RealToAmountConst;
        } 
    }

    public int PlayerMP
    {
        get => m_CurrentPlayerMP;
        set
        {
            m_CurrentPlayerMP = Mathf.Clamp(value, 0, m_MaxPlayerMP);
            m_AmountPlayerMP = m_CurrentPlayerMP * m_RealToAmountConst;
        }
    }
    #endregion

    #region UnityFuction
    private void Awake()
    {
        PlayerHP = m_MaxPlayerHP;
        PlayerMP = m_MaxPlayerMP;
        m_PlayerUIManager.Init(PlayerMaxHP, PlayerMaxMP, m_AmountToRealConst, m_RealToAmountConst, m_Inventory.HealKitHavingCount);
    }

    private void Update()
    {
        if (!IsAlive) return;

        if ((m_HPTimer += Time.deltaTime) >= m_AutoHPHealTime)
        {
            m_HPTimer = 0;
            PlayerHP += m_AutoHPHealAmount;
            m_PlayerUIManager.UpdatePlayerHP(m_AmountPlayerHP);
        }

        if ((m_MPTimer += Time.deltaTime) >= m_AutoMPHealTime)
        {
            m_MPTimer = 0;
            PlayerMP += m_AutoMPHealAmount;
            m_PlayerUIManager.UpdatePlayerMP(m_AmountPlayerMP);
        }
    }
    #endregion

    #region CheckBehaviour
    public bool CanStartRunning() => PlayerMP > m_StartRunningMP;

    public bool CanRunning()
    {
        if (PlayerMP < m_RunningMP) return false;
        UpdatePlayerMP(m_RunningMP);
        return true;
    }

    public bool CanJumping()
    {
        if (PlayerMP < m_JumpingMP) return false;
        UpdatePlayerMP(m_JumpingMP);
        return true;
    }
    #endregion

    #region Change related
    /// <summary>
    /// ���� ���� ��
    /// </summary>
    /// <param name="equipingWeaponType">���� Ÿ�� (���� ��ȣ�� ��ġ)</param>
    /// <param name="attackType">���� �Ѿ� Ÿ�� (������ ǥ�ÿ�)</param>
    /// <param name="weaponImage">���� ������ �̹���</param>
    public void ChangeWeapon(int equipingWeaponType, AttackType attackType, FireMode fireMode, Sprite weaponImage)
    {
        m_CurrentWeaponInfo = m_Inventory.WeaponInfo[equipingWeaponType];
        m_PlayerUIManager.ChangeWeapon(equipingWeaponType, (int)attackType, GetBitPosition((int)fireMode), m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, weaponImage);
        m_PlayerUIManager.DisplayReloadImage(m_CurrentWeaponInfo.IsActiveReloadImage());
    }

    /// <summary>
    /// ���Ÿ� ���� ��ݸ�� ���� ��
    /// </summary>
    /// <param name="fireMode">��� ���</param>
    public void ChangeFireMode(FireMode fireMode)
    {
        m_PlayerUIManager.ChangeFireMode(GetBitPosition((int)fireMode));
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
    #endregion

    #region Hit and Attack
    /// <summary>
    /// ���� �� ������ �¾��� �� �ߵ�
    /// </summary>
    public void HitEnemy() => m_PlayerUIManager.HitEnemy();
    
    /// <summary>
    /// ���Ÿ� ���� ��� ��
    /// </summary>
    public void RangeWeaponFire()
    {
        m_CurrentWeaponInfo.m_CurrentRemainBullet--;
        m_PlayerUIManager.RangeWeaponFire(m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.IsActiveReloadImage());
    }
    #endregion

    #region Reload related
    public void RangeWeaponCountingReload()
    {
        m_CurrentWeaponInfo.m_MagazineRemainBullet--;
        m_CurrentWeaponInfo.m_CurrentRemainBullet++;

        m_PlayerUIManager.RangeWeaponReload(m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, m_CurrentWeaponInfo.IsActiveReloadImage());
    }

    public void RangeWeaponReload()
    {
        int totalBullet = m_CurrentWeaponInfo.m_CurrentRemainBullet + m_CurrentWeaponInfo.m_MagazineRemainBullet;
        int maxBullet;
        if (totalBullet > (maxBullet = m_CurrentWeaponInfo.m_MaxBullet))
        {
            m_CurrentWeaponInfo.m_MagazineRemainBullet -= maxBullet - m_CurrentWeaponInfo.m_CurrentRemainBullet;
            m_CurrentWeaponInfo.m_CurrentRemainBullet = maxBullet;
        }
        else
        {
            m_CurrentWeaponInfo.m_MagazineRemainBullet = 0;
            m_CurrentWeaponInfo.m_CurrentRemainBullet = totalBullet;
        }
        m_PlayerUIManager.RangeWeaponReload(m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, m_CurrentWeaponInfo.IsActiveReloadImage());
    }

    //private bool IsActiveReloadImage() 
    //    => m_CurrentWeaponInfo.m_CurrentRemainBullet < m_CurrentWeaponInfo.m_MaxBullet * 0.5f && 
    //       m_CurrentWeaponInfo.m_MaxBullet != 0 && m_CurrentWeaponInfo.m_MagazineRemainBullet != 0;
    #endregion

    #region HP,MP related
    /// <summary>
    /// ��Ŷ�� ����ϰų� ����� ��
    /// </summary>
    /// <param name="value">����ϰų� ���� ���� (���� ����)</param>
    public void UsingHealKit(int value)
    {
        int remainHealKit = m_Inventory.HealKitHavingCount += value;
        m_PlayerUIManager.UsingHealKit(remainHealKit);
        if (value < 0) UpdatePlayerHP(-m_HealKitAmount);
    }

    /// <summary>
    /// �÷��̾ �ǰݴ��ϰų� ��Ŷ ��� ��
    /// </summary>
    /// <param name="value">�ǰݽ� �Ǵ� ȸ���� ���</param>
    public void UpdatePlayerHP(int value)
    {
        PlayerHP -= value;
        m_PlayerUIManager.UpdatePlayerHP(m_AmountPlayerHP);
        if (PlayerHP <= 0)
        {
            Manager.GameManager.GameEnd();
        }
    }

    /// <summary>
    /// �÷��̾ Ư���� ������ ����� ��
    /// </summary>
    /// <param name="value">���۽� �Ǵ� ȸ���� ���</param>
    public void UpdatePlayerMP(int value)
    {
        PlayerMP -= value;
        m_PlayerUIManager.UpdatePlayerMP(m_AmountPlayerMP);
    }
    #endregion

    #region Hit related
    /// <summary>
    /// �Լ� ���� ȣ��
    /// </summary>
    /// <param name="target">������ ���</param>
    /// <param name="damage">������</param>
    /// <param name="attackType">���� ����</param>
    public void PlayerHit(Transform target, int damage, AttackType attackType)
    {
        if(attackType == AttackType.Grab) GrabAction?.Invoke(true);
        else if (attackType == AttackType.Explosion) damage = (int)(damage * 0.5f);

        UpdatePlayerHP(damage);
        m_PlayerUIManager.DisplayHitDirection(target);
    }

    /// <summary>
    /// UnityEvent�� ȣ���
    /// </summary>
    /// <param name="damage">������</param>
    /// <param name="attackType">���� ����</param>
    public void PlayerHit(int damage, AttackType attackType)
    {
        if (attackType == AttackType.Explosion) damage = (int)(damage * 0.5f);
        UpdatePlayerHP(damage);
    }
    
    public void EndGrab() => GrabAction?.Invoke(false);
    #endregion

    #region SkillEventUI related
    /// <summary>
    /// UnityEvent�� ȣ��
    /// UI�� ���� ȹ�� �̺�Ʈ Ŭ���� �ߵ�
    /// </summary>
    /// <param name="slotNumber">���� Ÿ�� (���� ��ȣ�� ��ġ)</param>
    /// <param name="weaponIndex">Ÿ�� ���� ���� �ε���</param>
    public void GetWeapon(int slotNumber, int weaponIndex)
    {
        WeaponData weaponData = (WeaponData)(WeaponDataFunc?.Invoke(slotNumber, weaponIndex));
        m_PlayerUIManager.UpdateWeaponSlot(slotNumber, weaponData.m_Icon);

        // ���¸� ���� ���
        m_Inventory.WeaponInfo[slotNumber].m_HavingWeaponIndex = weaponIndex;
        m_Inventory.WeaponInfo[slotNumber].m_CurrentRemainBullet = weaponData.m_MaxBullet;
        m_Inventory.WeaponInfo[slotNumber].m_MaxBullet = weaponData.m_MaxBullet;
        m_Inventory.WeaponInfo[slotNumber].m_MagazineRemainBullet = weaponData.m_MagazineRemainBullet;
    }

    public void GetSupply(int slotNumber, int amount)
    {
        m_Inventory.WeaponInfo[slotNumber].m_MagazineRemainBullet += amount;
    }

    public void MaxBulletUp(float amount)
    {
        //��� ����� ��ȸ
        //or WeaponManager���� ��ȸ
    }

    public void HealKitRateUp(int amount)
    {
        m_HealKitAmount += amount;
    }

    public void HealthRecoverUp(int amount)
    {
        m_AutoHPHealAmount += amount;
    }

    public void MaxHealthUp(int amount)
    {
        m_MaxPlayerHP += amount;
    }

    public void MaxStaminaUp(int amount)
    {
        m_MaxPlayerMP += amount;
    }

    public void StaminaConsumeDown(int amount)
    {
        m_CallCount++;
        if (m_CallCount == 1)
        {
            m_RunningMP = 3;
            m_JumpingMP = 150;
        }
        else if (m_CallCount == 2)
        {
            m_RunningMP = 2;
            m_JumpingMP = 120;
        }
        else if(m_CallCount == 3)
        {
            m_RunningMP = 1;
            m_JumpingMP = 90;
        }
    }

    public void StaminaRecoverUp(int amount)
    {
        m_AutoMPHealAmount += amount;
    }

    #endregion
}
