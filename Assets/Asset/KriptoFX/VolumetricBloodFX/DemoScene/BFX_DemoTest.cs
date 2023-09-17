
using UnityEngine;
using UnityEngine.Rendering;

public class BFX_DemoTest : MonoBehaviour
{
    public bool InfiniteDecal;
    public Light DirLight;
    public GameObject BloodAttach;
    public GameObject[] BloodFX;
    public Vector3 direction;

    private int m_EffectIndex;
    private int m_ActiveBloods;
    private Camera m_MainCamera;

    private void Awake()
    {
        m_MainCamera = GetComponent<Camera>();
    }

    private Transform GetNearestObject(Transform hit, Vector3 hitPos)
    {
        float closestPos = float.MaxValue;
        Transform closestBone = null;
        var childs = hit.GetComponentsInChildren<Transform>();

        foreach (var child in childs)
        {
            var dist = Vector3.SqrMagnitude(child.position - hitPos);
            if (dist < closestPos)
            {
                closestPos = dist;
                closestBone = child;
            }
        }

        return closestBone;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = m_MainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))//Layer추가
            {
                // var randRotation = new Vector3(0, Random.value * 360f, 0);
                // var dir = CalculateAngle(Vector3.forward, hit.normal);
                //float angle = Mathf.Atan2(hit.normal.x, hit.normal.z) * Mathf.Rad2Deg - 90;

                Quaternion rotation = Quaternion.LookRotation(hit.normal, -Manager.GravityManager.GravityVector);
                rotation *= Quaternion.Euler(0,-90,0);
                int effectIdx = Random.Range(0, BloodFX.Length);

                Instantiate(BloodFX[effectIdx], hit.point, rotation);
                m_ActiveBloods++;

                var nearestBone = GetNearestObject(hit.transform.root, hit.point);
                if (nearestBone != null)
                {
                    var attachBloodInstance = Instantiate(BloodAttach);
                    var bloodT = attachBloodInstance.transform;
                    bloodT.position = hit.point;
                    bloodT.localRotation = Quaternion.identity;
                    bloodT.localScale = Vector3.one * Random.Range(0.75f, 1.2f);
                    bloodT.LookAt(hit.point + hit.normal);
                    bloodT.Rotate(90, 0, 0);
                    bloodT.transform.parent = nearestBone;
                    //Destroy(attachBloodInstance, 20);
                }

                // if (!InfiniteDecal) Destroy(instance, 20);
            }
        }
    }

    public float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }
}
