﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalController : MonoBehaviour {

	public static GlobalController gc;
	public TitleText editorTitleText;
	public static TitleText titleText;
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
		dialogueUI = GetComponent<DialogueUI>();
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
		dialogueUI.Show();
		pc.EnterDialogue();
		currentNPC = npc;
		dialogueOpenedThisFrame = true;
	}

	public static void ExitDialogue() {
		dialogueOpen = false;
		dialogueUI.Hide();
		pc.ExitDialogue();
	}

	public void FinishOpeningLetterboxes() {
		dialogueOpen = true;
		DialogueLine nextLine = currentNPC.GetNextLine();
		if (nextLine != null) {
			dialogueUI.RenderDialogueLine(nextLine);
		} else {
			ExitDialogue();
		}
	}
}