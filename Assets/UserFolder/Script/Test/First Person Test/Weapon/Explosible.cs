using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosible : PoolableScript
{
    [Header("Effect")]
    [Tooltip("활성, 비활성화 할 이펙트 오브젝트")]
    [SerializeField] protected GameObject[] m_ActivatingEffectObject;

    [Header("Info")]
    [Tooltip("자동으로 폭파하는 시간")]
    [SerializeField] private float m_AutoExplosionTime = 3;

    [Tooltip("이펙트 지속 시간")]
    [SerializeField] private float m_EffectDuration = 5;

    [Tooltip("이펙트가 꺼지는 데 걸리는 시간")]
    [SerializeField] protected float m_StopDuration = 5;

    private Rigidbody m_Rigidbody;
    private MeshRenderer m_MeshRenderer;

    private WaitForSeconds m_AutoExplosionSecond;
    private WaitForSeconds m_DestroyObjectSecond;

    protected bool m_IsExploded;

    public Action<bool, Collider> m_TriggerStay;

    protected virtual void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_MeshRenderer = GetComponent<MeshRenderer>();

        m_AutoExplosionSecond = new WaitForSeconds(m_AutoExplosionTime);
        m_DestroyObjectSecond = new WaitForSeconds(m_EffectDuration);
    }

    public virtual void Init(Manager.ObjectPoolManager.PoolingObject poolingObject, Vector3 pos, Quaternion rot)
    {
        m_PoolingObject = poolingObject;
        m_IsExploded = m_Rigidbody.isKinematic = false;
        m_MeshRenderer.enabled = true;
        transform.SetPositionAndRotation(pos, rot);
    }

    protected IEnumerator Explosion()
    {
        yield return m_AutoExplosionSecond;
        m_IsExploded = m_Rigidbody.isKinematic = true;
        m_MeshRenderer.enabled = false;

        for (int i = 0; i < m_ActivatingEffectObject.Length; i++)
            m_ActivatingEffectObject[i].SetActive(true);
        
        yield return m_DestroyObjectSecond;
    }

    protected void EndExplosion()
    {
        for (int i = 0; i < m_ActivatingEffectObject.Length; i++)
            m_ActivatingEffectObject[i].SetActive(false);
    }

    protected virtual void Damage(bool isInside, Collider other)
    {
        if (!m_IsExploded) return;
    }

    public override void ReturnObject() => m_PoolingObject.ReturnObject(this);
}
