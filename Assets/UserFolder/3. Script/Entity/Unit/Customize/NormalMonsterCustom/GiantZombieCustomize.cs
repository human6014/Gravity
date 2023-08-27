using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit
{
    public class GiantZombieCustomize : CustomizableScript
    {
        [Header("Original Model")]
        [SerializeField] private Transform tshirtT;
        [SerializeField] private Transform tanktopT;
        [SerializeField] private Transform torsoT;
        [SerializeField] private Transform armT;
        [SerializeField] private Transform trousersT;
        [SerializeField] private Transform footL_T;
        [SerializeField] private Transform headsT;

        [Header("RagDoll Model")]
        [SerializeField] private Transform tshirtT_RagDoll;
        [SerializeField] private Transform tanktopT_RagDoll;
        [SerializeField] private Transform torsoT_RagDoll;
        [SerializeField] private Transform armT_RagDoll;
        [SerializeField] private Transform trousersT_RagDoll;
        [SerializeField] private Transform footL_T_RagDoll;
        [SerializeField] private Transform headsT_RagDoll;

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

        protected override void RandNum()
        {
            faceType = Random.Range(0, faceTypeLength);
            bodyType = Random.Range(0, bodyTypeLength);
            trouserType = Random.Range(0, trouserTypeLength);
            tanktopType = Random.Range(0, tanktopTypeLength);
            tshirtType = Random.Range(0, tshirtTypeLength);
        }

        public override void Customizing(ref CustomizingAssetList.MaterialsStruct[] materialStructs)
        {
            RandNum();

            ChangeParts(ref materialStructs, headsT, armT, torsoT, footL_T, trousersT, tshirtT);
            ChangeParts(ref materialStructs, headsT_RagDoll, armT_RagDoll, torsoT_RagDoll, footL_T_RagDoll, trousersT_RagDoll, tshirtT_RagDoll);

        }

        private void ChangeParts(ref CustomizingAssetList.MaterialsStruct[] materialStructs, Transform heads, 
            Transform arm, Transform torso, Transform footL, Transform trousers, Transform tshirt)
        {
            Renderer skinRend;
            Transform activeHead = null;
            int i = 0;
            foreach (Transform child in heads)
            {
                if (i == faceType)
                {
                    child.gameObject.SetActive(true);
                    activeHead = child;
                }
                else child.gameObject.SetActive(false);
                i++;
            }


            foreach (Transform child in activeHead)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialStructs[0].partMaterials[bodyType];
            }

            // Body_Exposed arms
            foreach (Transform child in arm)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialStructs[0].partMaterials[bodyType];
            }


            //Torso to hide
            foreach (Transform child in torso)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialStructs[0].partMaterials[bodyType];
            }

            foreach (Transform child in footL)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialStructs[0].partMaterials[bodyType];
            }

            //Trousers
            foreach (Transform child in trousers)
            {
                skinRend = child.GetComponent<Renderer>();
                skinRend.material = materialStructs[1].partMaterials[trouserType];
            }


            // Tshirt
            if (tshirtType < 1)
            {
                tshirt.gameObject.SetActive(false);
                torso.gameObject.SetActive(true);
            }
            else
            {
                tshirt.gameObject.SetActive(true);
                torso.gameObject.SetActive(false);

                foreach (Transform child in tshirt)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialStructs[2].partMaterials[tshirtType - 1];
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
                torso.gameObject.SetActive(true);

                foreach (Transform child in tanktopT)
                {
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialStructs[3].partMaterials[tanktopType - 1];
                }
            }
        }
    }
}

