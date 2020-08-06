using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : Activatable {
	public Item toGive;
	public StoredItem storedItem;
	public bool quiet;

	public override void ActivateSwitch(bool b) {
		if (b) {
			if (toGive != null) GlobalController.AddItem(this.toGive.Instance(), quiet: quiet);
			else GlobalController.AddItem(storedItem.item, quiet: quiet);
		}
	}
}
