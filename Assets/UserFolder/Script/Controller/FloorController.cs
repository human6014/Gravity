using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;
using Manager;

namespace Contoller.Floor
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] floorObject;
        public void Start()
        {
            AIManager.FloorDetector = this;
        }
        public GameObject GetNowFloor()
        {
            return floorObject[(int)GravitiesManager.currentGravityType];
        }
    }
}
