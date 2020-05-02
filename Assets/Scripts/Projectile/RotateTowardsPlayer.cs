using UnityEngine;

public class RotateTowardsPlayer : MonoBehaviour {

    public float maxDelta = 360;

    GameObject player;
    Vector3 r;

    void Start() {
        player = GlobalController.pc.gameObject;
    }

    void Update() {
        var dir = player.transform.position - this.transform.position;
		var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion target = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, maxDelta*Time.deltaTime);
    }
}