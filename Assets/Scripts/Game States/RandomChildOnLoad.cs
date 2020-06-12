using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChildOnLoad : MonoBehaviour {
	void Start () {
		List<GameObject> children = new List<GameObject>();
		foreach (Transform child in this.transform) {
			children.Add(child.gameObject);
			child.gameObject.SetActive(false);
		}
		children[Random.Range(0, children.Count)].SetActive(true);
	}
	
}
