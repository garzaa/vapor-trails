using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour {

	bool toRespawn;
	GameObject player;

	void Start() {
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
		player = GameObject.Find("Player");
	}

	void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		if (toRespawn) {
			toRespawn = false;
			GlobalController.StartPlayerRespawning();
		}
	}

	public void RespawnPlayer() {
		if (!player.GetComponent<Entity>().facingRight) {
			player.GetComponent<Entity>().Flip();
		}
		toRespawn = true;
		GlobalController.LoadGame();
	}
}
