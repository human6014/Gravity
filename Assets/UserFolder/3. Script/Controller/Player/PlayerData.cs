using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PlayerData : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private UI.Manager.PlayerUIManager m_PlayerUIManager;
    [SerializeField] private Inventory m_Inventory;

    [Header("SkillPoint")]
    [Tooltip("플레이어가 소유한 스킬 포인트")]
    [SerializeField] private int m_SkillPoint = 100;

    [Tooltip("스킬 포인트를 얻기 위한 킬 수")]
    [SerializeField] private int m_GetSkillKillCount = 5;

    [Header("")]
    [Tooltip("힐 킷 회복량")]
    [SerializeField] private int m_HealKitAmount = 400;

    [Tooltip("방어력")]
    [SerializeField] private int m_Defense = 0;


    [Header("HP")]
    [Tooltip("최대 HP")]
    [SerializeField] private int m_MaxPlayerHP = 1000;

    [Tooltip("특정 시간당 회복 할 HP계수")]
    [SerializeField] private int m_AutoHPHealAmount = 2;

    [Tooltip("HP 회복 할 시간")]
    [SerializeField] private float m_AutoHPHealTime = 0.1f;


    [Header("MP")]
    [Tooltip("최대 MP")]
    [SerializeField] private int m_MaxPlayerMP = 1000;

    [Tooltip("특정 시간당 회복 할 MP계수")]
    [SerializeField] private int m_AutoMPHealAmount = 10;

    [Tooltip("MP 회복, 소모 할 시간")]
    [SerializeField] private float m_AutoMPTime = 0.1f;

    [Tooltip("점프 MP 소모량")]
    [SerializeField] private int m_JumpingMPConsumeAmount = 170;

    [Tooltip("달리기 MP 소모량")]
    [SerializeField] private int m_RunningMPConsumeAmount = 17;

    [Tooltip("달리기를 시작할 최소 MP")]
    [SerializeField] private int m_StartRunningMP = 100;


    [Header("GE")]
    [Tooltip("중력 변경 에너지 최대량")]
    [SerializeField] private int m_MaxPlayerGE = 1000;

    [Tooltip("중력 변경 에너지 단위당 회복량")]
    [SerializeField] private int m_AutoGEHealAmount = 3;

    [Tooltip("중력 변경 회복 할 시간 단위")]
    [SerializeField] private float m_AutoGETime = 0.1f;

    [Tooltip("중력 변경 에너지 소모량")]
    [SerializeField] private int m_GEConsumeAmount = 600;


    [Header("TE")]
    [Tooltip("시간 변경 에너지 최대량")]
    [SerializeField] private int m_MaxPlayerTE = 1000;

    [Tooltip("시간 변경 에너지 단위당 회복량")]
    [SerializeField] private int m_TEHealAmount = 6;

    [Tooltip("시간 변경 에너지 소모량")]
    [SerializeField] private int m_TEConsumeAmount = 20;

    [Tooltip("시간 변경 회복, 소모 할 시간 단위")]
    [SerializeField] private float m_AutoTETime = 0.1f;

    [Tooltip("시간 변경 시작할 최소 TE")]
    [SerializeField] private int m_StartTimeChangeTE = 100;

    [SerializeField] UnityEngine.Events.UnityEvent<int> ChangeSkillPointEvent;
    #endregion

    #region Field
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

    private int m_LifeCount = 0;

    private int m_DeltaTimeInterporateValue;
    private bool m_IsTimeSlow;
    #endregion

    #region Property
    private WeaponInfo CurrentWeaponInfo { get; set; }
    public Inventory Inventory { get => m_Inventory; }
    public PlayerState PlayerState { get; } = new PlayerState();


    public Action ReInit { get; set; }
    public Action StopSlowModeAction { get; set; }
    public Action<bool,Vector3> GrabAction { get; set; }
    public Action<Vector3, Vector3> ThrowingAction { get; set; }
    public Action<Transform, Transform, Transform> GrabPointAssign {get;set;}
    public Func<int, int, WeaponData> WeaponDataFunc { get; set; }
    

    public bool IsAlive { get; set; } = true;
    public bool IsSameMaxCurrentHP { get => PlayerHP == PlayerMaxHP; }
    private int PlayerSkillPoint 
    { 
        get => m_SkillPoint;
        set
        {
            m_SkillPoint = value;
            ChangeSkillPointEvent?.Invoke(m_SkillPoint);
        }
    }
    
    #region Player current stat
    private int PlayerHP 
    {
        get => m_CurrentPlayerHP;
        set
        {
            m_CurrentPlayerHP = Mathf.Clamp(value, 0, PlayerMaxHP);
            m_AmountPlayerHP = m_CurrentPlayerHP * m_RealToAmountHPConst;
        } 
    }

    private int PlayerMP
    {
        get => m_CurrentPlayerMP;
        set
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
            m_CurrentPlayerGE = Mathf.Clamp(value, 0, PlayerMaxGE);
            m_AmountPlayerGE = m_CurrentPlayerGE * m_RealToAmountGEConst;
            m_PlayerUIManager.UpdatePlayerGE(m_AmountPlayerGE);
        }
    }

    private int PlayerTE
    {
        get => m_CurrentPlayerTE;
        set
        {
            m_CurrentPlayerTE = Mathf.Clamp(value,0, PlayerMaxTE);
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

    private void Start()
    {
        ChangeSkillPointEvent?.Invoke(PlayerSkillPoint);
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

        if (PlayerState.PlayerBehaviorState != PlayerBehaviorState.Running &&
            PlayerState.PlayerBehaviorState != PlayerBehaviorState.Jumping &&
            (m_MPTimer += Time.deltaTime) >= m_AutoMPTime)
        {
            m_MPTimer = 0;
            PlayerMP += m_AutoMPHealAmount;
            m_PlayerUIManager.UpdatePlayerMP(m_AmountPlayerMP);
        }

        if((m_GETimer += Time.deltaTime) >= m_AutoGETime)
        {
            m_GETimer = 0;
            PlayerGE += m_AutoGEHealAmount;
        }

        if (m_IsTimeSlow && (m_TETimer += Time.deltaTime * m_DeltaTimeInterporateValue) >= m_AutoTETime)
        {
            m_TETimer = 0;
            PlayerTE -= m_TEConsumeAmount;
            if (PlayerTE <= 0) StopSlowModeAction?.Invoke();
        }
        else if(!m_IsTimeSlow && (m_TETimer += Time.deltaTime) >= m_AutoTETime)
        {
            m_TETimer = 0;
            PlayerTE += m_TEHealAmount;
        }
    }
    #endregion

    #region CheckBehaviour
    public bool CanStartRunning() => PlayerMP > m_StartRunningMP;
    public bool CanStartTimeSlow() => PlayerTE > m_StartTimeChangeTE;

    public bool CanRunning()
    {
        if (PlayerMP < m_RunningMPConsumeAmount) return false;

        if ((m_MPTimer += Time.deltaTime) >= m_AutoMPTime)
        {
            m_MPTimer = 0;
            UpdatePlayerMP(m_RunningMPConsumeAmount);
        }
        return true;
    }

    public bool CanJumping()
    {
        if (PlayerMP < m_JumpingMPConsumeAmount) return false;
        UpdatePlayerMP(m_JumpingMPConsumeAmount);
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

    public void UseTimeSlow(bool value, float time)
    {
        m_IsTimeSlow = value;
        m_DeltaTimeInterporateValue = Mathf.RoundToInt(1 / time);

        Time.timeScale = value ? 0.25f : 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
    #endregion

    #region Change related
    /// <summary>
    /// 무기 변경 시
    /// </summary>
    /// <param name="slotNumber">무기 타입 (슬롯 번호랑 일치)</param>
    /// <param name="attackType">무기 총알 타입 (아이콘 표시용)</param>
    /// <param name="weaponImage">무기 아이콘 이미지</param>
    public void ChangeWeapon(int slotNumber, AttackType attackType, FireMode fireMode, Sprite weaponImage)
    {
        CurrentWeaponInfo = m_Inventory.WeaponInfo[slotNumber];
        m_PlayerUIManager.ChangeWeapon(slotNumber, (int)attackType, GetBitPosition((int)fireMode), CurrentWeaponInfo.m_CurrentRemainBullet, CurrentWeaponInfo.m_MagazineRemainBullet, weaponImage);
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
    /// 플레이어가 공격 시 적에게 맞았을 때 발동
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
            if (m_LifeCount >= 1)
            {
                //-----------------------------Test Here-----------------------------
                Debug.Log("Life consume");
                m_LifeCount--;
                PlayerHP = PlayerMaxHP;
                m_PlayerUIManager.UpdatePlayerHP(m_AmountPlayerHP);
            }
            else FindObjectOfType<Manager.GameManager>().GameEnd();
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
        if (attackType == AttackType.OnlyDamage)
        {
            if (PlayerHP - damage <= 0) damage = PlayerHP - 1;
        }
        else
        {
            if (attackType == AttackType.Grab) GrabAction?.Invoke(true, Vector3.zero);
            else if (attackType == AttackType.Explosion) damage = (int)(damage * 0.25f);
            
            damage = Mathf.Max(damage - m_Defense, 0);
        }

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
        if (attackType == AttackType.Explosion) damage = (int)(damage * 0.25f);
        damage = Mathf.Max(damage- m_Defense, 0);

        UpdatePlayerHP(damage);
    }
    #endregion

    #region SkillEvent related
    public void GetSkillPoint(int monsterType, int monsterNumber, int allKillCount)
    {
        if (monsterType == 1) PlayerSkillPoint++;
        else
        {
            if (monsterNumber == 4) PlayerSkillPoint += 2;
            else if (monsterNumber == 3) PlayerSkillPoint++;
        }
        if(allKillCount % 5 == 0) PlayerSkillPoint++;
    }

    public void GetSkillPoint(int point) => PlayerSkillPoint += point;
    
    public bool CanSkillUpPoint(int point)
    {
        if (PlayerSkillPoint < point) return false;
        PlayerSkillPoint -= point;
        return true;
    }

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
        if(CurrentWeaponInfo != null) m_PlayerUIManager.RangeWeaponReload(CurrentWeaponInfo.m_MagazineRemainBullet);
    }

    public void MaxBulletUp(float amount)
    {
        for(int slotNumber = 1; slotNumber <= 3; slotNumber++)
            m_Inventory.WeaponInfo[slotNumber].MaxBullet += (int)amount;
    }

    #region Defense
    public void HealKitRateUp(int amount)
        => m_HealKitAmount += amount;
    
    public void HealthRecoverUp(int amount)
        => m_AutoHPHealAmount += amount;
    
    public void MaxHealthUp(int amount)
    {
        PlayerMaxHP += amount;
        PlayerHP += amount;
        m_PlayerUIManager.UpdatePlayerMaxHP(m_MaxPlayerHP, m_AmountToRealHPConst);
        m_PlayerUIManager.UpdatePlayerHP(m_AmountPlayerHP);
    }

    public void DefenseUp(int amount)
        => m_Defense += amount;
    
    public void GetLife(int amount)
        => m_LifeCount += amount;
    

    #endregion
    #region Support
    public void MaxStaminaUp(int amount)
    {
        PlayerMaxMP += amount;
        PlayerMP += amount;
        m_PlayerUIManager.UpdatePlayerMP(m_AmountPlayerMP);
    }

    public void StaminaRecoverUp(int amount)
    {
        m_AutoMPHealAmount += amount;
    }
    #endregion
    #region Special
    public void MaxTEUp(int amount)
    {
        PlayerMaxTE += amount;
        PlayerTE += amount;
    }

    public void MaxGEUp(int amount)
    {
        PlayerMaxGE += amount;
        PlayerGE += amount;
    }

    public void TERecoverUp(int amount)
        => m_TEHealAmount += amount;
    
    public void GERecoverUp(int amount)
        => m_AutoGEHealAmount += amount;
    
    public void TEConsumeDown(int amount)
        => m_TEConsumeAmount -= amount;

    public void GEConsumeDown(int amount) 
        => m_GEConsumeAmount -= amount;
    
    #endregion

    #endregion
}