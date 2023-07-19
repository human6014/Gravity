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
    [SerializeField] private int m_AutoHPHealAmount = 1;

    [Tooltip("Ư�� �ð��� ȸ�� �� MP���")]
    [SerializeField] private int m_AutoMPHealAmount = 10;

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

    private float m_AmountPlayerHP = 1;
    private float m_AmountPlayerMP = 1;

    private float m_HPTimer;
    private float m_MPTimer;

    private float m_RealToAmountHPConst = 0.001f;
    private float m_RealToAmountMPConst = 0.001f;

    private int m_AmountToRealHPConst = 1000;
    private int m_AmountToRealMPConst = 1000;

    private int m_CallCount;

    #region Property
    public Inventory Inventory { get => m_Inventory; }
    private WeaponInfo CurrentWeaponInfo { get; set; }
    public PlayerState PlayerState { get; } = new PlayerState();

    public Action ReInit { get; set; }
    public Action<bool> GrabAction { get; set; }
    public Action<Transform, Transform, Transform, Transform> GrabPoint {get;set;}
    public Func<int, int, WeaponData> WeaponDataFunc { get; set; }
    

    public bool IsAlive { get; set; } = true;
    public bool IsSameMaxCurrentHP { get => PlayerHP == m_MaxPlayerHP; }
    public int PlayerHP 
    {
        get => m_CurrentPlayerHP;
        set
        {
            m_CurrentPlayerHP = Mathf.Clamp(value, 0, m_MaxPlayerHP);
            m_AmountPlayerHP = m_CurrentPlayerHP * m_RealToAmountHPConst;
        } 
    }

    public int PlayerMP
    {
        get => m_CurrentPlayerMP;
        set
        {
            m_CurrentPlayerMP = Mathf.Clamp(value, 0, m_MaxPlayerMP);
            m_AmountPlayerMP = m_CurrentPlayerMP * m_RealToAmountMPConst;
        }
    }
    
    public int PlayerMaxHP
    {
        get => m_MaxPlayerHP;
        set
        {
            m_MaxPlayerHP = value;
            m_AmountToRealHPConst = m_MaxPlayerHP;
            m_RealToAmountHPConst = 1 / (float)m_AmountToRealHPConst;
        }
    }

    public int PlayerMaxMP
    {
        get => m_MaxPlayerMP;
        set
        {
            m_MaxPlayerMP = value;
            m_AmountToRealMPConst = m_MaxPlayerMP;
            m_RealToAmountMPConst = 1 / (float)m_AmountToRealMPConst;
        }
    }

    #endregion

    #region UnityFuction
    private void Awake()
    {
        PlayerHP = m_MaxPlayerHP;
        PlayerMP = m_MaxPlayerMP;
        m_PlayerUIManager.Init(m_MaxPlayerHP, m_MaxPlayerMP, m_AmountToRealHPConst, m_RealToAmountHPConst, m_Inventory.HealKitHavingCount);
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

        if ((m_MPTimer += Time.deltaTime) >= m_AutoMPHealTime &&
            PlayerState.PlayerBehaviorState != PlayerBehaviorState.Running &&
            PlayerState.PlayerBehaviorState != PlayerBehaviorState.Jumping)
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
        CurrentWeaponInfo = m_Inventory.WeaponInfo[equipingWeaponType];
        m_PlayerUIManager.ChangeWeapon(equipingWeaponType, (int)attackType, GetBitPosition((int)fireMode), CurrentWeaponInfo.m_CurrentRemainBullet, CurrentWeaponInfo.m_MagazineRemainBullet, weaponImage);
        m_PlayerUIManager.DisplayReloadImage(CurrentWeaponInfo.IsActiveReloadImage());
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
        CurrentWeaponInfo.m_CurrentRemainBullet--;
        m_PlayerUIManager.RangeWeaponFire(CurrentWeaponInfo.m_CurrentRemainBullet, CurrentWeaponInfo.IsActiveReloadImage());
    }
    #endregion

    #region Reload related
    public void RangeWeaponCountingReload()
    {
        CurrentWeaponInfo.m_MagazineRemainBullet--;
        CurrentWeaponInfo.m_CurrentRemainBullet++;

        m_PlayerUIManager.RangeWeaponReload(CurrentWeaponInfo.m_MagazineRemainBullet);
        m_PlayerUIManager.RangeWeaponFire(CurrentWeaponInfo.m_CurrentRemainBullet, CurrentWeaponInfo.IsActiveReloadImage());
    }

    public void RangeWeaponReload()
    {
        int totalBullet = CurrentWeaponInfo.m_CurrentRemainBullet + CurrentWeaponInfo.m_MagazineRemainBullet;
        int maxBullet;
        if (totalBullet > (maxBullet = CurrentWeaponInfo.MaxBullet))
        {
            CurrentWeaponInfo.m_MagazineRemainBullet -= maxBullet - CurrentWeaponInfo.m_CurrentRemainBullet;
            CurrentWeaponInfo.m_CurrentRemainBullet = maxBullet;
        }
        else
        {
            CurrentWeaponInfo.m_MagazineRemainBullet = 0;
            CurrentWeaponInfo.m_CurrentRemainBullet = totalBullet;
        }
        m_PlayerUIManager.RangeWeaponReload(CurrentWeaponInfo.m_MagazineRemainBullet);
        m_PlayerUIManager.RangeWeaponFire(CurrentWeaponInfo.m_CurrentRemainBullet, CurrentWeaponInfo.IsActiveReloadImage());
    }
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
        else if (attackType == AttackType.OnlyDamage && PlayerHP - damage <= 0) damage = PlayerHP - 1;
        
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
        //���� �ڱⰡ ��� �ڱⰡ �´°� ����
        if ((int)attackType >= 1 && (int)attackType <= 4) return;
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
        m_Inventory.WeaponInfo[slotNumber].MaxBullet += weaponData.m_MaxBullet;
        m_Inventory.WeaponInfo[slotNumber].m_MagazineRemainBullet = weaponData.m_MagazineRemainBullet;
    }

    public void GetSupply(int slotNumber, int amount)
    {
        if (slotNumber == 4 && m_Inventory.WeaponInfo[slotNumber].m_CurrentRemainBullet <= 0)
        {
            m_Inventory.WeaponInfo[slotNumber].m_CurrentRemainBullet = 1;
            m_Inventory.WeaponInfo[slotNumber].m_MagazineRemainBullet = amount - 1;
            if (CurrentWeaponInfo == m_Inventory.WeaponInfo[slotNumber])
            {
                m_PlayerUIManager.RangeWeaponFire(1,false);
                ReInit?.Invoke();
            }
        }
        else m_Inventory.WeaponInfo[slotNumber].m_MagazineRemainBullet += amount;
        m_PlayerUIManager.RangeWeaponReload(CurrentWeaponInfo.m_MagazineRemainBullet);
    }

    public void MaxBulletUp(float amount)
    {
        for(int slotNumber = 1; slotNumber <= 3; slotNumber++)
            m_Inventory.WeaponInfo[slotNumber].MaxBullet += (int)amount;
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
        PlayerMaxHP += amount;
        PlayerHP += amount;
        m_PlayerUIManager.UpdatePlayerMaxHP(m_MaxPlayerHP, m_AmountToRealHPConst);
        m_PlayerUIManager.UpdatePlayerHP(m_AmountPlayerHP);
    }

    public void MaxStaminaUp(int amount)
    {
        PlayerMaxMP += amount;
        PlayerMP += amount;
        m_PlayerUIManager.UpdatePlayerMP(m_AmountPlayerMP);
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
