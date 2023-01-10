using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
	public class WomenCharCustomize : MonoBehaviour
	{
		public enum BodySkin
		{
			V1 = 0,
			V2 = 1,
		}

		public enum Hair
		{
			V1 = 0,
			V2 = 1,
		}

		public BodySkin bodySkin;
		public Hair hair;
		WomenAssetList materialsList;
		Renderer skinnedRenderer;

		public void charCustomize(int body, int hair)
		{
			materialsList = GetComponent<WomenAssetList>();
			Material[] mat = new Material[2];
			Transform curSub = transform.Find("Geo/WomenZombie");

			foreach(Transform child in curSub)
            {
				skinnedRenderer = child.GetComponent<Renderer>();

				mat[0] = materialsList.BodySkinMaterials[body];
				mat[1] = materialsList.HairMaterials[hair];

				skinnedRenderer.materials = mat;
			}
		}
		void OnValidate()
		{
			charCustomize((int)bodySkin, (int)hair);
		}
	}
}
