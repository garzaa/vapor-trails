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

    public int NUM_COLUMNS = 3;
    RectTransform gridRect;

    void Start() {
        animator = GetComponent<Animator>();
        gridRect = gridHolder.GetComponent<RectTransform>();
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

        if (Input.GetButtonDown("Inventory")) {
            animator.SetBool("Shown", !animator.GetBool("Shown"));
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
        //then update the grid container to be as tall as the list of items
        SetGridHeight(gridRect, inventoryList.items.Count, NUM_COLUMNS);
    }

    public void SetGridHeight(RectTransform g, int itemCount, int numColumns) {
        Vector2 s = g.sizeDelta; 
        GridLayoutGroup grid = g.GetComponent<GridLayoutGroup>();

        int numRows = itemCount / numColumns;
        s.y = grid.padding.top + grid.padding.bottom
            + (numRows * (int)grid.cellSize.y * grid.spacing.y)
            // muh fencepost error
            + ((numRows-1) * grid.spacing.y);

        g.sizeDelta = s;
    }

}