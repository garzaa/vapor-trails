using UnityEngine;
using UnityEngine.EventSystems;

public class PauseUI : CloseableUI {
    Animator animator;
    EventSystem eventSystem;
    GameObject firstSelected;

    void Start() {
        animator = GetComponent<Animator>();
        eventSystem = GetComponentInParent<EventSystem>();
        firstSelected = transform.GetChild(0).gameObject;
    }

    override public void Open() {
        base.Open();
        animator.SetBool("Shown", true);
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    override public void Close() {
        base.Close();
        animator.SetBool("Shown", false);
    }
}