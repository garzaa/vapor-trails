using UnityEngine;
using UnityEngine.UI;

public class ControllerText : MonoBehaviour {
    string originalText;

    void OnEnable() {
        Text t = GetComponentInChildren<Text>();
        if (string.IsNullOrEmpty(originalText)) originalText = t.text;
        t.text = ControllerTextChanger.ReplaceText(originalText);
    }
}