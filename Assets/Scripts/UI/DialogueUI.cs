using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DialogueUI : CloseableUI {

	public Animator anim;
	public Text speakerName;
	public Text dialogue;
	public Image speakerImage;

	public bool slowRendering;
	int letterIndex;
	string textToRender;
	public bool switchingImage;
	float letterDelay = 0.01f;

	int voiceIndex = 0;

	Sprite nextImage;

	static DialogueUI dialogueUI;

	void Start() {
		dialogueUI = this;
	}

	public static bool LineFullyRendered() {
		return !dialogueUI.slowRendering;
	}

	public override void Open() {
		base.Open();
		anim.SetBool("LastLine", false);
		ClearDialogue();
		anim.SetBool("Letterboxed", true);
	}

	public override void Close() {
		if (!GlobalController.inAnimationCutscene) base.Close();
		anim.SetBool("Letterboxed", false);
	}

	public void RenderDialogueLine(DialogueLine line, bool hasMoreLines, bool fromCutscene = false) {
		anim.SetBool("IsSign", line.speakerImage == null);
		anim.SetBool("LastLine", !hasMoreLines);
		if (line.speakerImage != speakerImage.sprite && line.speakerName != speakerName.text && !fromCutscene) {
			nextImage = line.speakerImage;
			if (!switchingImage) {
				// anim.SetTrigger("SwitchSpeakerImage");
				SwitchSpeakerImage();
			}	
		} else {
			speakerImage.sprite = line.speakerImage;
		}
		speakerName.text = line.speakerName;
		this.voiceIndex = (int) line.voiceSound;
		string controllerFriendlyText = ControllerTextChanger.ReplaceText(line.lineText);
		StartSlowRender(controllerFriendlyText);

		if (line.gameFlag != GameFlag.None) {
			GlobalController.AddGameFlag(line.gameFlag);
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
				scalar = 7;
			}
			yield return new WaitForSecondsRealtime(letterDelay * scalar);
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
		string invisText = textToRender.Substring(letterIndex+1);
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

	public void FinishClosingLetterboxes() {
		GlobalController.FinishClosingLetterboxes();
	}

	//for when the dialogue first fades in
	public void ShowNameAndPicture(DialogueLine line) {
		ClearDialogue();
		anim.SetBool("IsSign", line.speakerImage == null);
		speakerName.text = line.speakerName;
		speakerImage.sprite = line.speakerImage;
	}

	//called from the animation or in the case of an interrupt
	public void SwitchSpeakerImage() {
		speakerImage.sprite = nextImage;
		speakerImage.preserveAspect = true;
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
