using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerUI : UIComponent {

	public int max;
	public int current;

	public GameObject containerPrefab;
	public Transform gridContainer;

	bool hidden;

	public void Start() {
		DrawContainers();
	}

	void DrawContainers() {
		if (hidden) {
			return;
		}

		if (gridContainer.childCount < max) {
			for (int i=1; i<=max; i++) {
				if (i > gridContainer.childCount) {
					Instantiate(containerPrefab, Vector3.zero, Quaternion.identity, gridContainer);
				}
			}
		} else if (gridContainer.childCount > max) {
			for (int i=0; i<max-current; i++) {
				Destroy(gridContainer.GetChild(gridContainer.childCount-1).gameObject);
			}
		}

		for (int i=0; i<gridContainer.childCount; i++) {
			gridContainer.GetChild(i).GetComponent<Animator>().SetBool("Full", i<current);
		}
		
	}

	public void SetMax(int newMax) {
		this.max = newMax;
		DrawContainers();
	}

	public void SetCurrent(int newCurrent) {
		this.current = newCurrent;
		DrawContainers();
	}

	public override void Hide() {
		this.hidden = true;
	}

	public override void Show() {
		this.hidden = false;
	}
}