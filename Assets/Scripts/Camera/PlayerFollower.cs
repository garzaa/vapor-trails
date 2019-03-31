using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour {

	[HideInInspector]
	public GameObject player;
	[HideInInspector]
	public PlayerController pc;
	public float smoothAmount;
	public float lookAhead;
	public Vector2 initialOffset;
	Vector2 currentOffset;
	Vector3 velocity = Vector3.zero;

	public bool followX = true;
	public bool followY = true;

	bool smoothing = true;
	bool following = true;

	bool playerWasNull = false;

	public GameObject target;

	void Start () {
		if (player == null) {
			player = GameObject.Find("Player");
			playerWasNull = true;
		}
		this.transform.position = new Vector3(
			x:player.transform.position.x + currentOffset.x,
			y:player.transform.position.y + currentOffset.y,
			z:this.transform.position.z
		);
		pc = player.GetComponent<PlayerController>();
		this.target = player;
		smoothing = (smoothAmount != 0);
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

	public void SnapToPlayer() {
		this.transform.position = player.transform.position;
	}

	public void EnableSmoothing() {
		this.smoothing = true;
	}

	public void DisableSmoothing() {
		this.smoothing = false;
	}

	public void EnableFollowing() {
		this.following = true;
		GetComponentInChildren<CameraOffset>().following = true;
	}

	public void DisableFollowing() {
		this.following = false;
		GetComponentInChildren<CameraOffset>().following = false;
	}

	public void FollowTarget(GameObject target) {
		this.target = target;
		GetComponentInChildren<CameraOffset>().following = false;
	}

	public void FollowPlayer() {
		if (GetComponentInChildren<CameraOffset>() != null) {
			GetComponentInChildren<CameraOffset>().following = true;
		}
		this.target = player;
	}

	public void UpdateOffset(Vector2 newOffset) {
		this.currentOffset = newOffset;
	}

	public void ResetOffset() {
		this.currentOffset = initialOffset;
	}
}
