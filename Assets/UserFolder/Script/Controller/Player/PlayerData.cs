using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PlayerData : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private UI.Manager.PlayerUIManager m_PlayerUIManager;
    [SerializeField] private Inventory m_Inventory;

    [Tooltip("힐 킷 회복량")]
    [SerializeField] private int m_HealKitAmount = 400;

    [Tooltip("최대 HP")]
    [SerializeField] private int m_MaxPlayerHP = 1000;

    [Tooltip("최대 MP")]
    [SerializeField] private int m_MaxPlayerMP = 1000;


    [Tooltip("특정 시간당 회복 할 HP계수")]
    [SerializeField] private int m_AutoHPHealAmount = 1;

    [Tooltip("특정 시간당 회복 할 MP계수")]
    [SerializeField] private int m_AutoMPHealAmount = 10;

    [Tooltip("HP 회복 할 시간")]
    [SerializeField] private float m_AutoHPHealTime = 0.1f;

    [Tooltip("MP 회복 할 시간")]
    [SerializeField] private float m_AutoMPHealTime = 0.1f;

    [Tooltip("달리기를 시작할 최소 MP")]
    [SerializeField] private int m_StartRunningMP = 100;

    [Tooltip("달리기 MP 소모량")]
    [SerializeField] private int m_RunningMP = 4;

    [Tooltip("점프 MP 소모량")]
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
    /// 무기 변경 시
    /// </summary>
    /// <param name="equipingWeaponType">무기 타입 (슬롯 번호랑 일치)</param>
    /// <param name="attackType">무기 총알 타입 (아이콘 표시용)</param>
    /// <param name="weaponImage">무기 아이콘 이미지</param>
    public void ChangeWeapon(int equipingWeaponType, AttackType attackType, FireMode fireMode, Sprite weaponImage)
    {
        CurrentWeaponInfo = m_Inventory.WeaponInfo[equipingWeaponType];
        m_PlayerUIManager.ChangeWeapon(equipingWeaponType, (int)attackType, GetBitPosition((int)fireMode), CurrentWeaponInfo.m_CurrentRemainBullet, CurrentWeaponInfo.m_MagazineRemainBullet, weaponImage);
        m_PlayerUIManager.DisplayReloadImage(CurrentWeaponInfo.IsActiveReloadImage());
    }

    /// <summary>
    /// 원거리 무기 사격모드 변경 시
    /// </summary>
    /// <param name="fireMode">사격 모드</param>
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
    /// 공격 시 적에게 맞았을 때 발동
    /// </summary>
    public void HitEnemy() => m_PlayerUIManager.HitEnemy();
    
    /// <summary>
    /// 원거리 무기 사격 시
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
    /// 힐킷을 사용하거나 얻었을 때
    /// </summary>
    /// <param name="value">사용하거나 얻은 개수 (음수 가능)</param>
    public void UsingHealKit(int value)
    {
        int remainHealKit = m_Inventory.HealKitHavingCount += value;
        m_PlayerUIManager.UsingHealKit(remainHealKit);
        if (value < 0) UpdatePlayerHP(-m_HealKitAmount);
    }

    /// <summary>
    /// 플레이어가 피격당하거나 힐킷 사용 시
    /// </summary>
    /// <param name="value">피격시 또는 회복할 계수</param>
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
    /// 플레이어가 특수한 동작을 사용할 때
    /// </summary>
    /// <param name="value">동작시 또는 회복할 계수</param>
    public void UpdatePlayerMP(int value)
    {
        PlayerMP -= value;
        m_PlayerUIManager.UpdatePlayerMP(m_AmountPlayerMP);
    }
    #endregion

    #region Hit related
    /// <summary>
    /// 함수 직접 호출
    /// </summary>
    /// <param name="target">공격한 대상</param>
    /// <param name="damage">데미지</param>
    /// <param name="attackType">공격 종류</param>
    public void PlayerHit(Transform target, int damage, AttackType attackType)
    {
        if(attackType == AttackType.Grab) GrabAction?.Invoke(true);
        else if (attackType == AttackType.Explosion) damage = (int)(damage * 0.5f);
        else if (attackType == AttackType.OnlyDamage && PlayerHP - damage <= 0) damage = PlayerHP - 1;
        
        UpdatePlayerHP(damage);
        m_PlayerUIManager.DisplayHitDirection(target);
    }

    /// <summary>
    /// UnityEvent로 호출됨
    /// </summary>
    /// <param name="damage">데미지</param>
    /// <param name="attackType">공격 종류</param>
    public void PlayerHit(int damage, AttackType attackType)
    {
        //가끔 자기가 쏘고 자기가 맞는거 방지
        if ((int)attackType >= 1 && (int)attackType <= 4) return;
        if (attackType == AttackType.Explosion) damage = (int)(damage * 0.5f);
        UpdatePlayerHP(damage);
    }
    
    public void EndGrab() => GrabAction?.Invoke(false);
    #endregion

    #region SkillEventUI related
    /// <summary>
    /// UnityEvent로 호출
    /// UI로 무기 획득 이벤트 클릭시 발동
    /// </summary>
    /// <param name="slotNumber">무기 타입 (슬롯 번호랑 일치)</param>
    /// <param name="weaponIndex">타입 내의 무기 인덱스</param>
    public void GetWeapon(int slotNumber, int weaponIndex)
    {
        WeaponData weaponData = (WeaponData)(WeaponDataFunc?.Invoke(slotNumber, weaponIndex));
        m_PlayerUIManager.UpdateWeaponSlot(slotNumber, weaponData.m_Icon);

        // 형태만 변경 대기
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
