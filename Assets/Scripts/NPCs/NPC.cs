using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable {

	public int currentConversation = 0;
	public int currentDialogueLine = 0;

	//TODO: maybe fix this?
	NPCConversations conversations;

	public override void ExtendedStart() {
		conversations = GetComponent<NPCConversations>();
	}

	public override void Interact(GameObject player) {
		//if there's a sign object attached
		if (GetComponent<Sign>() != null) {
			GlobalController.CloseSign();
		}

		//start at the beginning of whatever conversation
		currentDialogueLine = 0;

		//restart the last conversation if it's been reached
		if (currentConversation >= conversations.Count()) {
			currentConversation = conversations.Count() - 1;
		}

		GlobalController.EnterDialogue(this);
	}

	public DialogueLine GetNextLine() {
		if (currentDialogueLine < conversations[currentConversation].Count()) {
			FinishDialogueLine(currentConversation, currentDialogueLine-1);
			currentDialogueLine++;
			return conversations[currentConversation][currentDialogueLine-1];
		} else {
			currentConversation++;
			return null;
		}
	}

	public virtual void FinishDialogueLine(int conversationNumber, int lineNumber) {

	}

	public DialogueLine GetCurrentLine() {
		return conversations[currentConversation][currentDialogueLine];
	}

	public void DisableDialogueSkipping() {

	}

	public void EnableDialogueSkipping() {
		
	}

	public virtual void CloseDialogue() {
		
	}

}
