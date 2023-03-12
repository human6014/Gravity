using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;

[ExecuteInEditMode]
public class RotateTest : MonoBehaviour
{
    const float ROTATETIME = 1;
    private readonly Vector3Int[] gravityRotation =
    {
            new Vector3Int(0, 0, -90)  , new Vector3Int(0, 0, 90),
            new Vector3Int(0, 0, 0)    , new Vector3Int(0, 0, 180),
            new Vector3Int(0, -90, -90), new Vector3Int(0, 90, 90),
        };

    private Vector3Int currentRot;
    [ContextMenu("Reset")]
    public void Reset()
    {
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    [ContextMenu("XDown")]
    public void XDown()
    {
        currentRot = gravityRotation[0];
        StartCoroutine(Rotate(new Vector3(0, 0, -90)));
    }

    [ContextMenu("XUp")]
    public void XUp()
    {
        currentRot = gravityRotation[1];
        StartCoroutine(Rotate(new Vector3(0, 0, 90)));
    }

    [ContextMenu("YDown")]
    public void YDown()
    {
        currentRot = gravityRotation[2];
        StartCoroutine(Rotate(new Vector3(0, -90, 0)));
    }

    [ContextMenu("YUp")]
    public void YUp()
    {
        currentRot = gravityRotation[3];
        StartCoroutine(Rotate(new Vector3(0, 90, 0)));
    }

    [ContextMenu("ZDown")]
    public void ZDown()
    {
        currentRot = gravityRotation[4];
        StartCoroutine(Rotate(new Vector3(-90, 0, 0)));
    }

    [ContextMenu("ZUp")]
    public void ZUp()
    {
        currentRot = gravityRotation[5];
        StartCoroutine(Rotate(new Vector3(90, 0, 0)));
    }

    public IEnumerator Rotate(Vector3 rotation)
    {
        //transform.Rotate(rotation);
        transform.rotation = Quaternion.Euler(currentRot);
        yield return null;
    }
}
