using UnityEngine;

public class SoundZone : PlayerTriggeredObject {
    public AudioFade audioFade;
    float fadeSeconds = 2f;
    
    override protected void Start() {
        base.Start();
        audioFade.FadeOut(0.01f);
    }

    override public void OnPlayerEnter() {
        audioFade.FadeIn(fadeSeconds);
    }

    override public void OnPlayerExit() {
        audioFade.FadeOut(fadeSeconds);
    }
}
