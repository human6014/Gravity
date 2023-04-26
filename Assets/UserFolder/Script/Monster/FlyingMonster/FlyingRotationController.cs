using System;
using UnityEngine;

public class FlyingRotationController : MonoBehaviour
{
	[SerializeField, Header("Animated Rotation")] private float rotationEasing = 1f;

	private Transform playerHead;
	private Rigidbody cachedRigidbody;
	private FlyingMovementController movementController;

	private bool isRun;
	public Vector3 LookAtDir
	{
		get
		{
			if (movementController.CloseToTarget)
				return movementController.CurrentTargetPosition - cachedRigidbody.position;
			return playerHead.position - cachedRigidbody.position;
		}
	}

	private void Awake()
	{
		cachedRigidbody = GetComponent<Rigidbody>();
		movementController = GetComponent<FlyingMovementController>();
		playerHead = Manager.AI.AIManager.PlayerTransform;
	}

	public void Init()
    {
		isRun = true;
	}

	public void Dispose()
	{
		isRun = false;
	}

	public void LookCurrentTarget()
    {
		if (!isRun) return;
		Quaternion lookRot = Quaternion.LookRotation(LookAtDir, -Manager.GravityManager.GravityVector);
		cachedRigidbody.MoveRotation(Quaternion.Lerp(cachedRigidbody.rotation, lookRot, Time.deltaTime * rotationEasing));
	}
}