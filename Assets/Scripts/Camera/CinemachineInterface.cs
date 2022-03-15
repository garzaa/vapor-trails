using UnityEngine;
using Cinemachine;

public class CinemachineInterface : MonoBehaviour {
	CinemachineBrain cinemachine;

	#pragma warning disable 0649
	[SerializeField] CinemachineVirtualCamera mainCam;
	[SerializeField] CinemachineVirtualCamera worldLookCam;
	#pragma warning restore 0649

	void OnEnable() {
		cinemachine = GetComponent<CinemachineBrain>();
	}

	public void LookAtPoint(Transform target) {
		worldLookCam.m_Follow = target;
		worldLookCam.m_Priority = 20;
	}

	public void StopLookingAtPoint(Transform target) {
		if (worldLookCam.m_Follow != target) {
			return;
		}
		worldLookCam.m_Priority = 0;
		worldLookCam.m_Follow = null;
	}
}
