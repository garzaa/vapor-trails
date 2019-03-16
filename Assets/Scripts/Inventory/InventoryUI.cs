using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : UIComponent {
    Animator animator;
    InventoryItem currentlySelectedItem;
    InventoryController inventoryController;

    public Image itemImage;
    public Text itemTitle;
    public Text itemDescription;
    
    void Start() {
        animator = GetComponent<Animator>();
    }

    public override void Show() {
        animator.SetBool("Shown", true);
    }

    public override void Hide() {
        animator.SetBool("Shown", false);
        currentlySelectedItem = null;
    }

    void Update() {
        if (animator.GetBool("Shown")) {
            if (Input.GetButtonDown("Jump")) {
                inventoryController.ReactToItemSelect(currentlySelectedItem);
            }
        }
    }

}