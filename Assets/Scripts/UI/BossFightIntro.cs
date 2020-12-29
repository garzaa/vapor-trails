using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossFightIntro : MonoBehaviour {
    public Text bossName;
    public Image bossFightImage;
    public Text playerName;

    const string defaultPlayerName = "Val";

    bool canSkip = false;

    public static void ShowIntro(BossInfo info) {
        GlobalController.bossFightIntro.m_ShowIntro(info);
    }

    void m_ShowIntro(BossInfo info) {
        canSkip = false;
        bossName.text = info.bossName;
        bossFightImage.sprite = info.bossFightImage;
        if (!string.IsNullOrEmpty(info.playerName)) {
            playerName.text = info.playerName;
        } else {
            playerName.text = defaultPlayerName;
        }

        gameObject.SetActive(true);
        GlobalController.pc.EnterCutscene();
        StartCoroutine(WaitAndEnableSkip());
    }

    IEnumerator WaitAndEnableSkip() {
        yield return new WaitForSecondsRealtime(0.5f);
        canSkip = true;
    }

    void Update() {
        if (canSkip && InputManager.GenericContinueInput()) {
            StartCoroutine(CloseUI());
        }
        GlobalController.pc.EnterCutscene();
    }

    IEnumerator CloseUI() {
        yield return new WaitForEndOfFrame();
        GlobalController.pc.ExitCutscene();
        GetComponent<Animator>().SetTrigger("Close");
    }
}
