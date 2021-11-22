using UnityEngine;

public class ParticleCollisionSound : MonoBehaviour {
    public AudioClip collisionNoise;
    public GameObject hitmarker;

    void OnParticleCollision(GameObject other) {
        if (Vector2.Distance(other.transform.position, GlobalController.pc.transform.position) < 4f) {
            SoundManager.WorldSound(collisionNoise);
            if (hitmarker != null ) Instantiate(hitmarker, other.transform.position, Quaternion.identity, null);
        }
    }
}
