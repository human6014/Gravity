using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class UnitManager : MonoBehaviour
    {
        private ObjectPoolManager.PoolingObject poolingObj;

        [SerializeField] private GameObject oldmanZombie;
        [SerializeField] private GameObject urbanZombie;
        [SerializeField] private GameObject womenZombie;
        [SerializeField] private GameObject bigZomibe;
        [SerializeField] private GameObject giantZombie;

        [SerializeField] private GameObject specialMonster1;
        [SerializeField] private GameObject specialMonster2;
        [SerializeField] private GameObject specialMonster3;


        public GameObject OldmanZombie { get => oldmanZombie; private set => oldmanZombie = value; }
        public GameObject UrbanZombie { get => urbanZombie; private set => urbanZombie = value; }
        public GameObject WomenZombie { get => womenZombie; private set => womenZombie = value; }
        public GameObject BigZomibe { get => bigZomibe; private set => bigZomibe = value; }
        public GameObject GiantZombie { get => giantZombie; private set => giantZombie = value; }

        public GameObject SpecialMonster1 { get => specialMonster1; private set => specialMonster1 = value; }
        public GameObject SpecialMonster2 { get => specialMonster2; private set => specialMonster2 = value; }
        public GameObject SpecialMonster3 { get => specialMonster3; private set => specialMonster3 = value; }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
