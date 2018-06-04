using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : UIComponent {

	public Animator anim;
	public Text speakerName;
	public Text dialogue;
	public Image speakerImage;

	public override void Show() {
		anim.SetBool("Letterboxed", true);
	}

	public override void Hide() {
		anim.SetBool("Letterboxed", false);
	}

	public void RenderDialogueLine(DialogueLine line) {
		speakerName.text = line.speakerName;
		dialogue.text = line.lineText;
		speakerImage.sprite = line.speakerImage;
	}

}
