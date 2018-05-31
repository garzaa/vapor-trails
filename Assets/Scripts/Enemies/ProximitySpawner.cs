using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Spawns the specified prefab whenever the player gets within the specified radius.
 */
public class ProximitySpawner : MonoBehaviour {

	private GameObject player;

	public float triggerRadius = 5f;
	private Vector2 lastPlayerPos;
	public GameObject toSpawn;
	private GameObject spawned;

	void Start () {
		player = GameObject.Find("Player");
		if (toSpawn == null) {
			Debug.LogWarning("ProximitySpawner exists, but has nothing to spawn.");
		} else if (Vector2.Distance(player.transform.position, this.transform.position) < triggerRadius) {
			Spawn();
		}
	}
	
	void FixedUpdate () {
		if (CheckTriggerDistance()) {
			this.OnPlayerEnter();
		}
	}

	void OnPlayerEnter() {
		if (this.spawned == null) {
			Spawn();
		}
	}

	bool CheckTriggerDistance() {
		if (lastPlayerPos == Vector2.zero) {
			lastPlayerPos = player.transform.position;
		} else {
			if (Vector2.Distance(lastPlayerPos, this.transform.position) > triggerRadius
				&& Vector2.Distance(player.transform.position, this.transform.position) < triggerRadius) {
					lastPlayerPos = player.transform.position;
					return true;
				}
		}
		lastPlayerPos = player.transform.position;
		return false;
	}

	void Spawn() {
		this.spawned = (GameObject) Instantiate(toSpawn, this.transform.position, Quaternion.identity);
	}
}