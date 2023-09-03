using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotStateDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject UnlockObject;
    [SerializeField] private GameObject LockObject;
    [SerializeField] private Text m_AmmoText;
    [SerializeField] private bool m_HasAmmo;
    [SerializeField] private int m_SlotNumber;

    private PlayerData m_PlayerData;
    private bool m_IsUnLock;
    private bool m_IsSelect;

    protected PlayerSkillReceiver PlayerSkillReceiver { get; private set; }

    private void Awake()
    {
        m_PlayerData = FindObjectOfType<PlayerData>();
        PlayerSkillReceiver = FindObjectOfType<PlayerSkillReceiver>();
    }

    public void UpdatemAmmoText(int current, int magazine)
    {
        if (!m_HasAmmo) return;
        m_AmmoText.text = string.Format("{0} / {1}", current, magazine);
    }

    public void UnlockSlot()
    {
        m_IsUnLock = true;

        LockObject.SetActive(false);
        UnlockObject.SetActive(true);
    }

    public void RegisterPlayerSlot(int slotIndex)
    {
        m_IsSelect = true;
        PlayerSkillReceiver.GetWeaponEvent(m_SlotNumber, slotIndex);
    }
}
