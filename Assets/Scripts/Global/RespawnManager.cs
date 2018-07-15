using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour {

	RespawnPoint respawnPoint;
	bool savedGameOnce;
	bool toRespawn;
	GameObject player;

	void Start() {
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
		player = GameObject.Find("Player");
	}

	void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


	public void TrySettingRespawnPoint() {
		SceneData sd = GetSceneData();
		if (sd == null) {
			return;
		}

		if (!sd.blockRespawning && !savedGameOnce && GetSceneData() != null) {
			this.savedGameOnce = true;
			this.respawnPoint = new RespawnPoint(GlobalController.GetPlayerPos(), sd.sceneName);
		}
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		SceneData sd = GetSceneData();
		if (!savedGameOnce && sd != null) {
			TrySettingRespawnPoint();
			if (!toRespawn) {
				player = GameObject.Find("Player");
				player.transform.position = sd.defaultSpawnPoint;
			}
		} else if (toRespawn) {
			toRespawn = false;
			RepositionPlayer();
			GlobalController.StartPlayerRespawning();
			GetComponentInChildren<PlayerFollower>().DisableSmoothing();
		}
	}

	SceneData GetSceneData() {
		GameObject sd = GameObject.Find("SceneData");
		if (sd != null) {
			return sd.GetComponent<SceneData>();
		} else {
			return null;
		}
	}

	public void RespawnPlayer() {
		if (SceneManager.GetActiveScene().name == this.respawnPoint.sceneName) {
			RepositionPlayer();
			GlobalController.StartPlayerRespawning();
		} else {
			toRespawn = true;
			GetComponentInChildren<PlayerFollower>().DisableSmoothing();
			GlobalController.gc.GetComponent<TransitionManager>().LoadSceneFade(respawnPoint.sceneName);
		}
	}
	
	void RepositionPlayer() {
		player.transform.position = respawnPoint.position;
	}
}

class RespawnPoint : Object {
	public Vector2 position;
	public string sceneName;
	
	public RespawnPoint(Vector2 position, string sceneName) {
		this.position = position;
		this.sceneName = sceneName;
	}
}
