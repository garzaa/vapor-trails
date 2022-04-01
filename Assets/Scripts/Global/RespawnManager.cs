using UnityEngine;

public class RespawnManager : MonoBehaviour {
	GameObject player;

	public void RespawnPlayer() {
		SaveManager.LoadGame();
	}
}
