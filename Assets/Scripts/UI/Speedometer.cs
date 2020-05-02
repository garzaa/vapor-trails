using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour {
    
    public Rigidbody2D rb2d;
    public Text speedText;

    float magnitude;

    void FixedUpdate() {
        magnitude = unitsToMPH(rb2d.velocity.magnitude);
        speedText.text = magnitude.ToString("0.000");
    }

    float unitsToMPH(float n) {
        // each unit is 3 feet
        // 5280 feet in a mile
        // 3600 minutes in an hour
        return n * 3 / 5280 * 3600;
    }
}