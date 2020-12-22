using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossFightIntro : MonoBehaviour {
    public Text bossName;
    public Image bossFightImage;

    bool canSkip = false;

    public static void ShowIntro(BossInfo info) {
        GlobalController.bossFightIntro.m_ShowIntro(info);
    }

    void m_ShowIntro(BossInfo info) {
        canSkip = false;
        bossName.text = info.bossName;
        bossFightImage.sprite = info.bossFightImage;
        gameObject.SetActive(true);
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
    }

    IEnumerator CloseUI() {
        yield return new WaitForEndOfFrame();
        GetComponent<Animator>().SetTrigger("Close");
    }
}
