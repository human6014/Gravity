using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private UI.Manager.PlayerUIManager m_PlayerUIManager;
    [SerializeField] private Inventory m_Inventory;

    [Tooltip("힐 킷 회복량")]
    [SerializeField] private int HealKitAmount = 400;

    [Tooltip("최대 HP")]
    [SerializeField] private int m_MaxPlayerHP = 1000;

    [Tooltip("최대 MP")]
    [SerializeField] private int m_MaxPlayerMP = 1000;


    [Tooltip("특정 시간당 회복 할 HP계수")]
    [SerializeField] private int m_AutoHPHealAmount = 2;

    [Tooltip("특정 시간당 회복 할 MP계수")]
    [SerializeField] private int m_AutoMPHealAmount = 12;

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

    private const float m_RealToAmountConst = 0.001f;
    private const int m_AmountToRealConst = 1000;

    private WeaponInfo m_CurrentWeaponInfo;

    #region Property
    public Inventory Inventory { get => m_Inventory; }
    public PlayerState m_PlayerState { get; } = new PlayerState();
    public System.Action<bool> GrabAction { get; set; }
    public System.Action<Transform, Transform, Transform, Transform> GrabPoint {get;set;}

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

    /// <summary>
    /// 무기 변경 시
    /// </summary>
    /// <param name="equipingWeaponType">무기 타입 (슬롯 번호랑 일치)</param>
    /// <param name="attackType">무기 총알 타입 (아이콘 표시용)</param>
    /// <param name="weaponImage">무기 아이콘 이미지</param>
    public void ChangeWeapon(int equipingWeaponType, AttackType attackType, FireMode fireMode, Sprite weaponImage)
    {
        m_CurrentWeaponInfo = m_Inventory.WeaponInfo[equipingWeaponType];
        m_PlayerUIManager.ChangeWeapon(equipingWeaponType, (int)attackType, GetBitPosition((int)fireMode), m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, weaponImage);
        m_PlayerUIManager.DisplayReloadImage(IsActiveReloadImage());
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

    public void HitEnemy()
    {
        m_PlayerUIManager.HitEnemy();
    }

    /// <summary>
    /// 원거리 무기 사격 시
    /// </summary>
    public void RangeWeaponFire()
    {
        //m_Inventory.GetCurrentRemainBullet((int)m_CurrentEquipingWeaponType)
        m_CurrentWeaponInfo.m_CurrentRemainBullet--;

        m_PlayerUIManager.RangeWeaponFire(m_CurrentWeaponInfo.m_CurrentRemainBullet, IsActiveReloadImage());
    }

    public void RangeWeaponCountingReload()
    {
        m_CurrentWeaponInfo.m_MagazineRemainBullet--;
        m_CurrentWeaponInfo.m_CurrentRemainBullet++;

        m_PlayerUIManager.RangeWeaponReload(m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, IsActiveReloadImage());
    }

    public void RangeWeaponReload()
    {
        int totalBullet = m_CurrentWeaponInfo.m_CurrentRemainBullet + m_CurrentWeaponInfo.m_MagazineRemainBullet;
        
        if (totalBullet > m_CurrentWeaponInfo.m_MaxBullet)
        {
            m_CurrentWeaponInfo.m_MagazineRemainBullet -= m_CurrentWeaponInfo.m_MaxBullet - m_CurrentWeaponInfo.m_CurrentRemainBullet;
            m_CurrentWeaponInfo.m_CurrentRemainBullet = m_CurrentWeaponInfo.m_MaxBullet;
        }
        else
        {
            m_CurrentWeaponInfo.m_MagazineRemainBullet = 0;
            m_CurrentWeaponInfo.m_CurrentRemainBullet = totalBullet;
        }
        m_PlayerUIManager.RangeWeaponReload(m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, IsActiveReloadImage());
    }

    private bool IsActiveReloadImage() 
        => m_CurrentWeaponInfo.m_CurrentRemainBullet < m_CurrentWeaponInfo.m_MaxBullet * 0.5f && 
           m_CurrentWeaponInfo.m_MaxBullet != 0 && m_CurrentWeaponInfo.m_MagazineRemainBullet != 0;
    

    /// <summary>
    /// 무기를 얻었을 때
    /// </summary>
    /// <param name="slotNumber">무기 타입 (슬롯 번호랑 일치)</param>
    public void GetWeapon(int slotNumber, Sprite sprite, int weaponIndex, int maxBullet)    //아직 안댐
    {
        m_Inventory.WeaponInfo[slotNumber].m_HavingWeaponIndex = weaponIndex;
        m_PlayerUIManager.UpdateWeaponSlot(slotNumber, sprite);
    }

    /// <summary>
    /// 힐킷을 사용하거나 얻었을 때
    /// </summary>
    /// <param name="value">사용하거나 얻은 개수 (음수 가능)</param>
    public void UsingHealKit(int value)
    {
        int remainHealKit = m_Inventory.HealKitHavingCount += value;
        m_PlayerUIManager.UsingHealKit(remainHealKit, m_AmountPlayerHP);
        UpdatePlayerHP(-HealKitAmount);
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



    public void PlayerHit(Transform target, int damage, AttackType attackType)
    {
        if(attackType == AttackType.Grab) GrabAction?.Invoke(true);
        else if (attackType == AttackType.Explosion) damage = (int)(damage * 0.5f);

        UpdatePlayerHP(damage);
        m_PlayerUIManager.DisplayHitDirection(target);
    }

    //UnityEvent 호출
    public void PlayerHit(int damage, AttackType attackType)
    {
        if (attackType == AttackType.Explosion) damage = (int)(damage * 0.5f);
        UpdatePlayerHP(damage);
    }

    public void EndGrab() => GrabAction?.Invoke(false);
}
