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

    public void UpdatemAmmoText(int current, int magazine)
    {
        if (!m_HasAmmo) return;
        m_AmmoText.text = string.Format("{0} / {1}", current, magazine);
    }

    public void UnlockSlot()
    {
        LockObject.SetActive(false);
        UnlockObject.SetActive(true);
    }
}
