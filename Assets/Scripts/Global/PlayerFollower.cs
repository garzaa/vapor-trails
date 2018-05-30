using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour {

	public GameObject player;
	public float moveSpeed;
	public float lookAhead;
	Vector3 velocity = Vector3.zero;

	bool smoothing = true;
	bool following = true;

	void Start () {
		this.transform.position = new Vector3(x:player.transform.position.x,
			y:player.transform.position.y,
			z:this.transform.position.z
		);
	}
	
	void FixedUpdate() {
		if (!following) {
			return;
		}
		if (smoothing) {
			transform.position = Vector3.SmoothDamp(
				transform.position,
				new Vector3(
					x:player.transform.position.x+(lookAhead * player.GetComponent<Entity>().GetForwardScalar()),
					y:player.transform.position.y,
					z:this.transform.position.z),
				ref velocity,
				moveSpeed * Time.deltaTime
				);
		} else {
			transform.position = player.transform.position;
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
}
