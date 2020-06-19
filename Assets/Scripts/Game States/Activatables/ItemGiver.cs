using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : Activatable {
	public Item toGive;

	public override void ActivateSwitch(bool b) {
		GlobalController.AddItem(this.toGive.Instance());
	}
}
