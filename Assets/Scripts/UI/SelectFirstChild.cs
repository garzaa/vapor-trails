using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectFirstChild : MonoBehaviour {
    public void OnEnable() {
        StartCoroutine(SelectChild());
    }

    IEnumerator SelectChild() {
        yield return new WaitForEndOfFrame();
        Selectable s = GetComponentInChildren<Selectable>();
        s.Select();
        s.OnSelect(null);
    }
}
