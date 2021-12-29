using UnityEngine;

public class PlayerFollower : MonoBehaviour {

	public GameObject target;
	GameObject player;

	public bool followX = true;
	public bool followY = true;

	void Start() {
		if (target == null) {
			target = GameObject.FindObjectOfType<PlayerController>().gameObject;
		}
	}

	void LateUpdate() {
		transform.position = new Vector3(
				x: followX ? target.transform.position.x : transform.position.x,
				y: followY ? target.transform.position.y : transform.position.y,
				z: transform.position.z
		);
	}
}
