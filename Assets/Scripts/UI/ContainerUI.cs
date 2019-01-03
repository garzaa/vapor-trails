using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContainerUI : UIComponent {

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

	bool hidden;

	public void Start() {
		InitPositions();
		DrawContainers();
	}

	void InitPositions() {
		initialGap = initialContainer.GetComponent<RectTransform>().position.x;
		initialPos = initialContainer.transform.position;
		initialRectWidth = initialContainer.GetComponent<RectTransform>().rect.width * GetCanvasScale();
	}

	void DrawContainers() {
		if (hidden) {
			return;
		}

		containerWidth = initialRectWidth;
		ClearContainers();

		//for each full container, draw one
		for (int i=0; i<current; i++) {
			Vector2 newPos = new Vector2(
				x:initialPos.x + (i * (containerWidth + initialGap)),
				y:initialPos.y
			);
			Image img = Instantiate(fullContainer, newPos, Quaternion.identity, this.transform);
		}
		//then do the same for the empty containers 
		for (int i=current; i<max; i++) {
			Vector2 newPos = new Vector2(
				x:initialPos.x + (i * (containerWidth + initialGap)),
				y:initialPos.y
			);
			Image img = Instantiate(emptyContainer, newPos, Quaternion.identity, this.transform);
		}
	}

	float GetCanvasScale() {
		return GameObject.Find("PixelCanvas").GetComponent<Canvas>().scaleFactor;
	}

	public void SetMax(int newMax) {
		int prevMax = this.max;
		this.max = newMax;
		DrawContainers();
	}

	public void SetCurrent(int newCurrent) {
		int prevCurrent = this.current;
		this.current = newCurrent;
		DrawContainers();
	}

	void ClearContainers() {
		foreach (Transform t in this.transform) {
			Destroy(t.gameObject);
		}
	}

	public override void Hide() {
		this.hidden = true;
		foreach (Image i in GetComponentsInChildren<Image>()) {
			i.enabled = false;
		}
	}

	public override void Show() {
		this.hidden = false;
		//dumb hack, but hey
		DrawContainers();
	}
	
}