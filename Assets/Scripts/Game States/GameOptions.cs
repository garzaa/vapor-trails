using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class GameOptions {
    public bool shortHop = true;
    public bool gameJournalist = false;
    public bool slideDrop = false;
    public int inputBuffer = 8;
    public float lookaheadRatio = 1f;
    
    bool fullscreen = true;

    public void Load() {
        shortHop = LoadBool("ShortHop");
        gameJournalist = LoadBool("GameJournalist");
        slideDrop = LoadBool("SlideDrop");
        GlobalController.pc.GetComponent<Animator>().SetBool("LedgeDrop", slideDrop);
        inputBuffer = LoadInt("InputBuffer");
        fullscreen = LoadBool("Fullscreen");
        lookaheadRatio = LoadFloat("LookaheadRatio");
    
        Application.runInBackground = LoadBool("RunInBackground");
        QualitySettings.vSyncCount = LoadInt("VSync");
    
        #if UNITY_STANDALONE
        if (fullscreen) {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        } else {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        #endif
    }

    // player pref changes are done via scripts attached to buttons
    // SettingsSlider and SettingsToggle
    public void Apply() {
        PlayerPrefs.Save();
        Load();
    }

    public static bool LoadBool(string boolName, bool defaultValue = false) {
        return PlayerPrefs.GetInt(boolName, defaultValue ? 1 : 0) == 1;
    }

    static int LoadInt(string intName, int defaultValue = 0) {
        return PlayerPrefs.GetInt(intName, defaultValue);
    }

    static float LoadFloat(string floatName, float defaultValue = 1f) {
        return PlayerPrefs.GetFloat(floatName, defaultValue);
    }
}
