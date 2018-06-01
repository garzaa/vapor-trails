using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour {

	public string title;
	public string subTitle;
	public Vector2 defaultSpawnPoint;

	void Awake() {
		this.gameObject.name = "SceneData";
	}
}
