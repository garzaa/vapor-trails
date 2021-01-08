using UnityEngine;

public class TopScrollVerticalLayout : MonoBehaviour {
    void OnEnable() {
        RectTransform r = GetComponent<RectTransform>();
        r.position = new Vector2(r.position.x, -9999);
    }
}
    