using UnityEngine;

public class ColliderDebug : MonoBehaviour {
	Collider2D col;

	void Start() {
		col = GetComponent<Collider2D>();
	}

	void Update() {
		Debug.DrawLine(col.bounds.center, col.bounds.min, Color.green);
		Debug.DrawLine(col.bounds.center, col.BottomLeftCorner(), Color.cyan);
	}
}
