using UnityEngine;
using Cinemachine;

public class PlayerCameraTargetGroup : MonoBehaviour {
	CinemachineTargetGroup tg;

	void Start() {
		tg = GetComponent<CinemachineTargetGroup>();
	}

	public void AddTarget(Object o) {
		tg.AddMember((o as GameObject).transform, 0.5f, 0);
	}

	public void RemoveTarget(Object o) {
		tg.RemoveMember((o as GameObject).transform);
	}
}
