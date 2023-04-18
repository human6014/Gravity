using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [SerializeField] private int[] m_HavingWeaponIndex;
    [SerializeField] private int m_ThrowingWeaponHavingCount = 0;
    [SerializeField] private int m_HealKitHavingCount = 0;

    private Test.FireMode [] currentFireMode;
    private int[] currentRemainBullet;

    public int GetHavingWeaponIndex(int value) => m_HavingWeaponIndex[value];
    public void SetHavingWeaponIndex(int type, int value) => m_HavingWeaponIndex[type] = value;


    public int ThrowingWeaponHavingCount(int value) => m_ThrowingWeaponHavingCount += value;
    public int HealKitHavingCount(int value) => m_HealKitHavingCount += value;


    public int AddThrowingWeapon(int value) => m_ThrowingWeaponHavingCount += value;
    public int AddHealKit(int value) => m_HealKitHavingCount += value;


    public void SetCurrentRemainBullet(int value, int count) => currentRemainBullet[value] = count;
    public void SetCurrentFireMode(int value, Test.FireMode fireMode) => currentFireMode[value] = fireMode;

    
    public int GetCurrentRemainBullet(int value) => currentRemainBullet[value];

    public Test.FireMode CurrentFireMode(int value) => currentFireMode[value];

    public Inventory(int m_EquipingTypeLength)
    {
        m_HavingWeaponIndex = new int[m_EquipingTypeLength];
        currentFireMode = new Test.FireMode[m_EquipingTypeLength];
        currentRemainBullet = new int[m_EquipingTypeLength];
        for (int i = 0; i < m_HavingWeaponIndex.Length; i++)
        {
            m_HavingWeaponIndex[i] = -1;
        }
    }
}
