using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Scriptable.Monster.SP1MonsterLegScriptable m_SP1MonsterLegSettings;
    [SerializeField] private Transform m_BodyTransform;
    [SerializeField] private Transform ikTarget;        //ikTarget : �� �ٷ� �� ����(�̵� ��)

    [Header("Rays")]
    [Tooltip("��ü -> �ϴ� ���� ����")]
    [SerializeField] private Transform downRayOrigin;
    [SerializeField] private float maxDownRayDist = 7f; // �ϴ� ����    7

    [Tooltip("��ü -> ���� ���� ����")]
    [SerializeField] private Transform forwardRayOrigin;//rayOrigin : ������ ��ġ(���� ��)
    [Tooltip("���� ���� ����")]
    [SerializeField] private float maxFowardRayDist = 2f; //���� ����     3

    [Tooltip("���� -> ��ü ���� ����")]
    [SerializeField] private Transform backRayOrigin;
    [SerializeField] private float maxBackRayDist = 7f; // ���� -> ��ü ����    7

    [Header("Details")]
    [Tooltip("�� ���� ����")]
    [SerializeField] private float tipMoveDist = 2;

    [Tooltip("�� ���� �ִϸ��̼� �̵� �ð�")]
    [SerializeField] private float tipAnimationTime = 0.25f; //�� ������ �ð� 0.15f

    [Tooltip("����� �� ��ġ ����")]
    [SerializeField] private float ikOffset = 2f;  // �� �� ���� 1.0f
    
    private readonly float m_TipMaxHeight = 1.5f;  //���� �� ���� ���� 0.2f
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
    /// �� ��ġ
    /// </summary>
    public Vector3 TipPos { get; private set; }

    public Vector3 TipUpDir { get; private set; }

    public Vector3 RaycastTipPos { get; private set; }

    /// <summary>
    /// ���� ���� �� �ִ� ������ �� ��ġ
    /// </summary>
    public Vector3 RaycastTipNormal { get; private set; }

    /// <summary>
    /// �ش� �� ������Ʈ�� ���� ����
    /// </summary>
    public bool Animating { get; private set; } = false;

    public bool Movable { get; set; } = false;

    /// <summary>
    /// �ڱ� �� ��ġ�� ray���� �ɸ� �� ��ġ�� �Ÿ�
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
            StartCoroutine(AnimateLeg()); //�������� �ٸ�(��) �ִϸ�����(������)
    }

    private IEnumerator AnimateLeg()
    {
        Animating = true;

        float timer = 0.0f;
        float animTime, tipAcceleration;

        Vector3 startingTipPos = TipPos;
        Vector3 tipDirVec = RaycastTipPos - TipPos;
        tipDirVec += tipDirVec.normalized * m_TipPassOver;
        //�߿��� �������

        Vector3 right = Vector3.Cross(m_BodyTransform.up, tipDirVec.normalized).normalized;
        TipUpDir = Vector3.Cross(tipDirVec.normalized, right);
        //tipUpVec : ������⿡���� �߿��� �� ����
        
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
