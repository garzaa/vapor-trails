using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalController : MonoBehaviour {

	public static GlobalController gc;
	public TitleText editorTitleText;
	public static TitleText titleText;
	static SignUI signUI;
	static DialogueUI dialogueUI;
	static PlayerController pc;
	static bool dialogueOpen;
	static bool dialogueOpenedThisFrame = false;
	static NPC currentNPC;

	void Awake()
	{
		gc = this;
		titleText = editorTitleText;
		DontDestroyOnLoad(this);
		dialogueUI = GetComponentInChildren<DialogueUI>();
		signUI = GetComponentInChildren<SignUI>();
		pc = GetComponentInChildren<PlayerController>();
	}

	public static void ShowTitleText(string title, string subTitle = null) {
		titleText.ShowText(title, subTitle);
	}

	public static void RespawnPlayer() {
		
	}

	void Update() {
		if (dialogueOpen && Input.GetButtonDown("Submit") && !dialogueOpenedThisFrame) {
			if (dialogueUI.slowRendering) {
				dialogueUI.CancelSlowRender();
				return;
			}

			if (dialogueUI.switchingImage) {
				dialogueUI.SwitchSpeakerImage();
			}

			//advance dialogue line or close
			DialogueLine nextLine = currentNPC.GetNextLine();
			if (nextLine != null) {
				dialogueUI.RenderDialogueLine(nextLine);
			} else {
				ExitDialogue();
			}
		}
		dialogueOpenedThisFrame = false;
	}

	public static void EnterDialogue(NPC npc) {
		dialogueUI.ShowNameAndPicture(npc.GetCurrentLine());
		dialogueUI.Show();
		pc.EnterDialogue();
		currentNPC = npc;
		dialogueOpenedThisFrame = true;
	}

	public static void ExitDialogue() {
		dialogueOpen = false;
		dialogueUI.Hide();
		pc.ExitDialogue();
		currentNPC.CloseDialogue();
		currentNPC = null;
	}

	public static void FinishOpeningLetterboxes() {
		dialogueOpen = true;
		DialogueLine nextLine = currentNPC.GetNextLine();
		if (nextLine != null) {
			dialogueUI.RenderDialogueLine(nextLine);
		} else {
			ExitDialogue();
		}
	}

	public static void OpenSign(string text, Vector2 position) {
		if (!signUI.IsVisible()) {
			signUI.SetText(text);
			signUI.SetPosition(position);
			signUI.Show();
		}
	}

	public static void CloseSign() {
		signUI.Hide();
	}
}
