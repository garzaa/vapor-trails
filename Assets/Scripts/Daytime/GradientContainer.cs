using UnityEngine;

[CreateAssetMenu(fileName="GradientContainer", menuName="Data Containers/Gradient Container")]
public class GradientContainer : ScriptableObject {
	[SerializeField] Gradient gradient;

	public Color Evaluate(float t) {
		return gradient.Evaluate(t);
	}
}
