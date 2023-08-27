using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LegController : MonoBehaviour
{
    [Header("")]
    [SerializeField] private SpecialMonsterAI m_SpecialMonsterAI;
    [SerializeField] private Transform m_BodyTransform; //Body 위치
    [SerializeField] private Leg[] legs;

    [Header("Options")]
    [SerializeField] private MultiAimConstraint[] m_MultiAimConstraint;

    private bool m_IsAlive = true;
    private bool preJump = false;
    private bool isJump = false;
    private bool readySwitchOrder = false;
    private bool stepOrder = true;

    private readonly float maxTipWait = 1.5f; //값이 작으면 발 위치 고정 시간이 길어짐 0.7f
    private readonly float bodyHeightBase = 0;   //body 높이 1.3f
    private readonly float posAdjustRatio = 0.05f;  //body 위치 조정 강도

    public void SetPreJump(bool _preJump) => preJump = _preJump;

    private void Start()
    {
        SetAimConstraint();
        StartCoroutine(AdjustBodyTransform());
    }

    private void SetAimConstraint()
    {
        WeightedTransform weightedTransform = new(Manager.AI.AIManager.PlayerTransform, 1);
        WeightedTransformArray weightedTransforms = new() { weightedTransform };
        foreach (MultiAimConstraint m in m_MultiAimConstraint) m.data.sourceObjects = weightedTransforms;
    }

    private void ClearAimConstraint()
    {
        foreach (MultiAimConstraint m in m_MultiAimConstraint)
            m.data.sourceObjects = new WeightedTransformArray();
    }

    private void FixedUpdate()
    {
        if (!m_IsAlive) return;
        // If tip is not in current order but it's too far from target position, Switch the order
        for (int i = 0; i < legs.Length; i++)
        {
            if (legs[i].TipDistance > maxTipWait)
            {
                stepOrder = i % 2 == 0;
                break;
            }
        }

        // Ordering steps
        foreach (Leg leg in legs)
        {
            leg.Movable = stepOrder;
            stepOrder = !stepOrder;
        }

        int index = stepOrder ? 0 : 1;

        // If the opposite foot step completes, switch the order to make a new step
        if (readySwitchOrder && !legs[index].Animating)
        {
            stepOrder = !stepOrder;
            readySwitchOrder = false;
        }

        if (!readySwitchOrder && legs[index].Animating) readySwitchOrder = true;
    }

    Quaternion bodyRotation;
    /// <summary>
    /// 다리에 대한 몸체 위치, 각도 계산 (Couroutine)
    /// </summary>
    /// <returns></returns>
    private IEnumerator AdjustBodyTransform()
    {
        Vector3 tipCenter;
        Vector3 bodyPos;
        Vector3 bodyUp;
        Vector3 bodyForward;
        Vector3 bodyRight;
        while (true)
        {
            if (!isJump && !preJump)
            {
                tipCenter = Vector3.zero;
                bodyUp = Vector3.zero;

                // Collect leg information to calculate body transform
                foreach (Leg leg in legs)
                {
                    tipCenter += leg.TipPos;
                    bodyUp += leg.TipUpDir + leg.RaycastTipNormal;
                }

                if (Physics.Raycast(m_BodyTransform.position + m_BodyTransform.up * 3, m_BodyTransform.up * -1, out RaycastHit hit, 30.0f))
                    bodyUp += hit.normal;

                bodyUp.Normalize();

                // calc transform
                // Interpolate postition from old to new

                tipCenter /= legs.Length;

                bodyPos = tipCenter + bodyUp * bodyHeightBase;
                m_SpecialMonsterAI.ProceduralPosition = Vector3.Lerp(m_BodyTransform.position, bodyPos, posAdjustRatio);
                //bodyTransform.position = Vector3.Lerp(bodyTransform.position, bodyPos, posAdjustRatio);


                // calc rotation
                // Calculate new body axis
                bodyRight = Vector3.Cross(bodyUp, m_BodyTransform.forward);
                bodyForward = Vector3.Cross(bodyRight, bodyUp).normalized;

                // Interpolate rotation from old to new
                bodyRotation = Quaternion.LookRotation(bodyForward, bodyUp);
                m_SpecialMonsterAI.ProceduralForwardAngle = bodyForward;
                m_SpecialMonsterAI.ProceduralUpAngle = bodyUp;
                //bodyTransform.rotation = Quaternion.Slerp(bodyTransform.rotation, bodyRotation, rotAdjustRatio);
            }
            else m_SpecialMonsterAI.ProceduralPosition = m_BodyTransform.position;
            yield return null;
        }
    }

    /// <summary>
    /// 점프 입력시 this + children setter
    /// </summary>
    /// <param name="_isJump">점프 == true, 점프 안함 == false</param>
    public void Jump(bool _isJump)
    {
        isJump = _isJump;
        foreach (Leg leg in legs)
            leg.SetIsJump(isJump);
    }

    public void Dispose()
    {
        ClearAimConstraint();
        m_IsAlive = false;
        StopAllCoroutines();
    }
}
