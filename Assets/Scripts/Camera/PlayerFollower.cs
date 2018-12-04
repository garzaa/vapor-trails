using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour {

	public GameObject player;
	public PlayerController pc;
	public float smoothAmount;
	public float lookAhead;
	public Vector2 initialOffset;
	Vector2 currentOffset;
	Vector3 velocity = Vector3.zero;

	bool smoothing = true;
	bool following = true;

	public GameObject target;

	void Start () {
		this.transform.position = new Vector3(
			x:player.transform.position.x + currentOffset.x,
			y:player.transform.position.y + currentOffset.y,
			z:this.transform.position.z
		);
		pc = player.GetComponent<PlayerController>();
		this.target = player;
	}
	
	void FixedUpdate() {
		if (!following) {
			return;
		}

		if (target == null) {
			FollowPlayer();
		}

		if (smoothing) {
			transform.position = Vector3.SmoothDamp(
				transform.position,
				new Vector3(
					x:target.transform.position.x + currentOffset.x,
					y:target.transform.position.y + currentOffset.y,
					z:this.transform.position.z),
				ref velocity,
				smoothAmount * Time.deltaTime
				);
		} else {
			transform.position = target.transform.position + (Vector3) currentOffset;
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
		GetComponentInChildren<CameraOffset>().following = true;
		this.target = player;
	}

	public void UpdateOffset(Vector2 newOffset) {
		this.currentOffset = newOffset;
	}

	public void ResetOffset() {
		this.currentOffset = initialOffset;
	}
}
