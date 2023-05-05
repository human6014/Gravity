using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit
{
    public class BigZomibeCustomize : CustomizableScript
    {
        private readonly int bodyTypeLength = 4;
        private readonly int shirtTypeLength = 5;
        private readonly int trouserTypeLength = 4;

        private int bodyType;
        private int shirtType;
        private int trouserType;

        Transform bodyT;
        Transform shirtT;
        private void Awake()
        {
            bodyT = transform.Find("Geo/BigZombie");
            shirtT = transform.Find("Geo/BigZombieShirt");
        }

        protected override void RandNum()
        {
            bodyType = Random.Range(0, bodyTypeLength);
            shirtType = Random.Range(0, shirtTypeLength);
            trouserType = Random.Range(0, trouserTypeLength);
        }

        public override void Customizing(ref CustomizingAssetList.MaterialsStruct[] materialsStructs)
        {
            RandNum();
            Material[] mat = new Material[2];
            Renderer skinRend;
            foreach (Transform child in bodyT)
            {
                skinRend = child.GetComponent<Renderer>();

                mat[0] = materialsStructs[1].partMaterials[trouserType];
                mat[1] = materialsStructs[0].partMaterials[bodyType];
                skinRend.materials = mat;
            }

            if (shirtType < 1) shirtT.gameObject.SetActive(false);
            else
            {
                shirtT.gameObject.SetActive(true);
                foreach (Transform child in shirtT)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialsStructs[0].partMaterials[shirtType - 1];
                }
            }
        }
    }
}

