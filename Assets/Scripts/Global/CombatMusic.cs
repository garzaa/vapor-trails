using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CombatMusic : MonoBehaviour {
    
    static float combatCooldown = 5f;
    static float fadeInTime = 1f;
    static float fadeOutTime = 10f;
    static CombatMusic cm;
    static List<AudioFade> musics;

    void OnEnable() {
        cm = this;
        musics = GameObject.FindObjectsOfType<AudioFade>().Where(x => x.combatMusic).ToList();
    }

    public static void EnterCombat() {
        cm.CancelInvoke("EndCombatCooldown");
        cm.Invoke("EndCombatCooldown", combatCooldown);
        foreach (AudioFade music in musics) {
            music.FadeIn(fadeInTime);
        }
    }

    void EndCombatCooldown() {
        foreach (AudioFade music in musics) {
            music.FadeOut(fadeOutTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(Tags.EnemyHitbox)) {
            CombatMusic.EnterCombat();
        }
    }

}
