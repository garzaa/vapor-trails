using UnityEngine;

public class CloseableUI : MonoBehaviour {
    
    [SerializeField] bool invincibleDuring;
    [SerializeField] GameObject targetUI;
    [SerializeField] bool interactSound = true;
    [SerializeField] bool stopTime = false;
    [SerializeField] bool exclusive = true;
    protected bool open;

    virtual public void Open() {
        if (exclusive && GlobalController.openUIs > 0) {
            return;
        }
        GlobalController.openUIs += 1;
        this.open = true;
        Hitstop.Interrupt();
        if (interactSound) SoundManager.InteractSound();
        GlobalController.pc.EnterCutscene(invincible:invincibleDuring);
        if (targetUI != null) targetUI.SetActive(true);
        if (stopTime) Time.timeScale = 0f;
    }

    virtual public void Close() {
        this.open = false;
        if (stopTime) Time.timeScale = 1f;
        GlobalController.openUIs -= 1;
        Debug.Log(GlobalController.openUIs);
        if (GlobalController.openUIs == 0) GlobalController.pc.ExitCutscene();
        if (targetUI != null) targetUI.SetActive(false);
    }
}