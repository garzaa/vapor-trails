using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulNPC : NPC {

	override protected void ExtendedStart() {
		// get the last loaded (most recent) conversation
		NPCConversations[] possibleConversations = GetComponentsInChildren<NPCConversations>(includeInactive:false);
		this.conversations = possibleConversations[possibleConversations.Length-1];
		this.currentConversation = 0;
		this.currentDialogueLine = 0;
	}

	public void ReactToStateChange() {
		this.ExtendedStart();
	}

}
