using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsMonster : PoolableScript
{
    #region Variables & Initializer
    [Header("Info")]
    [SerializeField] private BoidsScriptable settings;

    private List<BoidsMonster> neighbours = new();
    private Boids myBoids;

    private Transform target;
    private Transform cachedTransform;

    private float speed;
    private float additionalSpeed = 0;

    private Vector3 targetVec;
    private Vector3 egoVector;

    private Vector3 cohesionVec;
    private Vector3 alignmentVec;
    private Vector3 separationVec;

    private Vector3 targetForwardVec;
    private Vector3 boundsVec;
    private Vector3 obstacleVec;
    private Vector3 egoVec;
    #endregion
    public void InitializeUnit(Boids _boids, Transform _target)
    {
        myBoids = _boids;
        speed = Random.Range(settings.speedRange.x, settings.speedRange.y);
        target = _target;

        cachedTransform = GetComponent<Transform>();
        StartCoroutine(FindNeighbourCoroutine());
        StartCoroutine(CalculateEgoVectorCoroutine());
    }
    void Update()
    {
        if (additionalSpeed > 0) additionalSpeed -= Time.deltaTime;

        CalculateVectors();
        // Calculate all the vectors we need
        cohesionVec *= settings.cohesionWeight;
        alignmentVec *= settings.alignmentWeight;
        separationVec *= settings.separationWeight;

        // 추가적인 방향
        if (target != null && Input.GetKey(KeyCode.Tab)) //공격 패턴 주기시마다 하게 함
        {
            targetForwardVec = CalculateTargetVector() * settings.targetWeight;
        }
        else boundsVec = CalculateBoundsVector() * settings.boundsWeight;
        obstacleVec = CalculateObstacleVector() * settings.obstacleWeight;
        egoVec = egoVector * settings.egoWeight;

        targetVec = cohesionVec + alignmentVec + separationVec + boundsVec + obstacleVec + egoVec + targetForwardVec;

        // Steer and Move
        if (targetVec == Vector3.zero) targetVec = egoVector;
        else targetVec = Vector3.Lerp(cachedTransform.forward, targetVec, Time.deltaTime).normalized;

        cachedTransform.SetPositionAndRotation(cachedTransform.position + (speed + additionalSpeed) * Time.deltaTime * targetVec,
                                        Quaternion.LookRotation(targetVec));
    }
    #region Calculate Vectors
    IEnumerator CalculateEgoVectorCoroutine()
    {
        while (true)
        {
            speed = Random.Range(settings.speedRange.x, settings.speedRange.y);
            egoVector = Random.insideUnitSphere;
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
    
    IEnumerator FindNeighbourCoroutine()
    {
        Collider[] colls;
        while (true)
        {
            neighbours.Clear();

            colls = Physics.OverlapSphere(cachedTransform.position, settings.neighbourDistance, settings.boidUnitLayer);
            for (int i = 0; i < colls.Length && i <= settings.maxNeighbourCount; i++)
            {
                if (Vector3.Angle(cachedTransform.forward, colls[i].transform.position - cachedTransform.position) <= settings.FOVAngle)
                    neighbours.Add(colls[i].GetComponent<BoidsMonster>());
            }
            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
    }

    private void CalculateVectors()
    {
        cohesionVec = Vector3.zero;
        alignmentVec = cachedTransform.forward;
        separationVec = Vector3.zero;
        if (neighbours.Count > 0)
        {
            // 이웃 unit들의 위치 더하기
            for (int i = 0; i < neighbours.Count; i++)
            {
                cohesionVec += neighbours[i].transform.position;
                alignmentVec += neighbours[i].transform.forward;
                separationVec += (cachedTransform.position - neighbours[i].transform.position);
            }

            // 중심 위치로의 벡터 찾기
            cohesionVec /= neighbours.Count;
            alignmentVec /= neighbours.Count;
            separationVec /= neighbours.Count;
            cohesionVec -= cachedTransform.position;

            cohesionVec.Normalize();
            alignmentVec.Normalize();
            separationVec.Normalize();
        }
    }

    private Vector3 CalculateBoundsVector()
    {
        targetForwardVec = Vector3.zero;
        Vector3 offsetToCenter = myBoids.transform.position - cachedTransform.position;
        return offsetToCenter.magnitude >= myBoids.spawnRange ? offsetToCenter.normalized : Vector3.zero;
    }

    private Vector3 CalculateObstacleVector()
    {
        Vector3 obstacleVec = Vector3.zero;
        if (Physics.Raycast(cachedTransform.position, cachedTransform.forward, out RaycastHit hit, settings.obstacleDistance, settings.obstacleLayer))
        {
            obstacleVec = hit.normal;
            additionalSpeed = 10;
        }
        return obstacleVec;
    }

    private Vector3 CalculateTargetVector()
    {
        boundsVec = Vector3.zero;
        Vector3 offsetToTarget = target.position - cachedTransform.position;
        return offsetToTarget.normalized;
    }

    public override void ReturnObject()
    {
        myBoids.ReturnObj(this);
    }
    #endregion

}
