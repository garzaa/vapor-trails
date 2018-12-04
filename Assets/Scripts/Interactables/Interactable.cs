using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

	//constants
	const float topMargin = 3f/100f;

	//editor-linked items
	public GameObject promptPrefab;
	
	GameObject currentPrompt = null;

	void Start() {
		this.gameObject.layer = LayerMask.NameToLayer(Layers.Interactables);
		ExtendedStart();
	}	

	protected virtual void ExtendedStart() {

	}

	public void AddPrompt() {
		//if there's a sign, it doesn't need a prompt
		if (GetComponent<Sign>() != null) {
			return;
		}

		if (currentPrompt == null && promptPrefab != null) {
			//if there's a sprite renderer on this object, stick a prompt a little ways on top of it
			if (gameObject.GetComponent<SpriteRenderer>() != null) {
				SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
				float upperBound = spr.bounds.max.y;
				float yPos = upperBound + topMargin + promptPrefab.GetComponent<SpriteRenderer>().bounds.extents.y;
				currentPrompt = (GameObject) Instantiate(promptPrefab, new Vector2(this.transform.position.x, yPos), Quaternion.identity);
			} 
			//otherwise just do it above the gameobject's center
			else {
				currentPrompt = (GameObject) Instantiate(
					promptPrefab, 
					new Vector2(
						this.transform.position.x, 
						this.transform.position.y + topMargin
						), 
					Quaternion.identity, 
					this.transform
				);
			}
		}
	}

	public void RemovePrompt() {
		if (currentPrompt != null) {
			Destroy(currentPrompt.gameObject);
		}
	}

	public virtual void Interact(GameObject player) {

	}

}
