using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Spawns the specified prefab whenever the player gets within the specified radius.
 */
public class ProximitySpawner : MonoBehaviour {

	private GameObject player;

	public float triggerRadius = 5f;
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
		CheckTriggerDistance();
	}

	void OnPlayerEnter() {
		if (this.spawned == null) {
			Spawn();
		}
	}

	void CheckTriggerDistance() {
		if (Vector2.Distance(player.transform.position, this.transform.position) < triggerRadius) {
			this.OnPlayerEnter();
		}
	}

	void Spawn() {
		this.spawned = (GameObject) Instantiate(toSpawn, this.transform.position, Quaternion.identity);
	}
}