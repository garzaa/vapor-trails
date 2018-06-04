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

	public override void Show() {
		anim.SetBool("Letterboxed", true);
	}

	public override void Hide() {
		anim.SetBool("Letterboxed", false);
	}

	public void RenderDialogueLine(DialogueLine line) {
		speakerName.text = line.speakerName;
		StartSlowRender(line.lineText);
		speakerImage.sprite = line.speakerImage;
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

}
