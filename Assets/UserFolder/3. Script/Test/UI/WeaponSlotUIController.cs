using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotUIController : MonoBehaviour
{
    [SerializeField] private GameObject [] m_WeaponSlots;
    [SerializeField] private GameObject [] m_ClickedHideObject;

    private bool m_IsSelectWeapon;
    private int m_CurrentWeaponSlotIndex = 0;

    public void ChangeWeaponSlot()
    {
        m_WeaponSlots[m_CurrentWeaponSlotIndex].SetActive(false);
        m_CurrentWeaponSlotIndex = (m_CurrentWeaponSlotIndex + 1) % m_WeaponSlots.Length;
        m_WeaponSlots[m_CurrentWeaponSlotIndex].SetActive(true);
    }

    public void ClickWeaponSlot()
    {
        m_IsSelectWeapon = true;

        foreach (GameObject go in m_ClickedHideObject)
            go.SetActive(false);
        
        //m_CurrentWeaponSlotIndex µî·Ï
    }
}
