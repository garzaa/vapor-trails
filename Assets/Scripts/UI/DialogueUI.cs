using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DialogueUI : UIComponent {

	public Animator anim;
	public Text speakerName;
	public Text dialogue;
	public Image speakerImage;

	public bool slowRendering;
	int letterIndex;
	string textToRender;
	public bool switchingImage;
	float letterDelay = 0.05f;

	int voiceIndex = 0;

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
			if (!switchingImage) {
				anim.SetTrigger("SwitchSpeakerImage");
			}	
		} else {
			//this will be called from the animation in a separate function
			speakerImage.sprite = line.speakerImage;
		}
		speakerName.text = line.speakerName;
		this.voiceIndex = line.voiceIndex;
		StartSlowRender(line.lineText);

		if (line.activatable != null) {
			line.activatable.ActivateSwitch(true);
		}
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
			dialogue.text = textToRender.Substring(0, letterIndex+1) + MakeInvisibleText();
			if (char.IsLetter(textToRender[letterIndex])) {
				SoundManager.VoiceSound(voiceIndex);
			}
			int scalar = 1;
			if (isPause(textToRender[letterIndex])) {
				scalar = 2;
			}
			yield return new WaitForSeconds(letterDelay * scalar);
			letterIndex++;
			StartCoroutine(SlowRender());
		} else {
			//if there's no more, then the letter-by-letter rendering has stoppped
			slowRendering = false;
		}
	}

	bool isPause(char c) {
		char[] pauses = {'.', '!', ',', '?'};
		return pauses.Contains(c);
	}

	string MakeInvisibleText() {
		//why is the substring function Like This
		string invisText = textToRender.Substring(letterIndex, textToRender.Length - letterIndex - 1);
		invisText = "<color=#00000000>" + invisText + "</color>";
		return invisText;
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

	//called from the animation or in the case of an interrupt
	public void SwitchSpeakerImage() {
		speakerImage.sprite = nextImage;
	}


	//also called from animation
	public void StartSwitchingImage() {
		this.switchingImage = true;
	}

	//ditto
	public void StopSwitchingImage() {
		this.switchingImage = false;
	}

}
