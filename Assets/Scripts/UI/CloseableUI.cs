using UnityEngine;
using System.Collections;

public class CloseableUI : MonoBehaviour {
    
    [SerializeField] bool invincibleDuring;
    [SerializeField] GameObject targetUI;
    [SerializeField] bool interactSound = true;
    [SerializeField] bool stopTime = false;
    [SerializeField] bool exclusive = true;
    [SerializeField] bool closeOnGenericEscape = false;
    [SerializeField] bool useSelf = false;
    [SerializeField] bool closeAtStart = false;

    protected bool open;
    protected bool started;

    virtual public void Open() {
        if ((exclusive && GlobalController.openUIs > 0)) {
            return;
        }
        if (!open) GlobalController.openUIs += 1;
        this.open = true;
        Hitstop.Interrupt();
        if (interactSound) SoundManager.InteractSound();
        GlobalController.pc.EnterCutscene(invincible:invincibleDuring);
        if (targetUI != null) targetUI.SetActive(true);
        if (stopTime) Time.timeScale = 0f;
    }

    virtual public void Close() {
        if (stopTime) Time.timeScale = 1f;
        if (open) GlobalController.openUIs -= 1;
        this.open = false;
        if (GlobalController.openUIs == 0) GlobalController.pc.ExitCutscene();
        if (targetUI != null) targetUI.SetActive(false);
    }

    void Start() {
        started = true;
        OnEnable();
        gameObject.SetActive(false);
    }

    void OnEnable() {
        if (useSelf && started) {
            Open();
        }
    }
    
    void OnDisable() {
        if (useSelf) {
            Close();
        }
    }

    void Update() {
        if (open && closeOnGenericEscape && InputManager.GenericEscapeInput()) {
            StartCoroutine(WaitAndClose());
        }
    }

    // again, deal with input frame timing
    IEnumerator WaitAndClose() {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
}