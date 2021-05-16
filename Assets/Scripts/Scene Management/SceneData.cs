using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneData : MonoBehaviour {

	public string title;
	public string subTitle;
	public bool hidePlayer;
	public bool enablePausing = true;
	public bool blockRespawning;
	[HideInInspector]
	public string sceneName;
	public bool loadOtherSceneAtStart;
	public string otherSceneName;
	public bool hideUI;
	public bool unlockPlayer;
	public bool forceFaceRight;

	void Awake() {
		this.gameObject.name = "SceneData";
		this.sceneName = SceneManager.GetActiveScene().name;
	}
}
