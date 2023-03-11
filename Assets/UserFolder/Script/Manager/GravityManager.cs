using UnityEngine;
using System.Collections.Generic;
using System;
using EnumType;

namespace Manager
{
    public class GravityManager
    {
        public static GravityType currentGravityType = GravityType.yUp;
        public static GravityType beforeGravityType = GravityType.yUp;
        public static GravityDirection gravityDirection = GravityDirection.Y;

        /// <summary>
        /// �߷� ���� ���콺 ��ũ�� �Ʒ�  : -1 , �� : 1
        /// </summary>
        public static float GravityDirectionValue { get; private set; } = -1;

        /// <summary>
        /// �߷� �� ����� true, �÷��̾� ȸ�� ������ false
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
        /// �߷� ����, �߷��� ���ϴ� ������ �ƴ϶� �ݴ���
        /// </summary>
        /// <param name="direct"></param>
        public static void GravityChange(int direct)
        {
            beforeGravityType = currentGravityType;
            GravityDirectionValue = direct;
            switch (gravityDirection)
            {
                case GravityDirection.X:
                    currentGravityType = direct < 0 ? GravityType.xUp : GravityType.xDown;
                    GravityVector = new Vector3(direct, 0, 0);
                    break;
                case GravityDirection.Y:
                    currentGravityType = direct < 0 ? GravityType.yUp : GravityType.yDown;
                    GravityVector = new Vector3(0, direct, 0);
                    break;
                case GravityDirection.Z:
                    currentGravityType = direct < 0 ? GravityType.zUp : GravityType.zDown;
                    GravityVector = new Vector3(0, 0, direct);
                    break;
            }
            if (beforeGravityType == currentGravityType) IsGravityDupleicated = true;
            else
            {
                Physics.gravity = GravityVector * Physics.gravity.magnitude;
                
                IsGravityChanging = true;
                IsGravityDupleicated = false;
            }
        }

        public static void CompleteGravityChanging() => IsGravityChanging = false;
        
    }
}