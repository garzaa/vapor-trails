using UnityEngine;
using UnityEngine.UI;

public class ControllerText : MonoBehaviour {
    void OnEnable() {
        Text t = GetComponentInChildren<Text>();
        t.text = ControllerTextChanger.ReplaceText(t.text);
    }
}