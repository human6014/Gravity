using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using EnumType;
public class SpecialMonster1 : MonoBehaviour
{
    // Parabola
    [Header("Parabola")]
    [SerializeField] private float _step = 0.01f;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask layerMask;
    //[SerializeField] float height = 10;

    [Header("Code")]
    [SerializeField] private Transform target;
    [SerializeField] private Transform navObject;
    [SerializeField] private LegController legController;

    private Transform cachedTransform;
    private SpecialMonsterAI specialMonsterAI;

    private Vector3 groundDirection = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 direction;

    bool isPreJump = false;
    public Quaternion GetRotation() => cachedTransform.rotation;
    public Vector3 GetPosition() => cachedTransform.position;

    private void Awake()
    {
        cachedTransform = GetComponent<Transform>();
        specialMonsterAI = FindObjectOfType<SpecialMonsterAI>();
    }

    public void Init()
    {

    }

    private void FixedUpdate()
    {
        specialMonsterAI.OperateAIBehavior(cachedTransform.rotation);
    }
    void Update()
    {
        if (!target) return;

        direction = target.position - cachedTransform.position;
        float dir = 0;
        switch (GravitiesManager.currentGravityType)
        {
            case GravitiesType.xUp:
            case GravitiesType.xDown:
                groundDirection = new(0, direction.y, direction.z);
                dir = direction.x;
                break;

            case GravitiesType.yUp: //Init
            case GravitiesType.yDown:
                groundDirection = new(direction.x, 0, direction.z);
                dir = direction.y;
                break;

            case GravitiesType.zUp:
            case GravitiesType.zDown:
                groundDirection = new(direction.x, direction.y, 0);
                dir = direction.z;
                break;
        }
        targetPos = new(groundDirection.magnitude, dir * -GravitiesManager.GravityDirectionValue, 0);
        float height = Mathf.Max(0.01f, targetPos.y + targetPos.magnitude / 2f);

        CalculatePathWithHeight(targetPos, height, out float v0, out float angle, out float time);

        //DrawPath(groundDirection.normalized, v0, angle, time, _step); //경로 그리기

        if (Input.GetKeyDown(KeyCode.Backspace) && !isPreJump && !specialMonsterAI.GetIsOnOffMeshLink())
            StartCoroutine(Coroutine_Movement(groundDirection.normalized, v0, angle, time));
    }

    public void StartJumpCoroutine()
    {
        //StartCoroutine(Jump(0.9f));
    }

    #region 포물선 테스트중




#if UNITY_EDITOR
    private void DrawPath(Vector3 direction, float v0, float angle, float time, float step)
    {
        step = Mathf.Max(0.01f, step);
        lineRenderer.positionCount = (int)(time / step) + 2;
        int count = 0;
        for (float i = 0; i < time; i += step)
        {
            float x = v0 * i * Mathf.Cos(angle);
            float y = v0 * i * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(i, 2);
            lineRenderer.SetPosition(count, cachedTransform.position + direction * x - GravitiesManager.GravityVector * y);

            count++;
        }
        float xFinal = v0 * time * Mathf.Cos(angle);
        float yFinal = v0 * time * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(time, 2);
        lineRenderer.SetPosition(count, cachedTransform.position + direction * xFinal - GravitiesManager.GravityVector * yFinal);
    }
#endif

    private float QuadraticEquation(float a, float b, float c, float sign) =>
        (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
    
    private void CalculatePathWithHeight(Vector2 targetPos, float h, out float v0, out float angle, out float time)
    {
        float g = Physics.gravity.magnitude;

        float a = (-0.5f * g);
        float b = Mathf.Sqrt(2 * g * h);
        float c = -targetPos.y;

        float tplus = QuadraticEquation(a, b, c, 1);
        float tmin = QuadraticEquation(a, b, c, -1);
        time = tplus > tmin ? tplus : tmin;

        angle = Mathf.Atan(b * time / targetPos.x);

        v0 = b / Mathf.Sin(angle);
    }

    IEnumerator Coroutine_Movement(Vector3 direction, float v0, float angle, float time)
    {
        specialMonsterAI.SetNavMeshEnable(false);
        isPreJump = true;
        legController.SetPreJump(true);

        float beforeJumpingTime = 1f;
        float elapsedTime = 0;
        Vector3 startPos = cachedTransform.position;
        while (elapsedTime < beforeJumpingTime)
        {
            cachedTransform.position = Vector3.Lerp(startPos, startPos - transform.up, elapsedTime / beforeJumpingTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        legController.SetPreJump(false);
        legController.Jump(true);

        elapsedTime = 0;
        startPos = cachedTransform.position;
        Quaternion startRotation = cachedTransform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(groundDirection,target.up);
        Debug.Log("time : " + time);
        while (elapsedTime < time)
        {
            float x = v0 * elapsedTime * Mathf.Cos(angle);
            float y = v0 * elapsedTime * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(elapsedTime, 2);
            transform.SetPositionAndRotation(startPos + direction * x - GravitiesManager.GravityVector * y, 
                             Quaternion.Lerp(startRotation, targetRot, elapsedTime / time));
            //elapsedTime += Time.deltaTime * (time / 1.5f);
            //elapsedTime += Time.deltaTime;
            elapsedTime += Time.deltaTime * 2.5f;
            yield return null;
        }
        legController.Jump(false);
        isPreJump = false;
        specialMonsterAI.SetNavMeshEnable(true);
        specialMonsterAI.SetNavMeshPos(transform.position);
    }
    #endregion
}
