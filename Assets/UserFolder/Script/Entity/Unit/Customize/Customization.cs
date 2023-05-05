using System;
using System.Collections.Generic;
using UnityEngine;
using EnumType;

[RequireComponent(typeof(CustomizingAssetList))]
public class Customization : MonoBehaviour
{
    CustomizingAssetList customizingAssetList;
    public void Awake() => customizingAssetList = GetComponent<CustomizingAssetList>();


    public void Customize(Entity.Unit.Normal.NormalMonster unit)
    {
        NoramlMonsterType monsterType = unit.GetMonsterType();

        unit.GetComponent<CustomizableScript>().Customizing(ref customizingAssetList.GetUnitMaterial(monsterType));
    }
}
