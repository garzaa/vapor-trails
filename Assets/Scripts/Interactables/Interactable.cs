using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

	//constants
	const float topMargin = 3f/100f;

	//editor-linked items
	public GameObject promptPrefab;
	public Transform optionalPromptTransform;

	public bool forceFlipTo;
	
	GameObject currentPrompt = null;

	void OnEnable() {
		this.gameObject.layer = LayerMask.NameToLayer(Layers.Interactables);
		ExtendedStart();
	}	

	protected virtual void ExtendedStart() {

	}

	public virtual void InteractFromPlayer(GameObject player) {
		Interact(player);
	}

	public virtual void AddPrompt() {
		//if there's a sign, it doesn't need a prompt
		if (GetComponent<Sign>() != null) {
			return;
		}

		if (currentPrompt == null && promptPrefab != null) {
			currentPrompt = Instantiate(
				promptPrefab,
				GetPromptPosition(),
				Quaternion.identity,
				this.transform
			);
		}
	}

	protected Vector2 GetPromptPosition() {
		if (optionalPromptTransform != null) return optionalPromptTransform.position;

		//if there's a sprite renderer on this object, stick a prompt a little ways on top of it
		if (gameObject.GetComponent<SpriteRenderer>() != null) {
			SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
			float upperBound = spr.bounds.max.y;
			float yPos = upperBound + topMargin + promptPrefab.GetComponent<SpriteRenderer>().bounds.extents.y;
			return new Vector2(this.transform.position.x, yPos);
		} 
		//otherwise just do it above the gameobject's center
		else {
			return new Vector2(
				this.transform.position.x, 
				this.transform.position.y + topMargin
			);
		}			
	}

	public void RemovePrompt() {
		if (currentPrompt != null) {
			Destroy(currentPrompt.gameObject);
		}
	}

	public virtual void Interact(GameObject player) {
		if (forceFlipTo) {
			if (!GlobalController.pc.IsFacing(this.gameObject)) {
				GlobalController.pc.ForceFlip();
			}
		}
		RemovePrompt();
	}

}
