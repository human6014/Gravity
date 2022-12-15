using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalNormalMonsterSupport : MonoBehaviour
{
    new Rigidbody rigidbody;
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">true : none모드 활성화, false : none모드 비활성화</param>
    public void OnNoneMode(bool value)
    {
        rigidbody.isKinematic = !value;
        rigidbody.useGravity = value;
    }
    /// <summary>
    /// 해당 transform을 pos위치와 동기화 합니다
    /// </summary>
    /// <param name="pos">동기화 할 위치</param>
    public void SyncNav(Vector3 pos)
    {
        transform.position = pos;
    }

    /// <summary>
    /// 해당 오브젝트의 위치를 반환합니다.
    /// </summary>
    /// <returns>오브젝트의 위치(Vector3)</returns>
    public Vector3 GetPos()
    {
        return transform.position;
    }
}
