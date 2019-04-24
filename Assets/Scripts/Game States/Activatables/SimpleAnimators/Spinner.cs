using UnityEngine;

public class Spinner : SimpleAnimator {
    public float speed;

    override protected void Draw() {
        Vector3 r = this.transform.rotation.eulerAngles;
        r.z = (r.z + speed) % 360;
        this.transform.rotation = Quaternion.Euler(r);
    }
}