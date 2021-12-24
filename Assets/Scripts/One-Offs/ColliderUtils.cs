using UnityEngine;

public static class ColliderUtils {
    public static Vector2 BottomLeftCorner(this Collider2D col) {
        return new Vector2(
			col.bounds.center.x - (col.bounds.extents.x * 0.95f * (col.transform.localScale.x > 0 ? 1 : -1)),
			col.bounds.min.y
		);
    }

    public static Vector2 BottomRightCorner(this Collider2D col) {
        return new Vector2(
			col.bounds.center.x + (col.bounds.extents.x * 0.95f * (col.transform.localScale.x > 0 ? 1 : -1)),
			col.bounds.min.y
		);
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) {
        return Quaternion.Euler(0, 0, degrees) * v;
     }
}
