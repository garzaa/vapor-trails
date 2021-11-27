using UnityEngine;

[CreateAssetMenu(fileName="GradientContainer", menuName="Data Containers/Gradient Container")]
public class GradientContainer : ScriptableObject {
	#pragma warning disable 0649
	[SerializeField] Gradient gradient;
	#pragma warning restore 0649

	public Color Evaluate(float t) {
		return gradient.Evaluate(t);
	}
}
