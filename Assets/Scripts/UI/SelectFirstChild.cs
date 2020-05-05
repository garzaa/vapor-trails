using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectFirstChild : MonoBehaviour {
    void OnEnable() {
        StartCoroutine(SelectChild());
    }

    IEnumerator SelectChild() {
        yield return new WaitForEndOfFrame();
        Debug.Log("benis");
        Button b = GetComponentInChildren<Button>();
        b.Select();
        b.OnSelect(null);
    }
}