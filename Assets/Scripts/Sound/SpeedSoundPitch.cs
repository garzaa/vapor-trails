using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SpeedSoundPitch : MonoBehaviour {
    public Rigidbody2D target;
    public float targetSpeed;
    public float targetVolume = 1;

    AudioSource audioSource;
    
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        audioSource.pitch = (target.velocity.magnitude/targetSpeed);
        audioSource.volume = Mathf.Clamp(target.velocity.magnitude/targetSpeed, 0f, 1f);
    }
}