using System;
using System.Collections.Generic;
using UnityEngine;
using EnumType;

[RequireComponent(typeof(CustomizingAssetList))]
public class Customization : MonoBehaviour
{
    CustomizingAssetList customizingAssetList;
    public void Awake()
    {
        customizingAssetList = GetComponent<CustomizingAssetList>();
    }

    public void Customize(NormalMonster unit)
    {
        NoramlMonsterType monsterType = unit.GetMonsterType();
        CustomizableScript customizableScript = unit.GetComponent<CustomizableScript>();

        customizableScript.Customizing(customizingAssetList.GetUnitMaterial(monsterType));
    }
}
