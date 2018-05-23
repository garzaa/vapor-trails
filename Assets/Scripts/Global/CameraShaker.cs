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
		shakeAmount = 0.1f;
		shakeDuration = 0.1f;
	}

	public static void MedShake() {
		shakeAmount = 0.05f;
		shakeDuration = 0.1f;
	}

	public static void SmallShake() {
		shakeAmount = 0.01f;
		shakeDuration = 0.1f;
	}

	void Update()
	{
		if (shakeDuration > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			
			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shakeDuration = 0f;
			camTransform.localPosition = originalPos;
		}
	}
}