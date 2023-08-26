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


    [Header("HP")]
    [Tooltip("�ִ� HP")]
    [SerializeField] private int m_MaxPlayerHP = 1000;

    [Tooltip("Ư�� �ð��� ȸ�� �� HP���")]
    [SerializeField] private int m_AutoHPHealAmount = 1;

    [Tooltip("HP ȸ�� �� �ð�")]
    [SerializeField] private float m_AutoHPHealTime = 0.1f;


    [Header("MP")]
    [Tooltip("�ִ� MP")]
    [SerializeField] private int m_MaxPlayerMP = 1000;

    [Tooltip("Ư�� �ð��� ȸ�� �� MP���")]
    [SerializeField] private int m_AutoMPHealAmount = 10;

    [Tooltip("MP ȸ�� �� �ð�")]
    [SerializeField] private float m_AutoMPHealTime = 0.1f;

    [Tooltip("���� MP �Ҹ�")]
    [SerializeField] private int m_JumpingMP = 180;

    [Tooltip("�޸��� MP �Ҹ�")]
    [SerializeField] private int m_RunningMP = 4;

    [Tooltip("�޸��⸦ ������ �ּ� MP")]
    [SerializeField] private int m_StartRunningMP = 100;


    [Header("GE")]
    [Tooltip("�߷� ���� ������ �ִ뷮")]
    [SerializeField] private int m_MaxPlayerGE = 1000;

    [Tooltip("�߷� ���� ������ ������ ȸ����")]
    [SerializeField] private int m_AutoGEHealAmount = 10;

    [Tooltip("�߷� ���� ȸ�� �� �ð� ����")]
    [SerializeField] private float m_AutoGEHealTime = 0.1f;

    [Tooltip("�߷� ���� ������ �Ҹ�")]
    [SerializeField] private int m_GEConsumeAmount = 500;


    [Header("TE")]
    [Tooltip("�ð� ���� ������ �ִ뷮")]
    [SerializeField] private int m_MaxPlayerTE = 1000;

    [Tooltip("�ð� ���� ������ ������ ȸ����")]
    [SerializeField] private int m_AutoTEHealAmount = 10;

    [Tooltip("�ð� ���� ȸ�� �� �ð� ����")]
    [SerializeField] private float m_AutoTEHealTime = 0.1f;

    [Tooltip("�ð� ���� ������ �Ҹ�")]
    [SerializeField] private int m_TEConsumeAmount = 10;

    [Tooltip("�ð� ���� ������ �ּ� TE")]
    [SerializeField] private int m_StartTimeChangeTE = 100;
    #endregion

    private int m_CurrentPlayerHP;
    private int m_CurrentPlayerMP;
    private int m_CurrentPlayerGE;
    private int m_CurrentPlayerTE;

    private float m_AmountPlayerHP = 1;
    private float m_AmountPlayerMP = 1;
    private float m_AmountPlayerGE = 1;
    private float m_AmountPlayerTE = 1;

    private float m_HPTimer;
    private float m_MPTimer;
    private float m_GETimer;
    private float m_TETimer;

    private float m_RealToAmountHPConst = 0.001f;
    private float m_RealToAmountMPConst = 0.001f;
    private float m_RealToAmountGEConst = 0.001f;
    private float m_RealToAmountTEConst = 0.001f;

    private int m_AmountToRealHPConst = 1000;
    private int m_AmountToRealMPConst = 1000;
    private int m_AmountToRealGEConst = 1000;
    private int m_AmountToRealTEConst = 1000;

    private int m_CallCount;

    private bool m_IsTimeSlowing;
    #region Property
    public Inventory Inventory { get => m_Inventory; }
    private WeaponInfo CurrentWeaponInfo { get; set; }
    public PlayerState PlayerState { get; } = new PlayerState();

    public Action ReInit { get; set; }
    public Action<bool> GrabAction { get; set; }
    public Action<Transform, Transform, Transform, Transform> GrabPoint {get;set;}
    public Func<int, int, WeaponData> WeaponDataFunc { get; set; }
    

    public bool IsAlive { get; set; } = true;
    public bool IsSameMaxCurrentHP { get => PlayerHP == PlayerMaxHP; }

    #region Player current stat
    public int PlayerHP 
    {
        get => m_CurrentPlayerHP;
        private set
        {
            m_CurrentPlayerHP = Mathf.Clamp(value, 0, PlayerMaxHP);
            m_AmountPlayerHP = m_CurrentPlayerHP * m_RealToAmountHPConst;
        } 
    }

    public int PlayerMP
    {
        get => m_CurrentPlayerMP;
        private set
        {
            m_CurrentPlayerMP = Mathf.Clamp(value, 0, PlayerMaxMP);
            m_AmountPlayerMP = m_CurrentPlayerMP * m_RealToAmountMPConst;
        }
    }

    private int PlayerGE
    {
        get => m_CurrentPlayerGE;
        set
        {
            m_CurrentPlayerGE = Mathf.Clamp(value, 0, m_MaxPlayerGE);
            m_AmountPlayerGE = m_CurrentPlayerGE * m_RealToAmountGEConst;
            m_PlayerUIManager.UpdatePlayerGE(m_AmountPlayerGE);
        }
    }

    private int PlayerTE
    {
        get => m_CurrentPlayerTE;
        set
        {
            m_CurrentPlayerTE = Mathf.Clamp(value,0, m_MaxPlayerTE);
            m_AmountPlayerTE = m_CurrentPlayerTE * m_RealToAmountTEConst;
            m_PlayerUIManager.UpdatePlayerTE(m_AmountPlayerTE);
        }
    }
    #endregion
    #region Player max stat
    private int PlayerMaxHP
    {
        get => m_MaxPlayerHP;
        set
        {
            m_MaxPlayerHP = value;
            m_AmountToRealHPConst = m_MaxPlayerHP;
            m_RealToAmountHPConst = 1 / (float)m_AmountToRealHPConst;
        }
    }

    private int PlayerMaxMP
    {
        get => m_MaxPlayerMP;
        set
        {
            m_MaxPlayerMP = value;
            m_AmountToRealMPConst = m_MaxPlayerMP;
            m_RealToAmountMPConst = 1 / (float)m_AmountToRealMPConst;
        }
    }

    private int PlayerMaxGE
    {
        get => m_MaxPlayerGE;
        set
        {
            m_MaxPlayerGE = value;
            m_AmountToRealGEConst = m_MaxPlayerGE;
            m_RealToAmountGEConst = 1 / (float)m_AmountToRealGEConst;
        }
    }

    private int PlayerMaxTE
    {
        get => m_MaxPlayerTE;
        set
        {
            m_MaxPlayerTE = value;
            m_AmountToRealTEConst = m_MaxPlayerTE;
            m_RealToAmountTEConst = 1 / (float)m_AmountToRealTEConst;
        }
    }
    #endregion
    #endregion

    #region UnityFuction
    private void Awake()
    {
        PlayerHP = m_MaxPlayerHP;
        PlayerMP = m_MaxPlayerMP;
        PlayerGE = m_MaxPlayerGE;
        PlayerTE = m_MaxPlayerTE;
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

        if((m_GETimer += Time.deltaTime) >= m_AutoGEHealTime)
        {
            m_GETimer = 0;
            PlayerGE += m_AutoGEHealAmount;
        }

        if((m_TETimer += Time.deltaTime) >= m_AutoTEHealTime &&
            !m_IsTimeSlowing)
        {
            m_TETimer = 0;
            PlayerTE += m_AutoTEHealAmount;
        }
    }
    #endregion

    // HP, MP �Ҹ��ϴ°� deltaTime �ȽἭ �̻��� ���� ����
    //�׽�Ʈ �ϰ� ���� �ʿ��ϸ� ������ ��!!
    #region CheckBehaviour
    public bool CanStartRunning() => PlayerMP > m_StartRunningMP;
    public bool CanStartTimeSlow() => PlayerTE > m_StartTimeChangeTE;

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

    public bool CanChangeGravity()
    {
        if (PlayerGE < m_GEConsumeAmount) return false;
        return true;
    }

    public void UseGravityChange()
    {
        PlayerGE -= m_GEConsumeAmount;
    }

    public bool CanTimeSlow()
    {
        return true;
    }
    #endregion

    #region Change related
    /// <summary>
    /// ���� ���� ��
    /// </summary>
    /// <param name="slotNumber">���� Ÿ�� (���� ��ȣ�� ��ġ)</param>
    /// <param name="attackType">���� �Ѿ� Ÿ�� (������ ǥ�ÿ�)</param>
    /// <param name="weaponImage">���� ������ �̹���</param>
    public void ChangeWeapon(int slotNumber, AttackType attackType, FireMode fireMode, Sprite weaponImage)
    {
        CurrentWeaponInfo = m_Inventory.WeaponInfo[slotNumber];
        m_PlayerUIManager.ChangeWeapon(slotNumber, (int)attackType, GetBitPosition((int)fireMode), CurrentWeaponInfo.m_CurrentRemainBullet, CurrentWeaponInfo.m_MagazineRemainBullet, weaponImage);
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