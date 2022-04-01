using UnityEngine;
using Rewired;
using Cinemachine;

public class CameraShaker : MonoBehaviour
{
	public static float shakeAmount = 0.1f;
	public static float decreaseFactor = 1.0f;
	
	static Vector3 originalPos;
	static Player rewiredPlayer;

	static CameraShaker cs;

	public CinemachineImpulseSource tinyShake;
	public CinemachineImpulseSource smallShake;
	public CinemachineImpulseSource mediumShake;
	public CinemachineImpulseSource bigShake;

	void Awake() {
		cs = this;
		rewiredPlayer = ReInput.players.GetPlayer(0);
	}

	public static void BigShake() {
		Shake(cs.bigShake);
	}

	public static void MediumShake() {
		Shake(cs.mediumShake);
	}

	public static void SmallShake() {
		Shake(cs.smallShake);
	}

	public static void TinyShake() {
		Shake(cs.tinyShake);
	}

	static void Shake(CinemachineImpulseSource source) {
		if (SaveManager.save.options.cameraShake) {
			return;
		}
		source.GenerateImpulse();
		rewiredPlayer.StopVibration();
		rewiredPlayer.SetVibration(0, shakeAmount*2f, source.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime);
	}

	public static void StartShaking() {
		throw new System.NotImplementedException();
	}

	public static void StopShaking() {
		rewiredPlayer.StopVibration();
		// throw new System.NotImplementedException();
	}
}
