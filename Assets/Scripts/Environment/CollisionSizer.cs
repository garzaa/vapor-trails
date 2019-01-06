using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CollisionSizer : MonoBehaviour {
	public Vector2 positionoffset;
	public Vector2 radiusOffset;
	public bool pinToTop;
	void Awake() {
		BoxCollider2D bc = GetComponent<BoxCollider2D>();
		SpriteRenderer spr = GetComponent<SpriteRenderer>();
		bc.offset += positionoffset;
		bc.size = spr.size + radiusOffset;
		if (pinToTop) {
			bc.offset = new Vector2(
				bc.offset.x,
				spr.size.y / 2
			);
		}
	}
}
