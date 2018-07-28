using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneData : MonoBehaviour {

	public string title;
	public string subTitle;
	public bool hidePlayer;
	public bool blockRespawning;
	[HideInInspector]
	public string sceneName;
	public bool loadOtherSceneAtStart;
	public string otherSceneName;
	public bool hideUI;
	public bool lockPlayer;

	void Awake() {
		this.gameObject.name = "SceneData";
		this.sceneName = SceneManager.GetActiveScene().name;
	}
}
