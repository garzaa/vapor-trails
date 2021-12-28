using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGrabber : PlayerTriggeredObject {

	public GameObject targetPoint;
	CinemachineInterface cinemachineInterface;

	new void Start() {
		base.Start();
		cinemachineInterface = GameObject.FindObjectOfType<CinemachineInterface>();
	}

	public override void OnPlayerEnter() {
		cinemachineInterface.LookAtPoint(targetPoint.transform);
	}

	public override void OnPlayerExit() {
		cinemachineInterface.StopLookingAtPoint(targetPoint.transform);
	}
}
