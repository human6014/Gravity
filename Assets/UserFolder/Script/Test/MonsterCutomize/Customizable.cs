using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CustomizableScript : MonoBehaviour
{
    public abstract void Customizing(CustomizingAssetList.MaterialsStruct[] materialsStructs);
    protected abstract void RandNum();
}
