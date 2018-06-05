using UnityEngine;

public class Sign : PlayerTriggeredObject {

	public string signText;

	public override void OnPlayerEnter() {
		GlobalController.OpenSign(this.signText, this.transform.position);
	}

	public override void OnPlayerExit() {
		GlobalController.CloseSign();
	}
}
