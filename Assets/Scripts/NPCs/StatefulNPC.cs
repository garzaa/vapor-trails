using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulNPC : NPC {

	public StatefulNPC(NPCConversations c) : base(c) {
		this.conversations = c;
	}

	override protected void ExtendedStart() {
		persistence = GetComponent<PersistentNPC>();
		// get the last loaded (most recent) conversation
		NPCConversations[] possibleConversations = GetComponentsInChildren<NPCConversations>(includeInactive:false);
		this.conversations = possibleConversations[possibleConversations.Length-1];
		this.currentConversation = 0;
		this.currentDialogueLine = 0;
	}

	public void ReactToStateChange() {
		this.ExtendedStart();
	}

	override public void AddPrompt() {
		base.AddPrompt();
	}

}
