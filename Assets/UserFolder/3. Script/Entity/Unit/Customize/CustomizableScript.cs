using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entity.Unit
{
    public abstract class CustomizableScript : MonoBehaviour
    {
        public abstract void Customizing(ref CustomizingAssetList.MaterialsStruct[] materialsStructs);
        protected abstract void RandNum();
    }
}
