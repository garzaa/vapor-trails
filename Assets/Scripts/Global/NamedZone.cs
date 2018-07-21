using UnityEngine;

public class NamedZone : PlayerTriggeredObject {

	public string title;
	public string subtitle;

	public override void OnPlayerEnter() {
		GlobalController.ShowTitleText(title, subtitle);		
	}

	public override void OnPlayerExit() {
		
	}
}
