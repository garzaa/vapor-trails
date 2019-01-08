
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityUnlocker : Activatable {

	public string abilityName;
	[TextArea]
	public string description;
	public InventoryItem item;

	public override void ActivateSwitch(bool b) {
		
	}

}
