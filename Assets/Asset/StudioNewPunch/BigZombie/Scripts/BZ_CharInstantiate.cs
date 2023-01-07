using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BZ_CharInstantiate : MonoBehaviour
{
	private int bodyTyp;
	private int shirtTyp;
	private int shortsTyp;


	private BZ_AssetsList materialsList;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	public enum BodyType
	{
		V1,
		V2,
		V3,
		V4
	}

	public enum ShirtType
	{
		V1,
		V2,
		V3,
		V4,
		No


	}

	public enum ShortsType
	{
		V1,
		V2,
		V3,
		V4
	}
	public Transform prefabObject;
	public BodyType bodyType;
	public ShirtType shirtType;
	public ShortsType shortsType;
	void Start()
    {
		Transform pref = Instantiate(prefabObject, gameObject.transform.position, gameObject.transform.rotation);
		bodyTyp = (int)bodyType;
		shirtTyp = (int)shirtType;
		shortsTyp = (int)shortsType;
		if (pref.gameObject.GetComponent<BZ_Char_Customize>() != null)
		{
			pref.gameObject.GetComponent<BZ_Char_Customize>().charCustomize(bodyTyp, shirtTyp, shortsTyp);
        }
        else
        {
			pref.gameObject.GetComponent<BZ_CharCustomizeBP>().charCustomize(bodyTyp, shirtTyp, shortsTyp);
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
