using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotStateDisplayer : MonoBehaviour
{
    [SerializeField] private Text m_AmmoText;
    [SerializeField] private Text m_PointCostText;
    [SerializeField] private Text m_AmmoPointCostText;

    [SerializeField] private bool m_HasAmmo;
    [SerializeField] private int m_SlotNumber;
    [SerializeField] private int m_SkillCost;

    [SerializeField] private int m_AmmoCount;
    [SerializeField] private int m_AmmoCost;

    private PlayerData m_PlayerData;
    private WeaponInfo m_CurrentWeaponInfo;

    protected PlayerSkillReceiver PlayerSkillReceiver { get; private set; }

    private void Awake()
    {
        m_PlayerData = FindObjectOfType<PlayerData>();
        PlayerSkillReceiver = FindObjectOfType<PlayerSkillReceiver>();
        m_CurrentWeaponInfo = m_PlayerData.Inventory.WeaponInfo[m_SlotNumber];
        
        UpdateSkillPointText();
    }

    public void UpdateSkillPointText()
    {
        m_PointCostText.text = string.Format("Point : {0}", m_SkillCost);
        if(m_HasAmmo) m_AmmoPointCostText.text = string.Format("Point : {0}", m_AmmoCost);
    }

    private void UpdateAmmoText(int current, int magazine)
    {
        if (!m_HasAmmo) return;
        m_AmmoText.text = string.Format("{0} / {1}", current, magazine);
    }

    public bool CanRegisterWeapon() => m_PlayerData.CanSkillUpPoint(m_SkillCost);
    
    public void RegisterPlayerSlot(int slotIndex)
    {
        PlayerSkillReceiver.GetWeaponEvent(m_SlotNumber, slotIndex);
        UpdateAmmoText(m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet);
    }

    public void OnClickGetAmmo()
    {
        if (!m_PlayerData.CanSkillUpPoint(m_AmmoCost)) return;
        PlayerSkillReceiver.GetSupplyEvent(m_SlotNumber, m_AmmoCount);
        UpdateAmmoText(m_CurrentWeaponInfo.m_CurrentRemainBullet, m_CurrentWeaponInfo.m_MagazineRemainBullet);
    }
}
