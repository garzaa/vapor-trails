using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGiver : Activatable {
	public Item toGive;
	public List<Item> items;
	public bool quiet;

	public override void ActivateSwitch(bool b) {
		if (b) {
			if (toGive != null) GlobalController.AddItem(new StoredItem(toGive), quiet: this.quiet);
			else {
				foreach (Item i in items) {
					GlobalController.AddItem(new StoredItem(i), quiet: this.quiet);
				}
			}
		}
	}
}
