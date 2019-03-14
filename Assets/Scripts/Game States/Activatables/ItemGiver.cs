using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : Activatable {

	public ItemEditorWrapper item;

	public override void ActivateSwitch(bool b) {
		GlobalController.GetItem(this.item.item);
	}
}
