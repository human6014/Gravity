using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Object
{
    public class DefaultPoolingScript : PoolableScript
    {
        private Manager.ObjectPoolManager.PoolingObject poolingObject;

        public void Init(Vector3 pos, Quaternion rot ,Manager.ObjectPoolManager.PoolingObject poolingObject)
        {
            transform.SetPositionAndRotation(pos, rot);
            this.poolingObject = poolingObject;
            Invoke(nameof(ReturnObject), 5);
        }
        
        public override void ReturnObject() => poolingObject.ReturnObject(this);
    }
}
