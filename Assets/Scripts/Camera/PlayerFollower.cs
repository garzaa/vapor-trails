using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour {

	[HideInInspector]
	public GameObject player;
	[HideInInspector]
	public PlayerController pc;
	public float smoothAmount;
	public Vector2 initialOffset;
	Vector2 currentOffset;
	Vector3 velocity = Vector3.zero;

	public bool followX = true;
	public bool followY = true;

	bool smoothing = true;
	bool following = true;

	bool playerWasNull = false;

	public GameObject target;

	CameraOffset cameraOffset;

	void Start() {
		if (player == null) {
			player = GameObject.Find("Player");
			playerWasNull = true;
		}
		this.transform.position = new Vector3(
			x:followX ? player.transform.position.x + currentOffset.x : transform.position.x,
			y:followY ? player.transform.position.y + currentOffset.y : transform.position.y,
			z:this.transform.position.z
		);
		pc = player.GetComponent<PlayerController>();
		this.target = player;
		cameraOffset = GetComponentInChildren<CameraOffset>();
	}
	
	void FixedUpdate() {
		if (!following) {
			return;
		}

		if (target == null && !playerWasNull) {
			FollowPlayer();
		}

		if (smoothing) {
			transform.position = Vector3.SmoothDamp(
				transform.position,
				new Vector3(
					x:followX ? target.transform.position.x + currentOffset.x : transform.position.x,
					y:followY ? target.transform.position.y + currentOffset.y : transform.position.y,
					z:this.transform.position.z),
				ref velocity,
				smoothAmount * Time.deltaTime
			);
		} else {
			transform.position = new Vector3(
					x:followX ? target.transform.position.x + currentOffset.x : transform.position.x,
					y:followY ? target.transform.position.y + currentOffset.y : transform.position.y,
					z:this.transform.position.z);
			transform.position += (Vector3) currentOffset;
		}	
	}

	public void SnapToTarget() {
		DisableSmoothing();
		velocity = Vector3.zero;
		this.transform.position = target.transform.position;
		EnableSmoothing();
	}

	public void EnableSmoothing() {
		this.smoothing = true;
	}

	public void DisableSmoothing() {
		this.smoothing = false;
	}

	public void EnableFollowing() {
		this.following = true;
		cameraOffset.following = true;
	}

	public void DisableFollowing() {
		this.following = false;
		cameraOffset.following = false;
	}

	public void FollowTarget(GameObject target) {
		this.target = target;
		cameraOffset.following = false;
	}

	public void FollowPlayer() {
		if (cameraOffset != null) {
			cameraOffset.following = true;
		}
		this.target = player;
	}

	public void UpdateOffset(Vector2 newOffset) {
		this.currentOffset = newOffset;
	}

	public void ResetOffset() {
		this.currentOffset = initialOffset;
	}

	// use the camera offset subobject because it's smoother
	public void LookAtPoint(GameObject point) {
		cameraOffset.LookAtTarget(point);
	}

	public void StopLookingAtPoint() {
		cameraOffset.StopLookingAtTarget();
	}
}
