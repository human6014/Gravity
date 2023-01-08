using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldManCharCustomize : MonoBehaviour
{
	public enum BodySkin
	{
		V1 = 0,
		V2 = 1,
	}

	public enum TrousersSkin
	{
		V1 = 0,
		V2 = 1,
	}


	public BodySkin bodySkin;
	public TrousersSkin trousersSkin;
	OldManAssetList materialsList;

	public void charCustomize(int body, int trousers)
	{
		materialsList = gameObject.GetComponent<OldManAssetList>();
		Material[] mat = new Material[2];
		Renderer skinnedMeshRenderer = transform.Find("ONE_Mesh/OldManZombie_One").GetComponent<Renderer>();
		
		mat[0] = materialsList.BodySkinMaterials[body];
		mat[1] = materialsList.TrousersMaterials[trousers];

		skinnedMeshRenderer.materials = mat;
	}
	void OnValidate()
	{
		charCustomize((int)bodySkin, (int)trousersSkin);
	}
}
