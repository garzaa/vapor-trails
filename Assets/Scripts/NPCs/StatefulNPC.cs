using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulNPC : NPC {

	override protected void ExtendedStart() {
		this.conversations = GetComponentsInChildren<NPCConversations>(includeInactive:false)[0];
		this.currentConversation = 0;
		this.currentDialogueLine = 0;
	}

	public void ReactToStateChange() {
		this.ExtendedStart();
	}

}
