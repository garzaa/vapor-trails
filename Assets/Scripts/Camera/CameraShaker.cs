using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
	public static Transform camTransform;
	
	// How long the object should shake for.
	public static float shakeDuration = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public static float shakeAmount = 0.1f;
	public static float decreaseFactor = 1.0f;
	
	static Vector3 originalPos;

	public static CameraShaker instance;

	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
		instance = this;
	}
	
	void OnEnable()
	{
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
	}

	void Update()
	{
		if (shakeDuration > 0)
		{
			camTransform.localPosition = originalPos + OnUnitCircle() * shakeAmount;
			
			shakeDuration -= Time.unscaledDeltaTime * decreaseFactor;
		}
		else
		{
			shakeDuration = 0f;
			camTransform.localPosition = originalPos;
		}
	}

	Vector3 OnUnitCircle() {
		float randomAngle = Random.Range(0f, Mathf.PI * 2f);
		return (Vector3) new Vector2(Mathf.Sin(randomAngle), Mathf.Cos(randomAngle)).normalized;
	}
}