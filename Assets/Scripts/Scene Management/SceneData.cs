using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneData : MonoBehaviour {

	public string title;
	public string subTitle;
	public bool hidePlayer;
	public bool enablePausing = true;
	public bool pauseSpeedrunTimer;
	public bool hideUI;
	public bool forceFaceRight;
	public bool disableCameraLook;
	public bool dirty; 

	void Awake() {
		this.gameObject.name = "SceneData";
	}
}
