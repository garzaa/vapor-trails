using UnityEngine;

public class Teleporter : Activatable {

	public GameObject target;	

	void Start() {
		Instantiate(Resources.Load("DoorIcon"), transform.position, Quaternion.identity, this.transform);
	}

    public override void ActivateSwitch(bool b) {
		if (b) {
			GlobalController.MovePlayerTo(target.transform.position, fade:true);
		}
	}


}


