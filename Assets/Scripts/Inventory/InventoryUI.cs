using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : UIComponent {
    Animator animator;
    InventoryItem currentlySelectedItem;
    InventoryController inventoryController;
    
    public GameObject itemPaneTemplate;
    public Transform gridHolder;

    public Image itemImage;
    public Text itemTitle;
    public Text itemDescription;
    public Text itemCost;
    public ScrollRect scrollView;

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

    public void ReactToItemHover(ItemPane itemPane) {
        scrollView.content.localPosition = scrollView.GetSnapToPositionToBringChildIntoView(itemPane.GetComponent<RectTransform>());
        ShowItemInfo(itemPane.inventoryItem);
    }

    void Update() {
        if (animator.GetBool("Shown")) {
            if (Input.GetButtonDown("Jump")) {
                inventoryController.ReactToItemSelect(currentlySelectedItem);
            }
        }
    }

    void ShowItemInfo(InventoryItem item) {
        itemImage.sprite = item.detailedIcon;
        itemTitle.text = item.itemName;
        itemDescription.text = item.itemDescription;
    }

    public void PopulateItems(InventoryList inventoryList) {
        foreach (Transform oldItem in gridHolder.transform) {
            GameObject.Destroy(oldItem.gameObject);
        }
        foreach (InventoryItem item in inventoryList.items) {
            GameObject g = (GameObject) Instantiate(itemPaneTemplate);
            g.transform.parent = gridHolder;
            g.GetComponent<ItemPane>().PopulateSelfInfo(item);
        }
    }

}