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

		public void charCustomize(int body, int hair)
		{
			materialsList = gameObject.GetComponent<WomenAssetList>();
			Material[] mat = new Material[2];
			Renderer skinnedMeshRenderer = transform.Find("Geo/ZombieGirl_B").GetComponent<Renderer>();

			mat[0] = materialsList.BodySkinMaterials[body];
			mat[1] = materialsList.HairMaterials[hair];

			skinnedMeshRenderer.materials = mat;
		}
		void OnValidate()
		{
			charCustomize((int)bodySkin, (int)hair);
		}
	}
}
