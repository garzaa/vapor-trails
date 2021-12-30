using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossFightIntro : MonoBehaviour {
    public Text bossName;
    public Image bossFightImage;
    public Text playerName;
    public GameObject defaultFightObject;

    const string defaultPlayerName = "Val";
    bool canSkip = false;

    bool introShown = false;
    GameObject customIntro;

    public void ShowIntro(BossInfo info) {
        canSkip = false;
        bossName.text = info.bossName;
        bossFightImage.sprite = info.bossFightImage;
        if (!string.IsNullOrEmpty(info.playerName)) {
            playerName.text = info.playerName;
        } else {
            playerName.text = defaultPlayerName;
        }
        introShown = true;
        defaultFightObject.SetActive(true);
        StartCoroutine(WaitAndEnableSkip());
    }

    public void ShowIntro(GameObject introPrefab) {
        canSkip = false;
        introShown = true;
        Instantiate(introPrefab, this.transform);
    }

    IEnumerator WaitAndEnableSkip() {
        yield return new WaitForSecondsRealtime(0.5f);
        canSkip = true;
    }

    void Update() {
        if (!introShown) return;

        // custom intro has self destructed, exit cutscene
        if (introShown && !defaultFightObject.activeSelf && customIntro==null) {
            StartCoroutine(CloseUI());
        }

        if (canSkip && InputManager.GenericContinueInput()) {
            StartCoroutine(CloseUI());
        }
    }

    IEnumerator CloseUI() {
        introShown = false;
        yield return new WaitForEndOfFrame();
        GetComponentInChildren<Animator>().SetTrigger("Close");
    }
}
