using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit
{
    public class WomenZombieCustomize : CustomizableScript
    {
        private Transform bodyT;

        private readonly int bodyTypeLength = 2;
        private readonly int hairTypeLength = 2;

        private int bodyType;
        private int hairType;

        private void Awake()
        {
            bodyT = transform.Find("Geo/WomenZombie");
        }

        protected override void RandNum()
        {
            bodyType = Random.Range(0, bodyTypeLength);
            hairType = Random.Range(0, hairTypeLength);
        }

        public override void Customizing(ref CustomizingAssetList.MaterialsStruct[] materialsStructs)
        {
            RandNum();
            Material[] mat = new Material[2];
            Renderer skinnedRenderer;

            foreach (Transform child in bodyT)
            {
                skinnedRenderer = child.GetComponent<Renderer>();

                mat[0] = materialsStructs[0].partMaterials[bodyType];
                mat[1] = materialsStructs[1].partMaterials[hairType];

                skinnedRenderer.materials = mat;
            }
        }
    }
}

