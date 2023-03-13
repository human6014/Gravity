using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookTarget : MonoBehaviour {

	[SerializeField] private Transform target;
	[SerializeField] private LayerMask pointLayerMask = -1;
	[SerializeField] private float maxPointDistance = 8;
	[SerializeField] private float distnaceFromSurface = 1;
	[SerializeField] private float velocityDirectionContribution = 0.5f;
	[SerializeField] private float maxDistanceToTrigger = 4;

	private Vector3 lastPos;
	private CharacterController characterController;
	private float lastDistanceTravelled;
	private float distanceTravelled;
	private bool isWalking;
	private int randomDir;

	private void Awake ()
	{
		characterController = GetComponent<CharacterController>();
	}

	private void Update ()
	{
		distanceTravelled += (transform.position - lastPos).magnitude;

		if (Mathf.Abs(distanceTravelled - lastDistanceTravelled) > maxDistanceToTrigger)
		{
			if (!isWalking)
			{
				randomDir = (int)Mathf.Sign(Random.Range(-1f, 1f));
				isWalking = true;
			}

			lastDistanceTravelled = distanceTravelled;
			Vector3 velocity = Vector3.Normalize(transform.TransformVector(characterController.velocity * randomDir).normalized * velocityDirectionContribution + transform.forward);
			if (Physics.Raycast(transform.position, velocity, out RaycastHit hit, maxPointDistance, pointLayerMask))
                target.position = hit.point + hit.normal * distnaceFromSurface + transform.up * 3;
            else
                target.position = transform.position + velocity * maxPointDistance + transform.up * 3;
		}
		else isWalking = false;
		

		lastPos = transform.position;
	}
}
