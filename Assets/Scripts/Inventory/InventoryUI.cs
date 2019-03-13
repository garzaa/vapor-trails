using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : UIComponent {
    Animator animator;
    
    public override void Show() {
        animator.SetTrigger("Show");
    }

    public override void Hide() {
        animator.SetTrigger("Hide");
    }

    void FixedUpdate() {

    }
}