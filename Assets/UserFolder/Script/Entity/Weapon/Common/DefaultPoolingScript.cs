using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Object
{
    public class DefaultPoolingScript : PoolableScript
    {
        public void Init(Vector3 pos, Quaternion rot ,Manager.ObjectPoolManager.PoolingObject poolingObject)
        {
            transform.SetPositionAndRotation(pos, rot);
            this.m_PoolingObject = poolingObject;
            Invoke(nameof(ReturnObject), 5);
        }
        
        public override void ReturnObject() => m_PoolingObject.ReturnObject(this);
    }
}
