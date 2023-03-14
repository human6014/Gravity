using System;
using UnityEngine;

public class FlyingRotationController : MonoBehaviour
{
	private readonly VectorPid angularVelocityController = new VectorPid(33.7766f, 0, 0.2553191f);
	[SerializeField] private VectorPid headingController = new VectorPid(9.244681f, 0, 0.06382979f);

	[SerializeField, Header("Animated Rotation")] private float rotationEasing = 1f;

	private Transform playerHead;
	private Rigidbody cachedRigidbody;
	private FlyingMovementController movementController;

	public Vector3 LookAtDir
	{
		get
		{
			if (movementController.CloseToTarget)
				return movementController.CurrentTargetPosition - cachedRigidbody.position;
			return playerHead.position - cachedRigidbody.position;
		}
	}

	void Awake()
	{
		cachedRigidbody = GetComponent<Rigidbody>();
		movementController = GetComponent<FlyingMovementController>();
		playerHead = Manager.AI.AIManager.PlayerTransfrom;
	}

	public void LookCurrentTarget()
    {
		Quaternion lookRot = Quaternion.LookRotation(LookAtDir, -Manager.GravityManager.GravityVector);
		cachedRigidbody.MoveRotation(Quaternion.Lerp(cachedRigidbody.rotation, lookRot, Time.deltaTime * rotationEasing));
	}

	/*
	private void FixedUpdate()
	{
		if (physicsRotation)
		{
			Vector3 angularVelocityError = cachedRigidbody.angularVelocity * -1f;
			Vector3 angularVelocityCorrection = angularVelocityController.Update(angularVelocityError, Time.deltaTime);
			cachedRigidbody.AddTorque(angularVelocityCorrection, ForceMode.Acceleration);

			//forward heading correction
			Vector3 desiredHeading = LookAtDir;
			Vector3 currentHeading = transform.forward;
			Vector3 headingError = Vector3.Cross(currentHeading, desiredHeading);
			Vector3 headingCorrection = headingController.Update(headingError, Time.deltaTime);
			cachedRigidbody.AddTorque(headingCorrection, ForceMode.Acceleration);

			//up heading correction
			desiredHeading = Vector3.up - transform.up;
			currentHeading = transform.up;
			headingError = Vector3.Cross(currentHeading, desiredHeading);
			headingCorrection = headingController.Update(headingError, Time.deltaTime);
			cachedRigidbody.AddTorque(headingCorrection, ForceMode.Acceleration);
		}
		
	}
	*/

	[Serializable]
	public class VectorPid
	{
		public float pFactor, iFactor, dFactor;

		private Vector3 integral;
		private Vector3 lastError;

		public VectorPid(float pFactor, float iFactor, float dFactor)
		{
			this.pFactor = pFactor;
			this.iFactor = iFactor;
			this.dFactor = dFactor;
		}

		public Vector3 Update(Vector3 currentError, float timeFrame)
		{
			integral += currentError * timeFrame;
			var deriv = (currentError - lastError) / timeFrame;
			lastError = currentError;
			return currentError * pFactor + integral * iFactor + deriv * dFactor;
		}
	}
}