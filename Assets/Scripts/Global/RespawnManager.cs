using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour {
	GameObject player;

	public void RespawnPlayer() {
		GlobalController.LoadGame();
	}
}
