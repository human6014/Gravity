using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    #region Having weapon index
    //이상  이하

    public int[] HavingWeaponIndex { get; set; }
    #endregion

    #region Having item count
    public int ThrowingWeaponHavingCount { get; set; } = 0;         
    public int HealKitHavingCount { get; set; } = 0;
    #endregion

    public Inventory(int weaponCount)
    {
        HavingWeaponIndex = new int[weaponCount];
    }
}
