using UnityEngine;
using Rewired;

public class CameraShaker : MonoBehaviour
{
	public static Transform camTransform;
	
	public static float shakeDuration = 0f;
	static bool shaking = false;
	
	public static float shakeAmount = 0.1f;
	public static float decreaseFactor = 1.0f;
	
	static Vector3 originalPos;
	static Player rewiredPlayer;

	void Awake() {
		if (camTransform == null) {
			camTransform = this.transform;
			rewiredPlayer = ReInput.players.GetPlayer(0);
		}

	}
	
	void OnEnable() {
		originalPos = camTransform.localPosition;
	}

	public static void BigShake() {
		Shake(0.5f, 0.5f);
	}

	public static void MedShake() {
		Shake(0.05f, 0.1f);
	}

	public static void SmallShake() {
		Shake(0.1f, 0.05f);
	}

	public static void TinyShake() {
		Shake(0.05f, 0.05f);
	}

	public static void Shake(float amount, float duration) {
		shakeAmount = amount;
		shakeDuration = duration;
		
		rewiredPlayer.StopVibration();
		rewiredPlayer.SetVibration(0, amount*2f, duration);
	}

	void Update() {
		if (shakeDuration > 0 || shaking) {
			camTransform.localPosition = originalPos + OnUnitCircle()*shakeAmount;
			shakeDuration -= Time.unscaledDeltaTime * decreaseFactor;
		} else {
			shakeDuration = 0f;
			camTransform.localPosition = originalPos;
		}
	}

	Vector3 OnUnitCircle() {
		float randomAngle = Random.Range(0f, Mathf.PI * 2f);
		return (Vector3) new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle));
	}

	public static void StartShaking() {
		shaking = true;
	}

	public static void StopShaking() {
		rewiredPlayer.StopVibration();
		shaking = false;
		shakeDuration = 0;
	}
}