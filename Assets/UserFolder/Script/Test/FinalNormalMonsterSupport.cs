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
    /// <param name="value">true : none��� Ȱ��ȭ, false : none��� ��Ȱ��ȭ</param>
    public void OnNoneMode(bool value)
    {
        rigidbody.isKinematic = !value;
        rigidbody.useGravity = value;
    }
    /// <summary>
    /// �ش� transform�� pos��ġ�� ����ȭ �մϴ�
    /// </summary>
    /// <param name="pos">����ȭ �� ��ġ</param>
    public void SyncNav(Vector3 pos)
    {
        transform.position = pos;
    }

    /// <summary>
    /// �ش� ������Ʈ�� ��ġ�� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>������Ʈ�� ��ġ(Vector3)</returns>
    public Vector3 GetPos()
    {
        return transform.position;
    }
}
