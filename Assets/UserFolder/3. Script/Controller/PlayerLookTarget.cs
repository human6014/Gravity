using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.AI
{
	public class PlayerLookTarget : MonoBehaviour
	{

		[SerializeField] private Transform supportTarget;
		[SerializeField] private LayerMask pointLayerMask = -1;
		[SerializeField] private float maxPointDistance = 8;
		[SerializeField] private float distnaceFromSurface = 1;
		[SerializeField] private float velocityDirectionContribution = 0.5f;
		[SerializeField] private float maxDistanceToTrigger = 4;

		private Vector3 m_LastPos;
		private Rigidbody m_PlayerRigidBody;
		private float m_LastDistanceTravelled;
		private float m_DistanceTravelled;
		private bool m_IsWalking;
		private int m_RandomDir;

		private void Awake()
		{
			m_PlayerRigidBody = GetComponent<Rigidbody>();
			Manager.AI.AIManager.PlayerSupportTargetTransform = supportTarget;
		}

		private void Update()
		{
			m_DistanceTravelled += (transform.position - m_LastPos).magnitude;

			if (Mathf.Abs(m_DistanceTravelled - m_LastDistanceTravelled) > maxDistanceToTrigger)
			{
				if (!m_IsWalking)
				{
					m_RandomDir = (int)Mathf.Sign(Random.Range(-1f, 1f));
					m_IsWalking = true;
				}

				m_LastDistanceTravelled = m_DistanceTravelled;
				Vector3 velocity = Vector3.Normalize(transform.TransformVector(m_PlayerRigidBody.velocity * m_RandomDir).normalized * velocityDirectionContribution + transform.forward);
				if (Physics.Raycast(transform.position, velocity, out RaycastHit hit, maxPointDistance, pointLayerMask))
					supportTarget.position = hit.point + hit.normal * distnaceFromSurface + transform.up * 3;
				else
					supportTarget.position = transform.position + velocity * maxPointDistance + transform.up * 3;
			}
			else m_IsWalking = false;


			m_LastPos = transform.position;
		}
	}
}
