using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLineCriteria : ActivationCriteria {
	
	[Header("DEPRECATED")]
	public NPC npc;
	public int conversationNum;
	public int lineNum;

	override protected void UpdateSatisfied() {
		if (npc.currentConversation >= conversationNum
		&& npc.currentDialogueLine >= lineNum) {
			satisfied = true;
		} else {
			satisfied = false;
		}
	}

	void FixedUpdate() {
		UpdateSatisfied();
	}
}
