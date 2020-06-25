using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulNPC : NPC {

	public bool pickFirstConvo = false;
	// don't update state in the middle of a conversation
	bool queuedChange = false;
	string currentConversationName;

	public StatefulNPC(NPCConversations c) : base(c) {
		this.conversations = c;
	}

	override protected void ExtendedStart() {
		persistence = GetComponent<PersistentNPC>();
		if (generateMapIcon && !name.ToLower().Contains("sign")) {
			SpawnMapIcon();
		}
		PickFirstConversation();

	}

	public void ReactToStateChange() {
		PickFirstConversation();
	}

	override public void AddPrompt() {
		base.AddPrompt();
	}

	override public void CloseDialogue() {
		base.CloseDialogue();
		if (queuedChange) {
			PickFirstConversation();
		}
	}

	void PickFirstConversation() {
		if (inDialogue) {
			queuedChange = true;
			return;
		}
		NPCConversations[] possibleConversations = GetComponentsInChildren<NPCConversations>(includeInactive:false);
		this.conversations = possibleConversations[possibleConversations.Length-1];
		if (pickFirstConvo) this.conversations = possibleConversations[0];
		// only reset the counter if there's a change
		if (currentConversationName != this.conversations.name) {
			this.currentConversation = 0;
			this.currentDialogueLine = 0;
		}
		currentConversationName = this.conversations.name;
		queuedChange = false;
	}

}
