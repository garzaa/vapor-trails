using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityGetUI : MonoBehaviour {

	public Text abilityText;
	public Text descriptionText;
	public Text instructionText;
	public Image abilityImage;

	Animator anim;

	void Awake() {
		anim = GetComponent<Animator>();
	}
	
	public void GetItem(AbilityItem item) {
		this.abilityText.text = item.itemName;
		this.descriptionText.text = item.itemDescription;
		this.instructionText.text = ControllerTextChanger.ReplaceText(item.instructions);
		this.abilityImage.sprite = item.abilitySprite;
		GlobalController.ShowAbilityGetUI();
	}
}
