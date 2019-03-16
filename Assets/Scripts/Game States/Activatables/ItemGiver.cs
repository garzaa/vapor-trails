using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : Activatable {

	public ItemWrapper item;

	public override void ActivateSwitch(bool b) {
		GlobalController.AddItem(this.item.GetItem());
	}
}
