using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ItemViewer : MonoBehaviour {
    public GameObject itemPaneTemplate;
    public Transform gridHolder;

    public Image itemImage;
    public Text itemTitle;
    public Text itemDescription;
    public ScrollRect scrollView;
    public AudioClip selectSound;

    [SerializeField] InventoryList editorInventoryLink;

    bool started = false;

    void OnEnable() {
        if (!started) return;
        itemImage.color = new Color(1, 1, 1, 0);
        itemTitle.text = "";
        itemDescription.text = "";

        if (editorInventoryLink != null) {
            PopulateItems(editorInventoryLink);
        }
    }

    void Start() {
        started = true;
        OnEnable();
    }

    public void ReactToItemHover(ItemPane itemPane) {
        if (selectSound != null) SoundManager.PlaySound(selectSound);
        scrollView.content.localPosition = new Vector2(
            scrollView.content.localPosition.x,
            scrollView.GetSnapToPositionToBringChildIntoView(itemPane.GetComponent<RectTransform>()).y
        );
        ShowItemInfo(itemPane.inventoryItem);
    }

    void ShowItemInfo(Item item) {
        itemImage.color = new Color(1, 1, 1, 1);
        itemImage.sprite = item.detailedIcon;
        itemTitle.text = item.name.ToUpper();
        if (item.count > 1) {
            itemTitle.text += ": " + item.count;
        }
        itemDescription.text = item.description;
        //itemCost.text = "$"+item.cost.ToString();
        if (item.type.Contains(ItemType.ABILITY)) {
            itemDescription.text += 
                "\n\n<color=white>"
                + ControllerTextChanger.ReplaceText(((AbilityItem) item).instructions)
                + "</color>";
        }
    }

    public void PopulateItems(InventoryList inventoryList) {
        // don't want to modify the list in place, instead copy and iterate through that
        // it Just Works
        foreach (Transform oldItem in gridHolder.transform.Cast<Transform>().ToArray()) {
            // Destroy is called after the Update loop, which screws up the first child selection logic
            // so we do this so it's not shown
            Destroy(oldItem.gameObject);
            oldItem.SetParent(null, false);
        }
        for (int i=inventoryList.items.Count-1; i>=0; i--) {
            Item item = inventoryList.items[i];
            GameObject g = Instantiate(
                itemPaneTemplate,
                Vector2.zero,
                Quaternion.identity,
                gridHolder
            );
            g.GetComponent<ItemPane>().PopulateSelfInfo(item);
        }
        gridHolder.GetComponent<SelectFirstChild>().OnEnable();
    }
}