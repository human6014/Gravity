using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Flying;
using Entity.Unit.Normal;
using Entity.Unit.Special;
using Entity.Object;
using System.Linq;


namespace Manager
{
    [RequireComponent(typeof(EntityManager))]
    public class SpawnManager : MonoBehaviour
    {
        public static int NormalMonsterCount { get; set; }
        public static int FlyingMonsterCount { get; set; }

        public bool IsSP1MonsterSpawned { get; private set; }
        public bool IsSP2MonsterSpawned { get; private set; }
        public bool IsSP3MonsterSpawned { get; private set; }

        #region SerializeField
        [Tooltip("���� ���� ���� (Editor only)")]
        [SerializeField] private bool m_IsActiveSpawn;

        [Header("Spawn Area")]
        [Tooltip("���������� �ڽ����� ������ Transform")]
        [SerializeField] private Transform [] spawnAreaTransform = new Transform[6];

        [Header("Pooling Transform")]
        [Tooltip("Ȱ��ȭ�� ������ transform")]
        [SerializeField] private Transform activeUnitPool;

        [Tooltip("Ȱ��ȭ�� ����� ������Ʈ�� transform")]
        [SerializeField] private Transform activeObjectPool;

        [Header("Pooling Count")]
        [Tooltip("�̸� ������ ���� �� urban -> oldman -> women -> big -> giant")]
        [Range(0, 100)][SerializeField] private int[] normalMonsterPoolingCount;

        [Tooltip("�̸� ������ FlyingMonstr ��")]
        [Range(0, 100)] [SerializeField] private int[] flyingMonsterPoolingCount;

        [Tooltip("�̸� ������ ����Ʈ ����")]
        [Range(0, 100)] [SerializeField] private int[] m_EffectPoolingCount;

        [Tooltip("�̸� ������ ź�� ����")]
        [Range(0, 100)] [SerializeField] private int[] m_CasingPoolingCount;

        [Tooltip("�̸� ������ ź���� ����")]
        [Range(0, 30)] [SerializeField] private int[] m_MagazinePoolingCount;

        [Header("Spawn info")]
        [Tooltip("�ִ�� ������ �Ϲ� ���� �� ����")]
        [Range(0, 100)][SerializeField] private int maxNormalMonsterCount = 30;

        [Tooltip("�ִ�� ������ ���� ���� �� ����")]
        [Range(0, 50)] [SerializeField] private int maxFlyingMonsterCount = 10;

        [Tooltip("�Ϲ� ���� ��ȯ �ֱ�")]
        [SerializeField] private float normalMonsterSpawnTime = 3;

        [Tooltip("���� ���� ��ȯ �ֱ�")]
        [SerializeField] private float flyingMonsterSpawnTime = 5;
        #endregion

        #region Object Value
        private readonly BoxCollider[][] m_SpawnAreaCollider = new BoxCollider[6][];
        private BoxCollider[] m_CurrentAreaCollider;

        private EntityManager m_EntityManager;
        private Customization m_Customization;
        

        private ObjectPoolManager.PoolingObject[] m_NormalMonsterPoolingObjectArray;
        private ObjectPoolManager.PoolingObject[] m_FlyingMonsterPoolingObjectArray;
        private ObjectPoolManager.PoolingObject[] m_EffectPoolingObjectArray;
        private ObjectPoolManager.PoolingObject[] m_CasingPoolingObjectArray;
        private ObjectPoolManager.PoolingObject[] m_MagazinePoolingObjectArray;
        #endregion

        #region Normal Value
        private EnumType.GravityType currentGravityType = EnumType.GravityType.yUp;
        private readonly float[] m_Probs = new float[] { 45, 20, 20, 10, 5 };
        private float m_Total = 0;

        private float m_NormalMonsterTimer;
        private float m_FlyingMonsterTimer;

        private int m_RandomNormalMonsterIndex;
        private int m_RandomFlyingMonsterIndex;
        #endregion

        [SerializeField] private WeaponManager m_WeaponManager;
        [SerializeField] Octree octree;        //temp
        private void Awake()
        {
            m_EntityManager = GetComponent<EntityManager>();
            m_Customization = GetComponent<Customization>();
            //m_WeaponManager = FindObjectOfType<WeaponManager>();

            for (int i = 0; i < spawnAreaTransform.Length; i++)
                m_SpawnAreaCollider[i] = spawnAreaTransform[i].GetComponentsInChildren<BoxCollider>();
            
            m_CurrentAreaCollider = m_SpawnAreaCollider[2]; // YDown

            NormalMonsterCount = 0;
            FlyingMonsterCount = 0;

            foreach (float elem in m_Probs) m_Total += elem;
        }

        private void Start()
        {
            RegisterPoolingObject();
            GravityManager.GravityChangeAction += (GravityType) => ChangeCurrentArea(GravityType);
        }

        private void RegisterPoolingObject()
        {
            int normalMonsterArrayLength = m_EntityManager.GetNormalMonsterArrayLength();
            int flyingMonsterArrayLength = m_EntityManager.GetFlyingMonsterArrayLength();
            int effectArrayLength = m_EntityManager.GetEffectArrayLength();
            int casingArrayLength = m_EntityManager.GetCasingArrayLength();
            int magazineArrayLength = m_EntityManager.GetMagazineArrayLength();

            if (normalMonsterArrayLength != normalMonsterPoolingCount.Length)
                Debug.LogError("Normal monster pooling Count is different from the number of PoolingObject's length");
            if (flyingMonsterArrayLength != flyingMonsterPoolingCount.Length)
                Debug.LogError("Flying monster pooling Count is different from the number of PoolingObject's length");
            if (effectArrayLength != m_EffectPoolingCount.Length)
                Debug.LogError("Effect pooling Count is different from the number of PoolingObject's length");
            if (casingArrayLength != m_CasingPoolingCount.Length)
                Debug.LogError("Casing pooling Count is different from the number of PoolingObject's length");
            if (magazineArrayLength != m_MagazinePoolingCount.Length)
                Debug.LogError("Magazine pooling Count is different from the number of PoolingObject's length");

            //normal monster �ε��� ����
            //urban -> oldman -> women -> big -> giant
            m_NormalMonsterPoolingObjectArray = new ObjectPoolManager.PoolingObject[normalMonsterArrayLength];
            m_FlyingMonsterPoolingObjectArray = new ObjectPoolManager.PoolingObject[flyingMonsterArrayLength];
            m_EffectPoolingObjectArray = new ObjectPoolManager.PoolingObject[effectArrayLength];
            m_CasingPoolingObjectArray = new ObjectPoolManager.PoolingObject[casingArrayLength];
            m_MagazinePoolingObjectArray = new ObjectPoolManager.PoolingObject[magazineArrayLength];

            //Unit
            for (int i = 0; i < normalMonsterArrayLength; i++)
            {
                m_NormalMonsterPoolingObjectArray[i] = ObjectPoolManager.Register(m_EntityManager.GetNormalMonster(i), activeUnitPool);
                m_NormalMonsterPoolingObjectArray[i].GenerateObj(normalMonsterPoolingCount[i]);
            }

            for (int i = 0; i < flyingMonsterArrayLength; i++)
            {
                m_FlyingMonsterPoolingObjectArray[i] = ObjectPoolManager.Register(m_EntityManager.GetFlyingMonster(i), activeUnitPool);
                m_FlyingMonsterPoolingObjectArray[i].GenerateObj(flyingMonsterPoolingCount[i]);
            }

            //Object
            for(int i = 0; i < effectArrayLength; i++)
            {
                m_EffectPoolingObjectArray[i] = ObjectPoolManager.Register(m_EntityManager.GetEffectObject(i), activeObjectPool);
                m_EffectPoolingObjectArray[i].GenerateObj(m_EffectPoolingCount[i]);
            }

            for (int i = 0; i < casingArrayLength; i++)
            {
                m_CasingPoolingObjectArray[i] = ObjectPoolManager.Register(m_EntityManager.GetCasingObject(i), activeObjectPool);
                m_CasingPoolingObjectArray[i].GenerateObj(m_CasingPoolingCount[i]);
            }

            for (int i = 0; i < magazineArrayLength; i++)
            {
                m_MagazinePoolingObjectArray[i] = ObjectPoolManager.Register(m_EntityManager.GetMagazineObject(i), activeObjectPool);
                m_MagazinePoolingObjectArray[i].GenerateObj(m_MagazinePoolingCount[i]);
            }

            //if (m_WeaponManager == null) return;
            m_WeaponManager.SetObjectPool(m_EffectPoolingObjectArray, m_CasingPoolingObjectArray, m_MagazinePoolingObjectArray);
        }

        public void ChangeCurrentArea(EnumType.GravityType gravityType)
        {
            currentGravityType = gravityType;
            m_CurrentAreaCollider = m_SpawnAreaCollider[(int)gravityType];
        }

        #region Random related

        /// <summary>
        /// BoxCollider[] �迭 ���� ������ BoxCollider �ϳ��� ��ȯ��
        /// </summary>
        /// <param name="boxColliders">BoxCollider[]</param>
        /// <returns>BoxCollider</returns>
        private BoxCollider GetClosetArea(BoxCollider[] boxColliders)
        {
            int rand = Random.Range(0, boxColliders.Length);
            return boxColliders[rand];
        }

        /// <summary>
        /// BoxCollider ���� ������ ��ġ�� ��ȯ��
        /// </summary>
        /// <param name="boxCollider">BoxCollider</param>
        /// <returns>Vector3</returns>
        private Vector3 GetRandomPos(BoxCollider boxCollider, float interporateRadius)
        {
            interporateRadius *= 0.5f;

            float rangeX = Random.Range(
                -boxCollider.bounds.size.x * 0.5f + interporateRadius,
                boxCollider.bounds.size.x * 0.5f - interporateRadius);
            float rangeZ = Random.Range(
                -boxCollider.bounds.size.z * 0.5f + interporateRadius, 
                boxCollider.bounds.size.z * 0.5f - interporateRadius);

            return boxCollider.transform.position + new Vector3(rangeX, 0, rangeZ);
        }

        /// <summary>
        /// ���� �ٸ� Ȯ������ ������ index ����
        /// </summary>
        /// <returns>������ int index</returns>
        private int RandomUnitIndex()
        {
            float randomPoint = Random.value * m_Total;

            for (int i = 0; i < m_Probs.Length; i++)
            {
                if (randomPoint < m_Probs[i]) return i;
                else randomPoint -= m_Probs[i];
            }
            return m_Probs.Length - 1;
        }

        /// <summary>
        /// spawnAreaCollider �迭���� Ư�� ���� �����ϰ� ������ index ����
        /// </summary>
        /// <param name="excludeIndex">������ index</param>
        /// <param name="specificIndex">excludeIndex�� �����ϰ� �������� ���� index</param>
        /// <returns>excludeIndex�� �����ϰ� �������� ���� BoxCollider[]</returns>
        private BoxCollider[] ExcludeRandomIndex(int excludeIndex, out int specificIndex)
        {
            HashSet<int> exclude = new() { excludeIndex };
            IEnumerable<int> range = Enumerable.Range(0, m_SpawnAreaCollider.Length).Where(i => !exclude.Contains(i));
            
            int index = Random.Range(0,m_SpawnAreaCollider.Length - exclude.Count);
            specificIndex = range.ElementAt(index);
            return m_SpawnAreaCollider[range.ElementAt(index)];
        }
        #endregion

        private void Update()
        {
            if (!m_IsActiveSpawn) return;

            m_NormalMonsterTimer += Time.deltaTime;
            m_FlyingMonsterTimer += Time.deltaTime;

            if (m_NormalMonsterTimer >= normalMonsterSpawnTime && NormalMonsterCount < maxNormalMonsterCount) SpawnNormalMonster();
            if (m_FlyingMonsterTimer >= flyingMonsterSpawnTime && FlyingMonsterCount < maxFlyingMonsterCount) SpawnFlyingMonster();
        }

        #region SpawnUnit
        private void SpawnNormalMonster()
        {
            m_NormalMonsterTimer = 0;
            m_RandomNormalMonsterIndex = RandomUnitIndex();
            NormalMonsterCount++;

            NormalMonster currentNormalMonster = (NormalMonster)m_NormalMonsterPoolingObjectArray[m_RandomNormalMonsterIndex].GetObject(false);
            m_Customization.Customize(currentNormalMonster);

            BoxCollider boxCollider = GetClosetArea(m_CurrentAreaCollider);
            Vector3 pos = GetRandomPos(boxCollider, 1);

            currentNormalMonster.Init(pos, m_NormalMonsterPoolingObjectArray[m_RandomNormalMonsterIndex]);
            currentNormalMonster.gameObject.SetActive(true);
        }

        private void SpawnFlyingMonster()
        {
            m_FlyingMonsterTimer = 0;
            //index �Ҵ� ���� �ʿ� x
            FlyingMonsterCount++;

            FlyingMonster currentFlyingMonster = (FlyingMonster)m_FlyingMonsterPoolingObjectArray[0].GetObject(false);
            currentFlyingMonster.Init(octree.GetRandomSpawnableArea(), m_FlyingMonsterPoolingObjectArray[0]);
            currentFlyingMonster.gameObject.SetActive(true);
        }

        [ContextMenu("SpawnSpecialMonster")]
        private void SpawnSpecialMonster1()
        {
            BoxCollider[] initColliders = ExcludeRandomIndex((int)currentGravityType,out int specificIndex);
            BoxCollider initCollider = GetClosetArea(initColliders);
            Vector3 initPosition = GetRandomPos(initCollider, 4);
            Quaternion initRotation = GravityManager.GetSpecificGravityNormalRotation(specificIndex);

            SpecialMonster1 specialMonster1 = Instantiate(m_EntityManager.GetSpecialMonster1, initPosition, Quaternion.identity).GetComponent<SpecialMonster1>();
            specialMonster1.Init(initRotation);

            IsSP1MonsterSpawned = true;
        }

        private void SpawnSpecialMonster2()
        {
            IsSP2MonsterSpawned = true;
        }

        private void SpawnSpecialMonster3()
        {
            IsSP3MonsterSpawned = true;
        }
        #endregion
    }
}
