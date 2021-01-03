using UnityEngine;

public class Spinner : SimpleAnimator {
    public float speed;
    public bool unscaled;

    override protected void Draw() {
        Vector3 r = transform.rotation.eulerAngles;
        r.z = (r.z + (speed * (unscaled ? Time.unscaledDeltaTime : Time.deltaTime))) % 360;
        transform.rotation = Quaternion.Euler(r);
    }
}