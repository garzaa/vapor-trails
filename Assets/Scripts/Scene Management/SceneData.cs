using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneData : MonoBehaviour {

	public string title;
	public string subTitle;
	public bool hidePlayer;
	public bool blockRespawning;
	[HideInInspector]
	public string sceneName;
	public Vector2 defaultSpawnPoint;
	public bool loadOtherSceneAtStart;
	public string otherSceneName;

	void Awake() {
		this.gameObject.name = "SceneData";
		this.sceneName = SceneManager.GetActiveScene().name;
	}
}
