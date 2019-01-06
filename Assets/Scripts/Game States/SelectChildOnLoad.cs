using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectChildOnLoad : MonoBehaviour {
	SelectionMethod selectionMethod = SelectionMethod.Random;

	void Start () {
		List<GameObject> children  = new List<GameObject>();
		foreach (Transform child in this.transform) {
			children.Add(child.gameObject);
		}
		
		if (selectionMethod == SelectionMethod.Random) {
			children[Random.Range(0, children.Count)].SetActive(true);
		} 

		else if (selectionMethod == SelectionMethod.FirstActive) {
			int firstActiveIndex = 0;
			for (int i=0; i<children.Count; i++) {
				if (children[i].activeSelf) {
					firstActiveIndex = i;
					break;
				}
			}
			DisableAll();
			children[firstActiveIndex].SetActive(true);
		}

		else if (selectionMethod == SelectionMethod.LastActive) {
			int lastActiveIndex = 0;
			for (int i=0; i<children.Count; i++) {
				if (children[i].activeSelf) {
					lastActiveIndex = i;
				}
			}
			DisableAll();
			children[lastActiveIndex].SetActive(true);
		}
	}
	
	void DisableAll() {
		foreach (Transform child in this.transform) {
			child.gameObject.SetActive(false);
		}
	}

	enum SelectionMethod {
		FirstActive,
		LastActive,
		Random
	}
}