using System;
using System.Collections.Generic;
using UnityEngine;
using EnumType;

public class Customization : MonoBehaviour
{
    //enum ±Ê¿Ã
    //int count = Enum.GetValues(typeof(NormalZomibeMaterials)).Length;

    CustomizingAssetList customizingAssetList;
    public void Awake()
    {
        customizingAssetList = GetComponent<CustomizingAssetList>();
    }

    public void Customize(NormalMonster unit)
    {
        Debug.Log(unit.name);
        Debug.Log(unit.GetMonsterType());
        NoramlMonsterType monsterType = unit.GetMonsterType();
        CustomizableScript customizableScript = unit.GetComponent<CustomizableScript>();

        customizableScript.Customizing(customizingAssetList.GetUnitMaterial(monsterType));
    }
}
