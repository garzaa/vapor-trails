using UnityEngine;

public class Spinner : SimpleAnimator {
    public float speed;

    override protected void Draw() {
        Vector3 r = transform.rotation.eulerAngles;
        r.z = (r.z + (speed * Time.deltaTime)) % 360;
        transform.rotation = Quaternion.Euler(r);
    }
}