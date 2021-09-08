using UnityEngine;

public class Spinner : SimpleAnimator {
    public float speed;
    public bool unscaled;

    public float updateInterval = 0;
    
    float lastUpdate = 0f;

    override protected void Draw() {
        float t = unscaled ? Time.unscaledTime : Time.time;

        if (t > lastUpdate+updateInterval) {
            Vector3 r = transform.localRotation.eulerAngles;
            r.z = ((speed * t)) % 360;
            transform.localRotation = Quaternion.Euler(r);

            lastUpdate = t;
        }
    }
}
