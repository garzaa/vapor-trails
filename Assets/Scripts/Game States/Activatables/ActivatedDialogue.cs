using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatedDialogue : Activatable {

	public NPC npc;

	public override void ActivateSwitch(bool b) {
		if (b) {
			npc.Interact(GlobalController.pc.gameObject);
		}
	}
}
