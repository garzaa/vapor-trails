using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class PersistentNPC : PersistentObject {

	NPC npc;


	protected override void SetDefaults() {
		npc = GetComponent<NPC>();
		npc.OnEnable();

		SetDefault("dialogueHash", "0");
		SetDefault("currentConversation", 0);
		SetDefault("currentDialogueLine", 0);

		if (npc.GetConversationsHash().ToString().Equals(GetProperty<string>("dialogueHash"))) {
			npc.currentConversation = GetProperty<int>("currentConversation");
			npc.currentDialogueLine = GetProperty<int>("currentDialogueLine");
		}
	}

	public void ReactToDialogueClose() {
		SetProperty("dialogueHash", npc.GetConversationsHash());
		SetProperty("currentConversation", npc.currentConversation);
		SetProperty("currentDialogueLine", npc.currentDialogueLine);
	}
	
}
