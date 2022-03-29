using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour {
	void Start() {
		GetComponent<Text>().text = "Vapor Trails "+ Application.version + "b";
	}
}
