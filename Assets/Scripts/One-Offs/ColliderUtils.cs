using UnityEngine;

public static class ColliderUtils {
    public static Vector2 BottomLeftCorner(this BoxCollider2D bc2d) {
        Vector2 h = ((Vector2) bc2d.transform.position + bc2d.offset) + new Vector2(
            (-bc2d.size.x / 2) * bc2d.transform.localScale.x,
            -bc2d.size.y / 2
        );
        return h;
    }

    public static Vector2 BottomRightCorner(this BoxCollider2D bc2d) {
        Vector2 h = ((Vector2) bc2d.transform.position + bc2d.offset) + new Vector2(
            (bc2d.size.x / 2) * bc2d.transform.localScale.x,
            -bc2d.size.y / 2
        );
        return h;
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) {
        return Quaternion.Euler(0, 0, degrees) * v;
     }
}
