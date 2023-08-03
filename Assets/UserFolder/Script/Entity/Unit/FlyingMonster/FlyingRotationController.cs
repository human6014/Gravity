using System;
using UnityEngine;

namespace Entity.Unit.Flying
{
	public class FlyingRotationController : MonoBehaviour
	{
		[SerializeField, Header("Animated Rotation")] private float rotationEasing = 1f;

		private Transform m_PlayerHead;
		private Rigidbody m_Rigidbody;
		private FlyingMovementController m_FlyingMovementController;

		private bool m_IsAlive;
		public Vector3 LookAtDir
		{
			get
			{
				if (m_FlyingMovementController.CloseToTarget)
					return m_FlyingMovementController.CurrentTargetPosition - m_Rigidbody.position;
				return m_PlayerHead.position - m_Rigidbody.position;
			}
		}

		private void Awake()
		{
			m_Rigidbody = GetComponent<Rigidbody>();
			m_FlyingMovementController = GetComponent<FlyingMovementController>();
			m_PlayerHead = Manager.AI.AIManager.PlayerTransform;
		}

		public void Init()
		{
			m_IsAlive = true;
		}

		public void Dispose()
		{
			m_IsAlive = false;
		}

		public void LookCurrentTarget()
		{
			if (!m_IsAlive) return;
			Quaternion lookRot = Quaternion.LookRotation(LookAtDir, -Manager.GravityManager.GravityVector);
			m_Rigidbody.MoveRotation(Quaternion.Lerp(m_Rigidbody.rotation, lookRot, Time.deltaTime * rotationEasing));
		}
	}
}