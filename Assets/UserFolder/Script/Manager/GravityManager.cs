using UnityEngine;
using System.Collections.Generic;
using System;
using EnumType;
using System.Collections;

namespace Manager
{
    public class GravityManager : MonoBehaviour
    {
        [SerializeField] private PlayerInputController m_PlayerInputController;

        public static GravityType currentGravityType = GravityType.yDown;
        public static GravityType beforeGravityType = GravityType.yDown;
        public static GravityDirection gravityDirection = GravityDirection.Y;

        private const float ROTATETIME = 1;

        public static Action <GravityType> GravityChangeAction { get; set; }

        /// <summary>
        /// �߷� ���� ���콺 ��ũ�� �Ʒ�  : -1 , �� : 1
        /// </summary>
        public static float GravityDirectionValue { get; private set; } = -1;

        /// <summary>
        /// �߷� �� ����� �÷��̾� ȸ�� ���� �� true, �÷��̾� ȸ�� ������ false
        /// </summary>
        public static bool IsGravityChanging { get; private set; } = false;

        /// <summary>
        /// �߷� �� ����� ��ĥ��� true
        /// </summary>
        public static bool IsGravityDupleicated { get; private set; } = false;

        /// <summary>
        /// �߷� ���� �Ϲ�ȭ
        /// </summary>
        public static Vector3 GravityVector { get; private set; } = Vector3.down;

        /// <summary>
        /// ���� �߷¿� �ش��ϴ� Area�� normal ����
        /// </summary>
        private static readonly Vector3Int[] gravityRotation =
        {
            new Vector3Int(0, 0, -90)   , new Vector3Int(0, 0, 90),
            new Vector3Int(0, 0, 0)     , new Vector3Int(180, 0, 0),
            new Vector3Int(90, 0, 0)   , new Vector3Int(-90, 0, 0),
        };

        private void Awake()
        {
            m_PlayerInputController.DoGravityChange += GravityChange;
        }

        /// <summary>
        /// �߷� ����, �߷��� ���ϴ� ������ �����
        /// </summary>
        /// <param name="direct"></param>
        private void GravityChange(int direct)
        {
            beforeGravityType = currentGravityType;
            GravityDirectionValue = direct;
            switch (gravityDirection)
            {
                case GravityDirection.X:
                    currentGravityType = direct < 0 ? GravityType.xDown : GravityType.xUp;
                    GravityVector = new Vector3(direct, 0, 0);
                    break;
                case GravityDirection.Y:
                    currentGravityType = direct < 0 ? GravityType.yDown : GravityType.yUp;
                    GravityVector = new Vector3(0, direct, 0);
                    break;
                case GravityDirection.Z:
                    currentGravityType = direct < 0 ? GravityType.zDown : GravityType.zUp;
                    GravityVector = new Vector3(0, 0, direct);
                    break;
            }
            if (beforeGravityType == currentGravityType) IsGravityDupleicated = true;
            else
            {
                GravityChangeAction(currentGravityType);
                Physics.gravity = GravityVector * Physics.gravity.magnitude;

                IsGravityChanging = true;
                IsGravityDupleicated = false;
            }
        }

        public static void CompleteGravityChanging() => IsGravityChanging = false;

        public static Vector3Int GetCurrentGravityNoramlDirection() => gravityRotation[(int)currentGravityType];

        public static Quaternion GetSpecificGravityNormalRotation(int index) => Quaternion.Euler(gravityRotation[index]);
        
        public static Quaternion GetCurrentGravityNormalRotation() => Quaternion.Euler(gravityRotation[(int)currentGravityType]);

        public void GravityChange(int gravityKeyInput, float mouseScroll)
        {
            if (IsGravityChanging) return;

            gravityDirection = (GravityDirection)gravityKeyInput;
            GravityChange(Mathf.FloorToInt(mouseScroll * 10));
            StartCoroutine(GravityRotate());
        }

        private IEnumerator GravityRotate()
        {
            Quaternion currentRotation = transform.rotation;
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / ROTATETIME;
                transform.rotation = Quaternion.Lerp(currentRotation, GravityManager.GetCurrentGravityNormalRotation(), t);
                yield return null;
            }
            CompleteGravityChanging();
        }
    }
}