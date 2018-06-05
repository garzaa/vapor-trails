using UnityEngine;

public class Sign : PlayerTriggeredObject {

	public string signText;
	public Vector2 signOffset;

	public override void OnPlayerEnter() {
		GlobalController.OpenSign(this.signText, this.transform.position + new Vector3(signOffset.x, signOffset.y));
	}

	public override void OnPlayerExit() {
		GlobalController.CloseSign();
	}
}
