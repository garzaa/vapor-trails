using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLineCriteria : ActivationCriteria {
	
	public NPC npc;
	public int conversationNum;
	public int lineNum;

	public override bool CheckSatisfied() {
		if (npc.currentConversation >= conversationNum
		&& npc.currentDialogueLine >= lineNum) {
			return true;
		} else {
			return false;
		}
	}
}
