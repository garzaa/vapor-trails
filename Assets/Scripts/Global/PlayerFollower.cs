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



	void Start () {
		this.transform.position = new Vector3(
			x:player.transform.position.x + currentOffset.x,
			y:player.transform.position.y + currentOffset.y,
			z:this.transform.position.z
		);
		pc = player.GetComponent<PlayerController>();
	}
	
	void FixedUpdate() {
		if (!following) {
			return;
		}

		if (smoothing) {
			transform.position = Vector3.SmoothDamp(
				transform.position,
				new Vector3(
					x:player.transform.position.x+(lookAhead * player.GetComponent<Entity>().GetForwardScalar())+currentOffset.x,
					y:player.transform.position.y + currentOffset.y,
					z:this.transform.position.z),
				ref velocity,
				smoothAmount * Time.deltaTime
				);
		} else {
			transform.position = player.transform.position + (Vector3) currentOffset;
		}	
	}

	public void EnableSmoothing() {
		this.smoothing = true;
	}

	public void DisableSmoothing() {
		this.smoothing = false;
	}

	public void EnableFollowing() {
		this.following = true;
	}

	public void DisableFollowing() {
		this.following = false;
	}

	public void UpdateOffset(Vector2 newOffset) {
		this.currentOffset = newOffset;
	}

	public void ResetOffset() {
		this.currentOffset = initialOffset;
	}
}
