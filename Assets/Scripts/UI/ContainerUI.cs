using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerUI : MonoBehaviour {

	public int max;
	public int current;

	public Image emptyContainer;
	public Image fullContainer;

	public Transform initialContainer;

	//how far apart to space the containers
	//taken from the width from the screen edge :^)
	float initialGap;
	float initialRectWidth;
	float containerWidth;
	Vector2 initialPos;

	void Start() {
		initialGap = initialContainer.GetComponent<RectTransform>().position.x;
		initialPos = initialContainer.transform.position;
		initialRectWidth = initialContainer.GetComponent<RectTransform>().rect.width * GetCanvasScale();
		UpdateContainers();
	}

	void Update() {

	}

	void UpdateContainers() {
		containerWidth = initialRectWidth;
		//first, remove all the old containers
		foreach (Transform child in this.transform) {
			Destroy(child.gameObject);
		}

		//for each full container, draw one
		for (int i=0; i<current; i++) {
			Vector2 newPos = new Vector2(
				x:initialPos.x + (i * (containerWidth + initialGap)),
				y:initialPos.y
			);
			Instantiate(fullContainer, newPos, Quaternion.identity, this.transform.parent.transform);
		}
		//then do the same for the empty containers 
		for (int i=current; i<max; i++) {
			Vector2 newPos = new Vector2(
				x:initialPos.x + (i * (containerWidth + initialGap)),
				y:initialPos.y
			);
			Instantiate(emptyContainer, newPos, Quaternion.identity, this.transform.parent.transform);
		}
	}

	float GetCanvasScale() {
		return GameObject.Find("Canvas").GetComponent<Canvas>().scaleFactor;
	}
	
}
