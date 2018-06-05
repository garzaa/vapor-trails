using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIComponent {

	public Animator anim;
	public Text speakerName;
	public Text dialogue;
	public Image speakerImage;

	public bool slowRendering;
	int letterIndex;
	string textToRender;

	Sprite nextImage;

	public override void Show() {
		ClearDialogue();
		anim.SetBool("Letterboxed", true);
	}

	public override void Hide() {
		anim.SetBool("Letterboxed", false);
	}

	public void RenderDialogueLine(DialogueLine line) {
		//if the speaker name and portrait differ, start the animation for the portrait change
		if (line.speakerImage != speakerImage.sprite && line.speakerName != speakerName.text) {
			nextImage = line.speakerImage;
			anim.SetTrigger("SwitchSpeakerImage");
		} else {
			//this will be called from the animation in a separate function
			speakerImage.sprite = line.speakerImage;
		}
		speakerName.text = line.speakerName;
		StartSlowRender(line.lineText);
	}

	void ClearDialogue() {
		dialogue.text = "";
	}

	void StartSlowRender(string str) {
		ClearDialogue();
		letterIndex = 0;
		textToRender = str;
		slowRendering = true;
		StartCoroutine(SlowRender());
	}

	IEnumerator SlowRender() {
		//then call self again to render the next letter
		if (letterIndex < textToRender.Length && slowRendering) {
			dialogue.text = dialogue.text + textToRender[letterIndex];
			letterIndex++;
			yield return new WaitForSeconds(.02f);
			StartCoroutine(SlowRender());
		} else {
			//if there's no more, then the letter-by-letter rendering has stoppped
			slowRendering = false;
		}
	}

	public void CancelSlowRender() {
		dialogue.text = textToRender;
		slowRendering = false;
	}

	public void FinishOpeningLetterboxes() {
		GlobalController.FinishOpeningLetterboxes();
	}

	//for when the dialogue first fades in
	public void ShowNameAndPicture(DialogueLine line) {
		ClearDialogue();
		speakerName.text = line.speakerName;
		speakerImage.sprite = line.speakerImage;
	}

	//called from the animation
	public void SwitchSpeakerImage() {
		speakerImage.sprite = nextImage;
	}

}
