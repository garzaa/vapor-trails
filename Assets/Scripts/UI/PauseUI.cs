using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseUI : CloseableUI {
    Animator animator;
    GameObject firstSelected;

    void Start() {
        animator = GetComponent<Animator>();
        firstSelected = transform.GetChild(0).gameObject;
    }

    override public void Open() {
        base.Open();
        animator.SetBool("Shown", true);
        StartCoroutine(SelectChild());
    }

    override public void Close() {
        StartCoroutine(_Close());
    }

    public IEnumerator _Close() {
        yield return new WaitForEndOfFrame();
        base.Close();
        GlobalController.Unpause();
        animator.SetBool("Shown", false);
    }

    void Update() {
        if (open && InputManager.ButtonDown(Buttons.PAUSE)) {
            Close();
        }
    }

    IEnumerator SelectChild() {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Button b = GetComponentInChildren<Button>();
        b.Select();
        b.OnSelect(null);
    }
}