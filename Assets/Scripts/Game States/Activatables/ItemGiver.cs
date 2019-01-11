using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : Activatable {

	public InventoryItem item;

	public override void ActivateSwitch(bool b) {
		GlobalController.GetItem(this.item);
	}
}
