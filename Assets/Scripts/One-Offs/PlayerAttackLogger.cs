using UnityEngine;

public class PlayerAttackLogger : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        PlayerAttack a = other.GetComponent<PlayerAttack>();
        if (a != null) {
            AlerterText.Alert(a.attackName);
        }
    }
}