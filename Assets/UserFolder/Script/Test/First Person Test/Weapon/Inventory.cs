using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [SerializeField] private int[] m_HavingWeaponIndex;
    [SerializeField] private int m_ThrowingWeaponHavingCount = 0;
    [SerializeField] private int m_HealKitHavingCount = 0;

    public int GetHavingWeaponIndex(int value) => m_HavingWeaponIndex[value];
    public void SetHavingWeaponIndex(int type, int value) => m_HavingWeaponIndex[type] = value;

    public int ThrowingWeaponHavingCount() => m_ThrowingWeaponHavingCount;
    public int HealKitHavingCount() => m_HealKitHavingCount;

    public void AddThrowingWeapon(int value) => m_ThrowingWeaponHavingCount += value;
    public void AddHealKit(int value) => m_HealKitHavingCount += value;

    public Inventory(int m_EquipingTypeLength)
    {
        m_HavingWeaponIndex = new int[m_EquipingTypeLength];
        for (int i = 0; i < m_HavingWeaponIndex.Length; i++)
        {
            m_HavingWeaponIndex[i] = -1;
        }
    }
}
