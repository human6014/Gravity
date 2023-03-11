using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RotateTest : MonoBehaviour
{
    const float ROTATETIME = 1;

    [ContextMenu("Reset")]
    public void Reset()
    {
        transform.rotation = Quaternion.Euler(0,0,0);
    }

    [ContextMenu("XDown")]
    public void XDown()
    {
        StartCoroutine(Rotate(new Vector3(0, 0, -90)));
    }

    [ContextMenu("XUp")]
    public void XUp()
    {
        StartCoroutine(Rotate(new Vector3(0, 0, 90)));
    }

    [ContextMenu("YDown")]
    public void YDown()
    {
        StartCoroutine(Rotate(new Vector3(0, -90, 0)));
    }

    [ContextMenu("YUp")]
    public void YUp()
    {
        StartCoroutine(Rotate(new Vector3(0, 90, 0)));
    }

    [ContextMenu("ZDown")]
    public void ZDown()
    {
        StartCoroutine(Rotate(new Vector3(-90, 0, 0)));
    }

    [ContextMenu("ZUp")]
    public void ZUp()
    {
        StartCoroutine(Rotate(new Vector3(90, 0, 0)));
    }

    public IEnumerator Rotate(Vector3 rotation)
    {
        transform.Rotate(rotation);
        yield return null;
    }
}
