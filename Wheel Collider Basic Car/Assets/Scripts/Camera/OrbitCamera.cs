using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour {

	public Transform target;
	public bool isAutoUpdated = true;
	public bool avoidObstacleClipping = true;
	public LayerMask cameraObstacleLayers;

	public float distance = 30.0f;
	public float smoothness = 0.2f;

	public float minVerticalAngle = 0.0f;
	public float maxVerticalAngle = 89.0f;
	public float minHorizontalAngle = 0.0f;
	public float maxHorizontalAngle = 360.0f;

	[Range(0, 360)]
	public float verticalAngle = 0;
	[Range(0, 360)]
	public float horizontalAngle = 0;

	private Vector3 currentVelocity = Vector3.zero;

	// Use this for initialization
	void Start () {
		if (target == null) {
			Debug.LogWarning ("Camera target undefined. Camera won't update");
		}
	}

	void LateUpdate () {
		if (isAutoUpdated) {
			SmoothCameraUpdate ();
		}
	}

	/// <summary>
	/// Smooths the camera to defined orbit position around target.
	/// </summary>
	public void SmoothCameraUpdate () {
		if (target == null) {
			return;
		}

		horizontalAngle = Mathf.Clamp (horizontalAngle, minHorizontalAngle, maxHorizontalAngle);
		verticalAngle = Mathf.Clamp (verticalAngle, minVerticalAngle, maxVerticalAngle);

		Vector3 desiredPosition = CalculateOrbitPosition (target.position, distance, horizontalAngle, verticalAngle);

		transform.position = Vector3.SmoothDamp (transform.position, desiredPosition, ref currentVelocity, smoothness);
		transform.rotation = LookAtTarget (target.position);
	}

	/// <summary>
	/// Sets the camera orbit position around target immediately.
	/// </summary>
	/// <param name="distance">Distance from orbit center.</param>
	/// <param name="horizontalAngle">Angle offset from world Z axis.</param>
	/// <param name="verticalAngle">Angle offset from ground plane.</param>
	public void SetCameraOrbitPosition (float distance, float horizontalAngle, float verticalAngle) {
		if (target == null) {
			return;
		}

		horizontalAngle = Mathf.Clamp (horizontalAngle, minHorizontalAngle, maxHorizontalAngle);
		verticalAngle = Mathf.Clamp (verticalAngle, minVerticalAngle, maxVerticalAngle);

		transform.position = CalculateOrbitPosition (target.position, distance, horizontalAngle, verticalAngle);
		transform.rotation = LookAtTarget (target.position);
	}

	public void ChangeHorizontalAngle (float angle) {
		horizontalAngle += angle;

		if (horizontalAngle > 360) {
			horizontalAngle -= 360;
		}

		if (horizontalAngle < 0) {
			horizontalAngle += 360;
		}
	}

	public void ChangeVerticalAngle (float angle) {
		verticalAngle += angle;

		if (verticalAngle > 360) {
			verticalAngle -= 360;
		}

		if (horizontalAngle < 0) {
			verticalAngle += 360;
		}
	}

	/// <summary>
	/// Returns a rotation that points to target.
	/// </summary>
	/// <returns>Rotation ponting to target.</returns>
	/// <param name="target">Target to look at</param>
	private Quaternion LookAtTarget (Vector3 target) {
		Vector3 relativePosition = target - transform.position;
		Quaternion rotation = Quaternion.LookRotation (relativePosition);
		return rotation;
	}

	/// <summary>
	/// Returns a position in a specified orbit around the center position.
	/// </summary>
	/// <param name="distance">Distance from orbit center.</param>
	/// <param name="horizontalAngle">Angle offset from world Z axis.</param>
	/// <param name="verticalAngle">Angle offset from ground plane.</param>
	private Vector3 CalculateOrbitPosition (Vector3 center, float distance, float horizontalAngle, float verticalAngle) {

		Vector3 direction = Vector3.back;

		direction = Quaternion.AngleAxis (horizontalAngle, Vector3.up) * direction;

		Vector3 verticalRotationAxis = Vector3.Cross (direction, Vector3.up);
		direction = Quaternion.AngleAxis (verticalAngle, verticalRotationAxis) * direction;

		if (avoidObstacleClipping) {
			RaycastHit hit;

			if (Physics.Raycast (center, direction, out hit, distance, cameraObstacleLayers)) {
				distance = hit.distance - 1;
			}
		}

		return center + (direction * distance);
	}
}
