using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsMonster : PoolableScript
{
    #region Variables & Initializer
    [Header("Info")]
    [SerializeField] private Scriptable.Monster.BoidsScriptable settings;
    [SerializeField] private Scriptable.Monster.FlyingMonsterScriptable m_FlyingMonsterScriptable;

    private WaitForSeconds calcEgoWaitSeconds;
    private WaitForSeconds findNeighbourSeconds;

    private readonly List<BoidsMonster> neighbours = new();
    private BoidsController myBoids;

    private Transform m_Target;

    private float m_Speed;
    private float m_AdditionalSpeed;

    private Vector3 m_TargetVec;
    private Vector3 m_EgoVector;

    private Vector3 m_CohesionVec;
    private Vector3 m_AlignmentVec;
    private Vector3 m_SeparationVec;

    private Vector3 m_TargetForwardVec;
    private Vector3 m_BoundsVec;
    private Vector3 m_ObstacleVec;
    #endregion

    public void Init(BoidsController _boids)
    {
        myBoids = _boids;
        m_Speed = Random.Range(settings.speedRange.x, settings.speedRange.y);
        m_Target = Manager.AI.AIManager.PlayerTransform;

        calcEgoWaitSeconds = new WaitForSeconds(Random.Range(1f, 3f));
        findNeighbourSeconds = new WaitForSeconds(Random.Range(1f, 2f));

        StartCoroutine(FindNeighbourCoroutine());
        StartCoroutine(CalculateEgoVectorCoroutine());
    }

    private void Update()
    {
        if (m_AdditionalSpeed > 0) m_AdditionalSpeed -= Time.deltaTime;

        CalculateVectors();
        // Calculate all the vectors we need
        m_CohesionVec *= settings.cohesionWeight;
        m_AlignmentVec *= settings.alignmentWeight;
        m_SeparationVec *= settings.separationWeight;

        // 추가적인 방향
        if (m_Target != null && Input.GetKey(KeyCode.Tab)) //공격 패턴 주기시마다 하게 함
            m_TargetForwardVec = CalculateTargetVector() * settings.targetWeight;
        else m_BoundsVec = CalculateBoundsVector() * settings.boundsWeight;
        m_ObstacleVec = CalculateObstacleVector() * settings.obstacleWeight;

        m_TargetVec = m_CohesionVec + m_AlignmentVec + m_SeparationVec + m_BoundsVec + m_ObstacleVec + (m_EgoVector * settings.egoWeight) + m_TargetForwardVec;

        // Steer and Move
        if (m_TargetVec == Vector3.zero) m_TargetVec = m_EgoVector;
        else m_TargetVec = Vector3.Lerp(transform.forward, m_TargetVec, Time.deltaTime).normalized;

        transform.SetPositionAndRotation(transform.position + (m_Speed + m_AdditionalSpeed) * Time.deltaTime * m_TargetVec,
                                        Quaternion.LookRotation(m_TargetVec));
    }

    #region Calculate Vectors
    private IEnumerator CalculateEgoVectorCoroutine()
    {
        while (true)
        {
            m_Speed = Random.Range(settings.speedRange.x, settings.speedRange.y);
            m_EgoVector = Random.insideUnitSphere;
            yield return calcEgoWaitSeconds;
        }
    }

    private IEnumerator FindNeighbourCoroutine()
    {
        Collider[] colls;
        while (true)
        {
            neighbours.Clear();

            colls = Physics.OverlapSphere(transform.position, settings.neighbourDistance, settings.boidUnitLayer);
            for (int i = 0; i < colls.Length && i <= settings.maxNeighbourCount; i++)
            {
                if (Vector3.Angle(transform.forward, colls[i].transform.position - transform.position) <= settings.FOVAngle)
                    neighbours.Add(colls[i].GetComponent<BoidsMonster>());
            }
            yield return findNeighbourSeconds;
        }
    }

    private void CalculateVectors()
    {
        m_CohesionVec = Vector3.zero;
        m_AlignmentVec = transform.forward;
        m_SeparationVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // 이웃 unit들의 위치 더하기
            for (int i = 0; i < neighbours.Count; i++)
            {
                m_CohesionVec += neighbours[i].transform.position;
                m_AlignmentVec += neighbours[i].transform.forward;
                m_SeparationVec += (transform.position - neighbours[i].transform.position);
            }

            // 중심 위치로의 벡터 찾기
            m_CohesionVec /= neighbours.Count;
            m_AlignmentVec /= neighbours.Count;
            m_SeparationVec /= neighbours.Count;
            m_CohesionVec -= transform.position;

            m_CohesionVec.Normalize();
            m_AlignmentVec.Normalize();
            m_SeparationVec.Normalize();
        }
    }

    private Vector3 CalculateBoundsVector()
    {
        m_TargetForwardVec = Vector3.zero;
        Vector3 offsetToCenter = myBoids.transform.position - transform.position;
        return offsetToCenter.magnitude >= myBoids.SpawnRange ? offsetToCenter.normalized : Vector3.zero;
    }

    private Vector3 CalculateObstacleVector()
    {
        Vector3 obstacleVec = Vector3.zero;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, settings.obstacleDistance, settings.obstacleLayer))
        {
            obstacleVec = hit.normal;
            m_AdditionalSpeed = 10;
        }
        return obstacleVec;
    }

    private Vector3 CalculateTargetVector()
    {
        m_BoundsVec = Vector3.zero;
        return (m_Target.position - transform.position).normalized;
    }
    #endregion

    //호출할 때 this
    public override void ReturnObject()
    {
        myBoids.ReturnObj(this);
    }
}
