using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class SlowRenderer : MonoBehaviour {
    [TextArea]
    public string text;

    string textLastFrame;
    Coroutine renderRoutine;

    Text uiText;
    int letterIndex = 0;
    char[] pauses = {'.', '!', ',', '?', '\n'};
    public float letterDelay = 0.01f;

    void OnEnable() {
        uiText = GetComponent<Text>();
        textLastFrame = text;
        StartRendering();
    }

    void Update() {
        if (text != textLastFrame) {
            StartRendering();
        }
        textLastFrame = text;
    }

    void StartRendering() {
        if (renderRoutine != null) StopCoroutine(renderRoutine);
        uiText.text = "";
        letterIndex = 0;
        renderRoutine = StartCoroutine(SlowRender());
    }
    
	IEnumerator SlowRender() {
		//then call self again to render the next letter
		if (letterIndex < text.Length) {
			uiText.text = text.Substring(0, letterIndex+1); // + MakeInvisibleText();
			if (char.IsLetter(text[letterIndex])) {
				SoundManager.VoiceSound(0);
			}
			int scalar = 1;
			if (IsPause(text[letterIndex])) {
				scalar = 7;
			}
			yield return new WaitForSecondsRealtime(letterDelay * scalar);
			letterIndex++;
			StartCoroutine(SlowRender());
		}
	}

    bool IsPause(char c) {
		return pauses.Contains(c);
	}

	string MakeInvisibleText() {
		string invisText = text.Substring(letterIndex+1);
		invisText = "<color=#00000000>" + invisText + "</color>";
		return invisText;
	}
}