using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PauseUI : CloseableUI {
    Animator animator;
    EventSystem eventSystem;
    GameObject firstSelected;

    void Start() {
        animator = GetComponent<Animator>();
        eventSystem = GetComponentInChildren<EventSystem>();
        firstSelected = transform.GetChild(0).gameObject;
    }

    override public void Open() {
        base.Open();
        animator.SetBool("Shown", true);
        eventSystem.SetSelectedGameObject(firstSelected);
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
        if (open && Input.GetButtonDown("Start")) {
            Close();
        }
    }
}