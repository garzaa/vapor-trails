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

	List<Image> drawnContainers;

	bool hidden;

	public void Start() {
		drawnContainers = new List<Image>();
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

		if (this.drawnContainers == null) {
			this.drawnContainers = new List<Image>();
			InitPositions();
		}

		InitPositions();
		containerWidth = initialRectWidth;
		ClearContainers();

		//for each full container, draw one
		for (int i=0; i<current; i++) {
			Vector2 newPos = new Vector2(
				x:initialPos.x + (i * (containerWidth + initialGap)),
				y:initialPos.y
			);
			Image img = Instantiate(fullContainer, newPos, Quaternion.identity, this.transform);
			drawnContainers.Add(img);
		}
		//then do the same for the empty containers 
		for (int i=current; i<max; i++) {
			Vector2 newPos = new Vector2(
				x:initialPos.x + (i * (containerWidth + initialGap)),
				y:initialPos.y
			);
			Image img = Instantiate(emptyContainer, newPos, Quaternion.identity, this.transform);
			drawnContainers.Add(img);
		}
	}

	float GetCanvasScale() {
		return GameObject.Find("PixelCanvas").GetComponent<Canvas>().scaleFactor;
	}

	public void SetMax(int newMax) {
		int prevMax = this.max;
		this.max = newMax;
		if (prevMax != this.max) {
			DrawContainers();
		}
	}

	public void SetCurrent(int newCurrent) {
		int prevCurrent = this.current;
		this.current = newCurrent;
		if (prevCurrent != this.current) {
			DrawContainers();
		}
	}

	void ClearContainers() {
		//this is throwing an error, maybe in the iterator mechanics or something
		if (this.drawnContainers != null) {
			foreach (Image i in drawnContainers) {
				Destroy(i.gameObject);
			}
			drawnContainers.Clear();
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