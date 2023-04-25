using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WeaponInfo
{
    public int m_HavingWeaponIndex;
    public int m_CurrentRemainBullet;
    public int m_MagazineRemainBullet;
    public int m_MaxBullet;
    public WeaponInfo(int index, int remainBullet, int magazineBullet, int maxBullet)
    {
        m_HavingWeaponIndex = index;
        m_CurrentRemainBullet = remainBullet;
        m_MagazineRemainBullet = magazineBullet;
        m_MaxBullet = maxBullet;
    }
}
[System.Serializable]
public class Inventory
{
    [SerializeField] private WeaponInfo[] m_WeaponInfo;
    [SerializeField] private int m_HealKitHavingCount = 0; 
    private Test.FireMode[] currentFireMode;

    public WeaponInfo[] WeaponInfo { get => m_WeaponInfo; }

    public int HealKitHavingCount { get => m_HealKitHavingCount; set => m_HealKitHavingCount = value; }

    public int AddThrowingWeapon(int value) => WeaponInfo[4].m_MagazineRemainBullet += value;

    public void SetCurrentFireMode(int value, Test.FireMode fireMode) => currentFireMode[value] = fireMode;


    public Test.FireMode CurrentFireMode(int value) => currentFireMode[value];

    public Inventory(int m_EquipingTypeLength)
    {
        //m_HavingWeaponIndex = new int[m_EquipingTypeLength];
        //currentFireMode = new Test.FireMode[m_EquipingTypeLength];
        //currentRemainBullet = new int[m_EquipingTypeLength];
        //for (int i = 0; i < m_HavingWeaponIndex.Length; i++)
        //{
        //    m_HavingWeaponIndex[i] = -1;
        //}
    }
}
