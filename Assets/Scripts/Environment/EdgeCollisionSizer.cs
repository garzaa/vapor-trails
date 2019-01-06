using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class EdgeCollisionSizer : MonoBehaviour {
	public Vector2 positionoffset;
	public bool pinToTop;
	void Awake() {
		EdgeCollider2D ec = GetComponent<EdgeCollider2D>();
		SpriteRenderer spr = GetComponent<SpriteRenderer>();
		ec.offset += positionoffset;
		if (pinToTop) {
			ec.offset = new Vector2(
				ec.offset.x,
				spr.size.y / 2
			);
		}
		ec.points = new Vector2[] {
			new Vector2(- (spr.size.x/2), 0),
			new Vector2(+ (spr.size.x/2), 0),
		};
	}
}
