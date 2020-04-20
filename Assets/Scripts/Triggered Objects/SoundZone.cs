using UnityEngine;

public class SoundZone : PlayerTriggeredObject {
    public AudioFade audioFade;
    float fadeSeconds = 2f;

    override public void OnPlayerEnter() {
        audioFade.FadeIn(fadeSeconds);
    }

    override public void OnPlayerExit() {
        audioFade.FadeOut(fadeSeconds);
    }
}