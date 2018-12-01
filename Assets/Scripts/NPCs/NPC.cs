using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Interactable {
	NPCConversations conversations;

	public override void ExtendedStart() {
		conversations = GetComponent<NPCConversations>();
	}

	public int currentConversation = 0;
	public int currentDialogueLine = 0;

	public override void Interact(GameObject player) {
		if (GlobalController.dialogueClosedThisFrame) {
			return;
		}

		//if there's a sign object attached
		if (GetComponent<Sign>() != null) {
			GlobalController.CloseSign();
		}

		//start at the beginning of whatever conversation
		currentDialogueLine = 0;

		//no need to restart the last conversation if it's been reached
		//the NPC conversation will take care of it

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

	public bool hasNextLine() {
		return currentDialogueLine < conversations[currentConversation].Count();
	}

	public virtual void FinishDialogueLine(int conversationNumber, int lineNumber) {
		
	}

	//called whenever auxiliary conversations are added or removed
	public void ReactToLineRemoval() {
		//unfortunate nomenclature
		//but if the NPC is somewhere in the auxiliary conversations, reset it
		if (currentConversation > conversations.conversations.Count) {
			currentConversation = conversations.conversations.Count - 1;
		}
		//if there are new conversations to index into, start at the first one there
		if (conversations.auxConversations.Count > 0) {
			currentConversation = conversations.conversations.Count;
		}
	}

	public DialogueLine GetCurrentLine() {
		return conversations[currentConversation][currentDialogueLine];
	}

	public void CloseDialogue() {

	}

}
