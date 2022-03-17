using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class ItemViewer : MonoBehaviour {
    public GameObject itemPaneTemplate;
    public Transform gridHolder;

    public Image itemImage;
    public Text itemTitle;
    public Text itemDescription;
    public Text itemCost;
    public ScrollRect scrollView;
    public AudioClip selectSound;

    #pragma warning disable 0649
    [SerializeField] InventoryController editorInventoryLink;
	#pragma warning restore 0649

    bool started = false;

    void OnEnable() {
        if (!started) return;
        itemImage.color = new Color(1, 1, 1, 0);
        itemTitle.text = "";
        itemDescription.text = "";

        if (editorInventoryLink != null) {
            PopulateItems(editorInventoryLink.items.GetAll());
        }
    }

    void Start() {
        started = true;
        OnEnable();
    }

    public void ReactToItemHover(ItemPane itemPane) {
        if (selectSound != null) SoundManager.UISound(selectSound);
        scrollView.content.localPosition = new Vector2(
            scrollView.content.localPosition.x,
            scrollView.GetSnapToPositionToBringChildIntoView(itemPane.GetComponent<RectTransform>()).y
        );
        ShowItemInfo(itemPane.storedItem);
    }

    void ShowItemInfo(StoredItem s) {
        Item item = s.item;
        itemImage.color = new Color(1, 1, 1, 1);
        itemImage.sprite = item.detailedIcon;
        itemTitle.text = item.name.ToUpper();
        if (s.count > 1) {
            itemTitle.text += ": " + s.count;
        }
        if (itemCost != null) itemCost.text = "$"+item.cost.ToString();
        
        string itemText = item.GetDescription();
        if (item.IsType(ItemType.WEAPON)) {
            itemText += "\n\n<color=lime>ATTACK FORM</color>";
        }

        itemDescription.text = itemText;
    }

    public void PopulateItems(List<StoredItem> items) {
        // don't want to modify the list in place, instead copy and iterate through that
        // it Just Works
        foreach (Transform oldItem in gridHolder.transform.Cast<Transform>().ToArray()) {
            // Destroy is called after the Update loop, which screws up the first child selection logic
            // so we do this so it's not shown
            Destroy(oldItem.gameObject);
            oldItem.SetParent(null, false);
        }
        
        for (int i=items.Count-1; i>=0; i--) {
            StoredItem storedItem = items[i];
            GameObject g = Instantiate(
                itemPaneTemplate,
                Vector2.zero,
                Quaternion.identity,
                gridHolder
            );
            g.GetComponent<ItemPane>().PopulateSelfInfo(storedItem);
        }
        gridHolder.GetComponent<SelectFirstChild>().OnEnable();
    }
}
