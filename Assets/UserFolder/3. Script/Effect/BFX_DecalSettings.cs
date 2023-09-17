
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

//[ExecuteAlways]
public class BFX_DecalSettings : MonoBehaviour
{
    public BFX_BloodSettings BloodSettings;
    public Transform parent;
    public float TimeHeightMax = 3.1f;
    public float TimeHeightMin = -0.1f;

    [Space]
    public Vector3 TimeScaleMax = Vector3.one;
    public Vector3 TimeScaleMin = Vector3.one;

    [Space]
    public Vector3 TimeOffsetMax = Vector3.zero;
    public Vector3 TimeOffsetMin = Vector3.zero;

    [Space]
    public AnimationCurve TimeByHeight = AnimationCurve.Linear(0, 0, 1, 1);

    private BFX_ShaderProperies shaderProperies;
    private DecalProjector decal;

    private Vector3 initializedPosition;
    private Vector3 averageRay;
    private Vector3 startOffset;
    private Vector3 startScale;

    private float timeDelay;
    private bool isPositionInitialized;

    private void Awake()
    {
        decal = GetComponent<DecalProjector>();
        startOffset = transform.localPosition;
        startScale = transform.localScale;
        shaderProperies = GetComponent<BFX_ShaderProperies>();
        shaderProperies.OnAnimationFinished += ShaderCurve_OnAnimationFinished;
    }

    private void ShaderCurve_OnAnimationFinished()
    {
        decal.enabled = false;
    }

    private void Update()
    {
        if (!isPositionInitialized) InitializePosition();
        //if (shaderProperies.enabled && initializedPosition.x < float.PositiveInfinity) transform.position = initializedPosition;
    }

    private float ApplyToGravity(Vector3 vector)
    {
        float value = 0;
        switch (Manager.GravityManager.CurrentGravityAxis)
        {
            case Manager.GravityDirection.X:
                value = vector.x;
                break;
            case Manager.GravityDirection.Y:
                value = vector.y;
                break;
            case Manager.GravityDirection.Z:
                value = vector.z;
                break;
        }
        return value;
    }

    private void InitializePosition()
    {
        decal.enabled = true;

        var currentHeight = ApplyToGravity(parent.position);
        float ground = currentHeight;
        if (BloodSettings.AutomaticGroundHeightDetection)
        {
            var raycasts = Physics.RaycastAll(parent.position, Manager.GravityManager.GravityVector, 5);
            foreach (var raycastHit in raycasts)
            {
                if(Manager.GravityManager.GravityDirectionValue == -1 && ApplyToGravity(raycastHit.point) < ground)
                    ground = ApplyToGravity(raycastHit.point);
                else if (Manager.GravityManager.GravityDirectionValue == 1 && ApplyToGravity(raycastHit.point) > ground)
                    ground = ApplyToGravity(raycastHit.point);
            }
        }
        else ground = BloodSettings.GroundHeight;
        

        var currentScale = parent.localScale;
        var scaledTimeHeightMax = TimeHeightMax * currentScale.y;
        var scaledTimeHeightMin = TimeHeightMin * currentScale.y;

        //if (currentHeight - ground >= scaledTimeHeightMax || currentHeight - ground <= scaledTimeHeightMin)
        //    decal.enabled = false;
        //else
        //    decal.enabled = true;

        float diff = (ApplyToGravity(parent.position) - ground) / scaledTimeHeightMax;
        diff = Mathf.Abs(diff);

        var scaleMul = Vector3.Lerp(TimeScaleMin, TimeScaleMax, diff);
        scaleMul.x *= currentScale.x;
        scaleMul.z *= currentScale.z;
        decal.size = new Vector3(scaleMul.x * startScale.x, scaleMul.z * startScale.z, startScale.y);

        var lastOffset = Vector3.Lerp(TimeOffsetMin, TimeOffsetMax, diff);
        transform.localPosition = startOffset + lastOffset;

        switch (Manager.GravityManager.CurrentGravityAxis)
        {
            case Manager.GravityDirection.X:
                transform.position = new Vector3(ground + 0.05f, transform.position.y, transform.position.z);
                break;
            case Manager.GravityDirection.Y:
                transform.position = new Vector3(transform.position.x, ground + 0.05f, transform.position.z);
                break;
            case Manager.GravityDirection.Z:
                transform.position = new Vector3(transform.position.x, transform.position.y, ground + 0.05f);
                break;
        }

        timeDelay = TimeByHeight.Evaluate(diff);

        shaderProperies.enabled = false;
        Invoke(nameof(EnableDecalAnimation), Mathf.Max(0, timeDelay / BloodSettings.AnimationSpeed));

        //if (BloodSettings.ClampDecalSideSurface) Shader.EnableKeyword("CLAMP_SIDE_SURFACE");

        isPositionInitialized = true;
    }

    private void OnDisable()
    {
        //if (BloodSettings.ClampDecalSideSurface) Shader.DisableKeyword("CLAMP_SIDE_SURFACE");
        isPositionInitialized = false;
        initializedPosition = Vector3.positiveInfinity;
    }

    private void EnableDecalAnimation()
    {
        shaderProperies.enabled = true;
        initializedPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(49 / 255.0f, 136 / 255.0f, 1, 0.03f);
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.color = new Color(49 / 255.0f, 136 / 255.0f, 1, 0.85f);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}
