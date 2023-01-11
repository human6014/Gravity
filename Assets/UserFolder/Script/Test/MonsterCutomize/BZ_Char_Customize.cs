using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class BZ_Char_Customize : MonoBehaviour
    {
        private Renderer skinRend;
        private BZ_AssetsList materialsList;

        public enum BodyType
        {
            V1,
            V2,
            V3,
            V4
        }

        public enum ShirtType
        {
            None,
            V1,
            V2,
            V3,
            V4
        }

        public enum ShortsType
        {
            V1,
            V2,
            V3,
            V4
        }

        public BodyType bodyType;
        public ShirtType shirtType;
        public ShortsType shortsType;

        public void charCustomize(int body, int top, int bottom)
        {
            materialsList = GetComponent<BZ_AssetsList>();
            Material[] mat;

            // Set BodyType, BottomType
            Transform curSub = transform.Find("Geo/BigZombie");
            foreach (Transform child in curSub)
            {
                skinRend = child.GetComponent<Renderer>();
                
                mat = new Material[2];
                mat[0] = materialsList.ClothesMaterials[bottom];
                mat[1] = materialsList.SkinMaterials[body];
                skinRend.materials = mat;
            }

            // Set ShirtType
            // Clothes2번씀
            curSub = transform.Find("Geo/BigZombieShirt");
            foreach (Transform child in curSub)
            {
                if (top < 1) curSub.gameObject.SetActive(false);
                else
                {
                    curSub.gameObject.SetActive(true);
                    skinRend = child.GetComponent<Renderer>();
                    skinRend.material = materialsList.ClothesMaterials[top];
                }
            }
        }

        void OnValidate()
        {
            charCustomize((int)bodyType, (int)shirtType, (int)shortsType);
        }
    }
}
