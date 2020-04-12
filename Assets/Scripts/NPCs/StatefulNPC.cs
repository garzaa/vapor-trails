using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulNPC : NPC {

	public bool pickFirstConvo = false;

	public StatefulNPC(NPCConversations c) : base(c) {
		this.conversations = c;
	}

	override protected void ExtendedStart() {
		persistence = GetComponent<PersistentNPC>();
		NPCConversations[] possibleConversations = GetComponentsInChildren<NPCConversations>(includeInactive:false);
		this.conversations = possibleConversations[possibleConversations.Length-1];
		if (pickFirstConvo) this.conversations = possibleConversations[0];
		this.currentConversation = 0;
		this.currentDialogueLine = 0;
		if (!name.ToLower().Contains("sign")) {
			Instantiate(Resources.Load("NPCIcon"), transform.position, Quaternion.identity, this.transform);
		}

	}

	public void ReactToStateChange() {
		this.ExtendedStart();
	}

	override public void AddPrompt() {
		base.AddPrompt();
	}

}
