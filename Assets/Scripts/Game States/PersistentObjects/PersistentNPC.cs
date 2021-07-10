using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PersistentNPC : PersistentObject {
	
	NPC npc;

	public override void Start() {
		//load, check the dialogue hash, if it's the same then go to the last line
		persistentProperties = new Hashtable();
		npc = GetComponent<NPC>();
		SerializedPersistentObject o = LoadObjectState();
		// this isn't a race condition with changing NPC dialogue on game flags, because that happens before Start()
		if (o != null && npc.GetConversationsHash() == (int) o.persistentProperties["dialogueHash"]) {
			ConstructFromSerialized(o);
			npc.currentConversation = (int) persistentProperties["currentConversation"];
			npc.currentDialogueLine = (int) persistentProperties["currentDialogueLine"];
		}	
	}

	public void ReactToDialogueClose() {
		UpdateObjectState();
	}

	protected override void UpdateObjectState() {
		persistentProperties = new Hashtable();
		persistentProperties.Add("dialogueHash", npc.GetConversationsHash());
		persistentProperties.Add("currentConversation", npc.currentConversation);
		persistentProperties.Add("currentDialogueLine", npc.currentDialogueLine);
		SaveObjectState();
	}
	
}
