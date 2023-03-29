using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using EnumType;

namespace Entity.Unit.Special
{
    public class SpecialMonster1 : MonoBehaviour
    {
        // Parabola
        [Header("Parabola")]
        [SerializeField] private float _step = 0.01f;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LayerMask layerMask;
        //[SerializeField] float height = 10;

        [Header("Code")]
        [SerializeField] private Transform navObject;
        [SerializeField] private LegController legController;


        private Transform target;
        [SerializeField] private Transform aiTransform;
        private SpecialMonsterAI specialMonsterAI;

        private Vector3 groundDirection = Vector3.zero;
        private Vector3 targetPos = Vector3.zero;
        private Vector3 direction;

        bool isPreJump = false;
        public Quaternion GetRotation() => transform.rotation;
        public Vector3 GetPosition() => transform.position;

        private void Awake()
        {
            Debug.Log("Awake");
            //aiTransform = GetComponent<Transform>();
            Debug.Log(aiTransform.name);
            specialMonsterAI = aiTransform.GetComponent<SpecialMonsterAI>();
            Debug.Log(specialMonsterAI.name);
        }

        public void Init(Quaternion rotation)
        {
            Debug.Log("Init");
            target = Manager.AI.AIManager.PlayerTransfrom;
            specialMonsterAI.Init(rotation);
        }

        private void FixedUpdate()
        {
            specialMonsterAI.OperateAIBehavior();
            //UpdateCollider();
        }
        void Update()
        {
            if (!target) return;

            direction = target.position - aiTransform.position;
            float dir = 0;
            switch (GravityManager.currentGravityType)
            {
                case GravityType.xUp:
                case GravityType.xDown:
                    groundDirection = new(0, direction.y, direction.z);
                    dir = direction.x;
                    break;

                case GravityType.yUp: //Init
                case GravityType.yDown:
                    groundDirection = new(direction.x, 0, direction.z);
                    dir = direction.y;
                    break;

                case GravityType.zUp:
                case GravityType.zDown:
                    groundDirection = new(direction.x, direction.y, 0);
                    dir = direction.z;
                    break;
            }
            targetPos = new(groundDirection.magnitude, dir * -GravityManager.GravityDirectionValue, 0);
            float height = Mathf.Max(0.01f, targetPos.y + targetPos.magnitude / 2f);

            CalculatePathWithHeight(targetPos, height, out float v0, out float angle, out float time);
            t_v0 = v0;
            t_angle = angle;
            t_time = time;
            //DrawPath(groundDirection.normalized, v0, angle, time, _step); //경로 그리기

            if (Input.GetKeyDown(KeyCode.Backspace) && !isPreJump && !specialMonsterAI.GetIsOnOffMeshLink())
                StartCoroutine(Coroutine_Movement(groundDirection.normalized, v0, angle, time));
        }


        float t_v0;
        float t_angle;
        float t_time;
        [ContextMenu("ExcuteJump")]
        public void ExcuteJump()
        {
            if (!isPreJump && !specialMonsterAI.GetIsOnOffMeshLink())
            {
                StartCoroutine(Coroutine_Movement(groundDirection.normalized, t_v0, t_angle, t_time));
            }
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
                lineRenderer.SetPosition(count, aiTransform.position + direction * x - GravityManager.GravityVector * y);

                count++;
            }
            float xFinal = v0 * time * Mathf.Cos(angle);
            float yFinal = v0 * time * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(time, 2);
            lineRenderer.SetPosition(count, aiTransform.position + direction * xFinal - GravityManager.GravityVector * yFinal);
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
            Vector3 startPos = aiTransform.position;
            while (elapsedTime < beforeJumpingTime)
            {
                aiTransform.position = Vector3.Lerp(startPos, startPos - aiTransform.up, elapsedTime / beforeJumpingTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            legController.SetPreJump(false);
            legController.Jump(true);
            Debug.Log("Coroutine target : " + target);
            elapsedTime = 0;
            startPos = aiTransform.position;
            Quaternion startRotation = aiTransform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(groundDirection, target.up);
            Debug.Log("time : " + time);
            while (elapsedTime < time)
            {
                float x = v0 * elapsedTime * Mathf.Cos(angle);
                float y = v0 * elapsedTime * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(elapsedTime, 2);
                aiTransform.SetPositionAndRotation(startPos + direction * x - GravityManager.GravityVector * y,
                                 Quaternion.Lerp(startRotation, targetRot, elapsedTime / time));
                //elapsedTime += Time.deltaTime * (time / 1.5f);
                //elapsedTime += Time.deltaTime;
                elapsedTime += Time.deltaTime * 2.5f;
                yield return null;
            }
            legController.Jump(false);
            isPreJump = false;
            specialMonsterAI.SetNavMeshEnable(true);
            //specialMonsterAI.SetNavMeshPos(transform.position);
        }
        #endregion
    }
}
