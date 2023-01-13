using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit
{
    public class OldManCustomize : CustomizableScript
    {
        private Transform bodyT;

        private readonly int bodyTypeLength = 2;
        private readonly int trouserTypeLength = 2;

        private int bodyType;
        private int trouserType;

        private void Awake()
        {
            bodyT = transform.Find("Geo/OldManZombie_One");
        }
        protected override void RandNum()
        {
            bodyType = Random.Range(0, bodyTypeLength);
            trouserType = Random.Range(0, trouserTypeLength);
        }

        public override void Customizing(CustomizingAssetList.MaterialsStruct[] materialsStructs)
        {
            RandNum();
            Renderer skinnedRenderer;
            Material[] mat = new Material[2];
            foreach (Transform child in bodyT)
            {
                skinnedRenderer = child.GetComponent<Renderer>();

                mat[0] = materialsStructs[0].partMaterials[bodyType];
                mat[1] = materialsStructs[1].partMaterials[trouserType];

                skinnedRenderer.materials = mat;
            }
        }
    }
}

