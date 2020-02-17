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
        if (cm == null) cm = this;
        SceneManager.sceneLoaded += OnSceneLoaded;
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

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        musics = GameObject.FindObjectsOfType<AudioFade>().Where(x => x.combatMusic).ToList();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag(Tags.EnemyHitbox)) {
            CombatMusic.EnterCombat();
        }
    }

}