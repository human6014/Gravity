using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit
{
    public class GiantZombieCustomize : CustomizableScript
    {
        private Transform tshirtT;
        private Transform tanktopT;
        private Transform bodyToHideT;
        private Transform bodyExposedT;
        private Transform trousersT;
        private Transform footL_T;
        private Transform[] faceT;

        private readonly int faceTypeLength = 4;
        private readonly int bodyTypeLength = 4;
        private readonly int trouserTypeLength = 4;
        private readonly int tanktopTypeLength = 5;
        private readonly int tshirtTypeLength = 5;

        private int faceType;
        private int bodyType;
        private int trouserType;
        private int tanktopType;
        private int tshirtType;
        private void Awake()
        {
            tshirtT = transform.Find("Geo/Tshirt");
            tanktopT = transform.Find("Geo/TankTop");
            trousersT = transform.Find("Geo/Trousers");
            bodyToHideT = transform.Find("Geo/Torso");
            bodyExposedT = transform.Find("Geo/Arms");
            footL_T = transform.Find("Geo/Foot_L");

            faceT = transform.Find("Geo/Heads").GetComponentsInChildren<Transform>(true);
            //이거 아님
        }

        protected override void RandNum()
        {
            faceType = Random.Range(0, faceTypeLength);
            bodyType = Random.Range(0, bodyTypeLength);
            trouserType = Random.Range(0, trouserTypeLength);
            tanktopType = Random.Range(0, tanktopTypeLength);
            tshirtType = Random.Range(0, tshirtTypeLength);
        }

        public override void Customizing(CustomizingAssetList.MaterialsStruct[] materialsStructs)
        {
            RandNum();
            Renderer skinRend;

            for (int i = 0; i < faceT.Length; i++)
                faceT[i].gameObject.SetActive(false);

            faceT[faceType].gameObject.SetActive(true);

            
            foreach (Transform child in faceT[faceType])
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsStructs[0].partMaterials[bodyType];
            }
            
            // Body_Exposed arms
            foreach (Transform child in bodyExposedT)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsStructs[0].partMaterials[bodyType];
            }


            //Torso to hide
            foreach (Transform child in bodyToHideT)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsStructs[0].partMaterials[bodyType];
            }

            foreach (Transform child in footL_T)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsStructs[0].partMaterials[bodyType];
            }

            //Trousers
            foreach (Transform child in trousersT)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialsStructs[1].partMaterials[trouserType];
            }


            // Tshirt
            if (tshirtType < 1)
            {
                tshirtT.gameObject.SetActive(false);
                bodyToHideT.gameObject.SetActive(true);
            }
            else
            {
                tshirtT.gameObject.SetActive(true);
                bodyToHideT.gameObject.SetActive(false);

                foreach (Transform child in tshirtT)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialsStructs[2].partMaterials[tshirtType - 1];
                }
            }

            // TankTop
            // Tshirt가 우선권이 있음
            if (tanktopType < 1 || tshirtType > 0)
            {
                tanktopT.gameObject.SetActive(false);
            }
            else
            {
                tanktopT.gameObject.SetActive(true);
                bodyToHideT.gameObject.SetActive(true);

                foreach (Transform child in tanktopT)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialsStructs[3].partMaterials[tanktopType - 1];
                }
            }
        }
    }
}

