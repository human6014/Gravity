using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Scriptable.Monster.SP1MonsterLegScriptable m_SP1MonsterLegSettings;
    [SerializeField] private Transform m_BodyTransform;
    [SerializeField] private Transform ikTarget;        //ikTarget : 발 바로 위 관절(이동 전)

    [Header("Rays")]
    [Tooltip("몸체 -> 하단 감지 레이")]
    [SerializeField] private Transform downRayOrigin;
    [SerializeField] private float maxDownRayDist = 7f; // 하단 레이    7

    [Tooltip("몸체 -> 정면 감지 레이")]
    [SerializeField] private Transform forwardRayOrigin;//rayOrigin : 감지용 위치(붉은 구)
    [Tooltip("정면 레이 길이")]
    [SerializeField] private float maxFowardRayDist = 2f; //정면 레이     3

    [Tooltip("정면 -> 몸체 감지 레이")]
    [SerializeField] private Transform backRayOrigin;
    [SerializeField] private float maxBackRayDist = 7f; // 정면 -> 몸체 레이    7

    [Header("Details")]
    [Tooltip("한 걸음 길이")]
    [SerializeField] private float tipMoveDist = 2;

    [Tooltip("한 걸음 애니메이션 이동 시간")]
    [SerializeField] private float tipAnimationTime = 0.25f; //발 움직임 시간 0.15f

    [Tooltip("지면과 발 위치 높이")]
    [SerializeField] private float ikOffset = 2f;  // 발 위 관절 1.0f
    
    private readonly float m_TipMaxHeight = 1.5f;  //걸을 때 관절 높이 0.2f
    private readonly float m_TipAnimationFrameTime = 0.02f;    // 1 / 60.0f
    private readonly float m_TipPassOver = 0.55f / 2.0f;   //0.55f/2.0f

    private float m_Interporation;
    private bool m_IsJump;

    private bool m_HasDownRay;
    private bool m_HasForwardRay;
    private bool m_HasBackRay;

    public void SetIsJump(bool isJump)
    {
        m_IsJump = isJump;
    }

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


    private void Awake()
    {
        TipPos = ikTarget.position;
        m_HasDownRay = downRayOrigin != null;
        m_HasForwardRay = forwardRayOrigin != null;
        m_HasBackRay = backRayOrigin != null;
    }

    private void Start() => ikTarget.SetPositionAndRotation(TipPos + m_BodyTransform.up * ikOffset,
                            Quaternion.LookRotation(TipPos - ikTarget.position) * Quaternion.Euler(180, 0, 0));

    RaycastHit hit;
    private void FixedUpdate()
    {
        // Calculate the tip target position
        //if (!IsIKEnable) return;
        if (m_HasForwardRay && Physics.Raycast(forwardRayOrigin.position, forwardRayOrigin.forward, out hit, maxFowardRayDist, m_SP1MonsterLegSettings.m_FDLayerMask))
        {
            RaycastTipPos = hit.point;
            RaycastTipNormal = hit.normal;
            m_Interporation = ikOffset - 3;
        }
        else if (m_HasBackRay && Physics.Raycast(backRayOrigin.position, backRayOrigin.forward * -1, out hit, maxBackRayDist, m_SP1MonsterLegSettings.m_BLayerMask))
        {
            RaycastTipPos = hit.point;
            RaycastTipNormal = hit.normal;
            m_Interporation = 0;
        }
        else if (m_HasDownRay && Physics.Raycast(downRayOrigin.position, downRayOrigin.up * -1, out hit, maxDownRayDist, m_SP1MonsterLegSettings.m_FDLayerMask))
        {
            RaycastTipPos = hit.point;
            RaycastTipNormal = hit.normal;
            m_Interporation = 0;
        }
        else
        {
            TipPos = RaycastTipPos = downRayOrigin.position + maxDownRayDist * -m_BodyTransform.up;
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
        tipDirVec += tipDirVec.normalized * m_TipPassOver;
        //발에서 진행방향

        Vector3 right = Vector3.Cross(m_BodyTransform.up, tipDirVec.normalized).normalized;
        TipUpDir = Vector3.Cross(tipDirVec.normalized, right);
        //tipUpVec : 진행방향에서의 발에서 위 백터
        
        while (timer < tipAnimationTime + m_TipAnimationFrameTime)
        {
            animTime = m_SP1MonsterLegSettings.m_SpeedCurve.Evaluate(timer / tipAnimationTime);

            // If the target is keep moving, apply acceleration to correct the end point
            tipAcceleration = Mathf.Max((RaycastTipPos - startingTipPos).magnitude / tipDirVec.magnitude, 1);

            TipPos = startingTipPos + animTime * tipAcceleration * tipDirVec; // Forward direction of tip vector
            TipPos += m_SP1MonsterLegSettings.m_HeightCurve.Evaluate(animTime) * m_TipMaxHeight * TipUpDir; // Upward direction of tip vector

            UpdateIKTargetTransform();
            yield return null;

            timer += m_TipAnimationFrameTime;
        }
        Animating = false;
    }
    // + legController.BodyTransform.forward * -m_Interporation
    private void UpdateIKTargetTransform() =>
        ikTarget.SetPositionAndRotation(TipPos + m_BodyTransform.up * ikOffset,
        Quaternion.LookRotation(TipPos - ikTarget.position) * Quaternion.Euler(90, 0, 0));


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(RaycastTipPos, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(TipPos, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(TipPos, RaycastTipPos);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(ikTarget.position, 0.1f);

        //Gizmos.color = Color.white;
        //Gizmos.DrawLine(TipPos, TipPos + tipDirVec);

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
        
    }
#endif
}
