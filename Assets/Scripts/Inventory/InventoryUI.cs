using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : UIComponent {
    public Animator animator;
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
        SelectFirstChild();
    }

    public override void Hide() {
        animator.SetBool("Shown", false);
        currentlySelectedItem = null;
    }

    void SelectFirstChild() {
        Button b = gridHolder.GetChild(0).GetComponent<Button>();
        b.Select();
        b.OnSelect(null);
        ReactToItemHover(b.GetComponent<ItemPane>());
    }

    public void ReactToItemHover(ItemPane itemPane) {
        scrollView.content.localPosition = scrollView.GetSnapToPositionToBringChildIntoView(itemPane.GetComponent<RectTransform>());
        ShowItemInfo(itemPane.inventoryItem);
    }

    void Update() {
        if (animator.GetBool("Shown")) {
            if (Input.GetButtonDown("Jump") && currentlySelectedItem != null) {
                inventoryController.ReactToItemSelect(currentlySelectedItem);
            }
        }
    }

    void ShowItemInfo(InventoryItem item) {
        itemImage.sprite = item.detailedIcon;
        itemTitle.text = item.itemName.ToUpper();
        itemDescription.text = item.itemDescription;
        if (item.IsAbility()) {
            itemDescription.text += "\n\n" + ControllerTextChanger.ReplaceText(((AbilityItem) item).instructions);
        }
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

        SetGridHeight(gridRect, inventoryList.items.Count, NUM_COLUMNS);
    }

    public void SetGridHeight(RectTransform g, int itemCount, int numColumns) {
        Vector2 s = g.sizeDelta; 
        GridLayoutGroup grid = g.GetComponent<GridLayoutGroup>();

        int numRows = Mathf.Max(itemCount / numColumns, 1);
        //max with the height of the viewport
        s.y = Mathf.Max(
            grid.padding.top + grid.padding.bottom
            + (numRows * (int)grid.cellSize.y)
            // muh fencepost error
            + ((numRows-1) * grid.spacing.y)
        , 261);

        g.sizeDelta = s;
    }

}