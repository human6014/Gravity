using System;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit;
using Scriptable.Monster;

[RequireComponent(typeof(CustomizingAssetList))]
public class Customization : MonoBehaviour
{
    private CustomizingAssetList customizingAssetList;
    public void Awake() => customizingAssetList = GetComponent<CustomizingAssetList>();


    public void Customize(Entity.Unit.Normal.NormalMonster unit)
    {
        NoramlMonsterType monsterType = unit.GetMonsterType;

        unit.GetComponent<CustomizableScript>().Customizing(ref customizingAssetList.GetUnitMaterial(monsterType));
    }
}
