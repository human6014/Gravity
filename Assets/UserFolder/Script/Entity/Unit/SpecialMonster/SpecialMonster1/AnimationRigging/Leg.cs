using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    // Self
    [SerializeField] private LegController legController;

    [Header("Rays")]

    [SerializeField] private bool m_HasDownRay;
    [Tooltip("몸체 -> 하단 감지 레이")]
    [SerializeField] private Transform downRayOrigin;


    [SerializeField] private bool m_HasForwardRay;
    [Tooltip("몸체 -> 정면 감지 레이")]
    [SerializeField] private Transform forwardRayOrigin;//rayOrigin : 감지용 위치(붉은 구)


    [SerializeField] private bool m_HasBackRay;
    [Tooltip("정면 -> 몸체 감지 레이")]
    [SerializeField] private Transform backRayOrigin;


    [Header("ikTarget")]
    [SerializeField] private Transform ikTarget;        //ikTarget : 발 바로 위 관절(이동 전)

    [Header("Details")]
    [Tooltip("한 걸음 길이")]
    [SerializeField] private float tipMoveDist = 2;

    [Tooltip("한 걸음 애니메이션 이동 시간")]
    [SerializeField] private float tipAnimationTime = 0.25f; //발 움직임 시간 0.15f

    [Tooltip("지면과 발 위치 높이")]
    [SerializeField] private float ikOffset = 2f;  // 발 위 관절 1.0f

    [Tooltip("정면 레이 길이")]
    [SerializeField] private float maxFowardRayDist = 2f; //정면 레이     3

    private readonly float maxDownRayDist = 10f; // 하단 레이    7
    private readonly float maxBackRayDist = 10f; // 정면 -> 몸체 레이    7
    private readonly float tipMaxHeight = 1.5f;  //걸을 때 관절 높이 0.2f
    private readonly float tipAnimationFrameTime = 0.02f;    // 1 / 60.0f
    private readonly float tipPassOver = 0.55f / 2.0f;   //0.55f/2.0f

    private float m_Interporation;
    private bool isJump;

    public void SetIsJump(bool _isJump) => isJump = _isJump;

    /// <summary>
    /// 발 위치
    /// </summary>
    public Vector3 TipPos { get; private set; }

    public Vector3 TipUpDir { get; private set; }

    public Vector3 RaycastTipPos { get; private set; }

    /// <summary>
    /// 발이 찍을 수 있는 감지된 땅 위치
    /// </summary>
    public Vector3 RaycastTipNormal { get; private set; }

    /// <summary>
    /// 해당 땅 오브젝트의 법선 벡터
    /// </summary>
    public bool Animating { get; private set; } = false;

    public bool Movable { get; set; } = false;

    /// <summary>
    /// 자기 발 위치와 ray에서 걸린 발 위치의 거리
    /// </summary>
    public float TipDistance { get; private set; }


    private void Awake() => TipPos = ikTarget.position;

    private void Start() => ikTarget.SetPositionAndRotation(TipPos + legController.BodyTransform.up.normalized * ikOffset,
                            Quaternion.LookRotation(TipPos - ikTarget.position) * Quaternion.Euler(180, 0, 0));

    RaycastHit hit;
    private void FixedUpdate()
    {
        // Calculate the tip target position
        //if (!IsIKEnable) return;
        if (m_HasForwardRay && Physics.Raycast(forwardRayOrigin.position, forwardRayOrigin.forward.normalized, out hit, maxFowardRayDist, legController.LayerMask))
        {
            RaycastTipPos = hit.point;
            RaycastTipNormal = hit.normal;
            m_Interporation = ikOffset - 3;
        }
        else if (m_HasBackRay && Physics.Raycast(backRayOrigin.position, backRayOrigin.forward.normalized * -1, out hit, maxBackRayDist, legController.LayerMaskBack))
        {
            RaycastTipPos = hit.point;
            RaycastTipNormal = hit.normal;
            m_Interporation = 0;
        }
        else if (m_HasDownRay && Physics.Raycast(downRayOrigin.position, downRayOrigin.up.normalized * -1, out hit, maxDownRayDist, legController.LayerMask))
        {
            RaycastTipPos = hit.point;
            RaycastTipNormal = hit.normal;
            m_Interporation = 0;
        }
        else
        {
            TipPos = RaycastTipPos = downRayOrigin.position + maxDownRayDist * -legController.BodyTransform.up.normalized;
            m_Interporation = 0;
            UpdateIKTargetTransform();
            return;
        }

        TipDistance = (RaycastTipPos - TipPos).magnitude;

        // If the distance gets too far, animate and move the tip to new position
        if (!Animating && (TipDistance > tipMoveDist && Movable))
            StartCoroutine(AnimateLeg()); //실질적인 다리(발) 애니메이팅(움직임)
    }

    private IEnumerator AnimateLeg()
    {
        Animating = true;

        float timer = 0.0f;
        float animTime, tipAcceleration;

        Vector3 startingTipPos = TipPos;
        Vector3 tipDirVec = RaycastTipPos - TipPos;
        tipDirVec += tipDirVec.normalized * tipPassOver;
        //발에서 진행방향

        Vector3 right = Vector3.Cross(legController.BodyTransform.up, tipDirVec.normalized).normalized;
        TipUpDir = Vector3.Cross(tipDirVec.normalized, right);
        //tipUpVec : 진행방향에서의 발에서 위 백터
        
        while (timer < tipAnimationTime + tipAnimationFrameTime)
        {
            animTime = legController.SpeedCurve.Evaluate(timer / tipAnimationTime);

            // If the target is keep moving, apply acceleration to correct the end point
            tipAcceleration = Mathf.Max((RaycastTipPos - startingTipPos).magnitude / tipDirVec.magnitude, 1);

            TipPos = startingTipPos + animTime * tipAcceleration * tipDirVec; // Forward direction of tip vector
            TipPos += legController.HeightCurve.Evaluate(animTime) * tipMaxHeight * TipUpDir; // Upward direction of tip vector

            UpdateIKTargetTransform();
            yield return null;

            timer += tipAnimationFrameTime;
        }
        Animating = false;
    }

    private void UpdateIKTargetTransform() =>
        ikTarget.SetPositionAndRotation(TipPos + legController.BodyTransform.up * ikOffset + legController.BodyTransform.forward * -m_Interporation,
        Quaternion.LookRotation(TipPos - ikTarget.position) * Quaternion.Euler(90, 0, 0));


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        /*
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(RaycastTipPos, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(TipPos, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(TipPos, RaycastTipPos);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(ikTarget.position, 0.1f);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(TipPos, TipPos + tipDirVec);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(TipPos, TipPos + TipUpDir);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(downRayOrigin.position, downRayOrigin.up.normalized * -maxDownRayDist);
        if (forwardRayOrigin)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(forwardRayOrigin.position, forwardRayOrigin.forward.normalized * maxFowardRayDist);
        }
        if (backRayOrigin)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(backRayOrigin.position, backRayOrigin.forward.normalized * -maxBackRayDist);
        }
        */
    }
#endif
}
