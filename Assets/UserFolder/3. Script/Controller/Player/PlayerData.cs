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
    [Tooltip("�÷��̾ ������ ��ų ����Ʈ")]
    [SerializeField] private int m_SkillPoint = 100;

    [Tooltip("��ų ����Ʈ�� ��� ���� ų ��")]
    [SerializeField] private int m_GetSkillKillCount = 5;

    [Header("")]
    [Tooltip("�� Ŷ ȸ����")]
    [SerializeField] private int m_HealKitAmount = 400;

    [Tooltip("����")]
    [SerializeField] private int m_Defense = 0;


    [Header("HP")]
    [Tooltip("�ִ� HP")]
    [SerializeField] private int m_MaxPlayerHP = 1000;

    [Tooltip("Ư�� �ð��� ȸ�� �� HP���")]
    [SerializeField] private int m_AutoHPHealAmount = 2;

    [Tooltip("HP ȸ�� �� �ð�")]
    [SerializeField] private float m_AutoHPHealTime = 0.1f;


    [Header("MP")]
    [Tooltip("�ִ� MP")]
    [SerializeField] private int m_MaxPlayerMP = 1000;

    [Tooltip("Ư�� �ð��� ȸ�� �� MP���")]
    [SerializeField] private int m_AutoMPHealAmount = 10;

    [Tooltip("MP ȸ��, �Ҹ� �� �ð�")]
    [SerializeField] private float m_AutoMPTime = 0.1f;

    [Tooltip("���� MP �Ҹ�")]
    [SerializeField] private int m_JumpingMPConsumeAmount = 170;

    [Tooltip("�޸��� MP �Ҹ�")]
    [SerializeField] private int m_RunningMPConsumeAmount = 17;

    [Tooltip("�޸��⸦ ������ �ּ� MP")]
    [SerializeField] private int m_StartRunningMP = 100;


    [Header("GE")]
    [Tooltip("�߷� ���� ������ �ִ뷮")]
    [SerializeField] private int m_MaxPlayerGE = 1000;

    [Tooltip("�߷� ���� ������ ������ ȸ����")]
    [SerializeField] private int m_AutoGEHealAmount = 3;

    [Tooltip("�߷� ���� ȸ�� �� �ð� ����")]
    [SerializeField] private float m_AutoGETime = 0.1f;

    [Tooltip("�߷� ���� ������ �Ҹ�")]
    [SerializeField] private int m_GEConsumeAmount = 600;


    [Header("TE")]
    [Tooltip("�ð� ���� ������ �ִ뷮")]
    [SerializeField] private int m_MaxPlayerTE = 1000;

    [Tooltip("�ð� ���� ������ ������ ȸ����")]
    [SerializeField] private int m_TEHealAmount = 6;

    [Tooltip("�ð� ���� ������ �Ҹ�")]
    [SerializeField] private int m_TEConsumeAmount = 20;

    [Tooltip("�ð� ���� ȸ��, �Ҹ� �� �ð� ����")]
    [SerializeField] private float m_AutoTETime = 0.1f;

    [Tooltip("�ð� ���� ������ �ּ� TE")]
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
    /// �÷��̾ ���� �� ������ �¾��� �� �ߵ�
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
    /// UnityEvent�� ȣ���
    /// </summary>
    /// <param name="damage">������</param>
    /// <param name="attackType">���� ����</param>
    public void PlayerHit(int damage, AttackType attackType)
    {
        //���� �ڱⰡ ��� �ڱⰡ �´°� ����
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