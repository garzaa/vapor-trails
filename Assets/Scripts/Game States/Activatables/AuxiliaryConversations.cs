using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AuxiliaryConversations : Activatable {

	public List<Conversation> conversations;
	public NPC target;

	public override void ActivateSwitch(bool b) {
		if (b) {
			target.GetComponent<NPCConversations>().AddConversations(this.conversations);
		} else {
			target.GetComponent<NPCConversations>().RemoveConversations(this.conversations);
		}
	}

}
