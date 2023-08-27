using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSphere : PoolableScript
{
    [SerializeField] private float m_MovementSpeed = 1;

    private ParticleEndSystem m_ParticleEndSystem;
    private Rigidbody m_Rigidbody;
    private SphereCollider m_SphereCollider;

    private int m_Damage;
    private bool m_CanMove;

    private void Awake()
    {
        m_ParticleEndSystem = GetComponent<ParticleEndSystem>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_SphereCollider = GetComponent<SphereCollider>();
    }

    public void Init(Manager.ObjectPoolManager.PoolingObject poolingObject, Vector3 pos, Quaternion rot, int damage)
    {
        m_CanMove = true;
        m_PoolingObject = poolingObject;
        transform.SetPositionAndRotation(pos,rot);
        m_Damage = damage;
    }

    private void Update()
    {
        if (!m_CanMove) return;
        transform.Translate(m_MovementSpeed * Time.deltaTime * transform.forward, Space.World);
    }

    public override void ReturnObject()
    {
        m_PoolingObject.ReturnObject(this);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!m_CanMove) return;
    //    m_CanMove = false;
    //    GameObject collisionObj = collision.gameObject;
    //    if(collisionObj.layer == 7)
    //        collisionObj.GetComponent<PlayerData>().PlayerHit(transform, m_Damage, AttackType.None);
        
    //    m_Rigidbody.isKinematic = true;
    //    m_SphereCollider.isTrigger = true;


    //    m_ParticleEndSystem.TurnOffParticles();
    //    Invoke(nameof(ReturnObject),5);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (!m_CanMove) return;
        m_CanMove = false;
        GameObject collisionObj = other.gameObject;
        if (collisionObj.layer == 7)
            collisionObj.GetComponent<PlayerData>().PlayerHit(transform, m_Damage, AttackType.None);

        m_Rigidbody.isKinematic = true;

        m_ParticleEndSystem.TurnOffParticles();
        Invoke(nameof(ReturnObject), 5);
    }
}
