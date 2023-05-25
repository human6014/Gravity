using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    #region SerializeField
    [SerializeField] private UI.Manager.PlayerUIManager m_PlayerUIManager;
    [SerializeField] private Inventory m_Inventory;

    [Tooltip("�� Ŷ ȸ����")]
    [SerializeField] private int HealKitAmount = 400;

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
    /// ���� ���� ��
    /// </summary>
    /// <param name="equipingWeaponType">���� Ÿ�� (���� ��ȣ�� ��ġ)</param>
    /// <param name="attackType">���� �Ѿ� Ÿ�� (������ ǥ�ÿ�)</param>
    /// <param name="weaponImage">���� ������ �̹���</param>
    public void ChangeWeapon(int equipingWeaponType, AttackType attackType, FireMode fireMode, Sprite weaponImage)
    {
        m_CurrentWeaponInfo = m_Inventory.WeaponInfo[equipingWeaponType];
        m_PlayerUIManager.ChangeWeapon(equipingWeaponType, (int)attackType, GetBitPosition((int)fireMode), m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet, weaponImage);
        m_PlayerUIManager.DisplayReloadImage(IsActiveReloadImage());
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

    public void HitEnemy()
    {
        m_PlayerUIManager.HitEnemy();
    }

    /// <summary>
    /// ���Ÿ� ���� ��� ��
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
    /// ���⸦ ����� ��
    /// </summary>
    /// <param name="slotNumber">���� Ÿ�� (���� ��ȣ�� ��ġ)</param>
    public void GetWeapon(int slotNumber, Sprite sprite, int weaponIndex, int maxBullet)    //���� �ȴ�
    {
        m_Inventory.WeaponInfo[slotNumber].m_HavingWeaponIndex = weaponIndex;
        m_PlayerUIManager.UpdateWeaponSlot(slotNumber, sprite);
    }

    /// <summary>
    /// ��Ŷ�� ����ϰų� ����� ��
    /// </summary>
    /// <param name="value">����ϰų� ���� ���� (���� ����)</param>
    public void UsingHealKit(int value)
    {
        int remainHealKit = m_Inventory.HealKitHavingCount += value;
        m_PlayerUIManager.UsingHealKit(remainHealKit, m_AmountPlayerHP);
        UpdatePlayerHP(-HealKitAmount);
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



    public void PlayerHit(Transform target, int damage, AttackType attackType)
    {
        if(attackType == AttackType.Grab) GrabAction?.Invoke(true);
        else if (attackType == AttackType.Explosion) damage = (int)(damage * 0.5f);

        UpdatePlayerHP(damage);
        m_PlayerUIManager.DisplayHitDirection(target);
    }

    //UnityEvent ȣ��
    public void PlayerHit(int damage, AttackType attackType)
    {
        if (attackType == AttackType.Explosion) damage = (int)(damage * 0.5f);
        UpdatePlayerHP(damage);
    }

    public void EndGrab() => GrabAction?.Invoke(false);
}
