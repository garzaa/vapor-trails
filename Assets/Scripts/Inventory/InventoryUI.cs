using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class InventoryUI : CloseableUI {
    public Animator animator;
    
    public GameObject itemPaneTemplate;
    public Transform gridHolder;

    public Image itemImage;
    public Text itemTitle;
    public Text itemDescription;
    public Text itemCost;
    public ScrollRect scrollView;
    public AudioSource audioSource;
    public Image merchantPortrait;
    public Text merchantName;
    public Text merchantLine;

    RectTransform gridRect;

    void Start() {
        animator = GetComponent<Animator>();
        gridRect = gridHolder.GetComponent<RectTransform>();
    }

    public void Show() {
        animator.SetBool("Shown", true);
        itemImage.color = new Color(1, 1, 1, 0);
        itemTitle.text = "";
        itemDescription.text = "";
        itemCost.text = "";
        base.Open();
        SelectFirstChild();
    }

    public void Hide() {
        animator.SetBool("Shown", false);
        base.Close();
    }

    void SelectFirstChild() {
        if (gridHolder.childCount == 0) {
            return;
        }
        Button b = gridHolder.GetChild(0).GetComponent<Button>();
        b.Select();
        b.OnSelect(null);
        ReactToItemHover(b.GetComponent<ItemPane>());
        scrollView.content.localPosition = Vector2.zero;
    }

    public void ReactToItemHover(ItemPane itemPane) {
        audioSource.PlayOneShot(audioSource.clip);
        scrollView.content.localPosition = scrollView.GetSnapToPositionToBringChildIntoView(itemPane.GetComponent<RectTransform>());
        ShowItemInfo(itemPane.inventoryItem);
    }

    void ShowItemInfo(Item item) {
        itemImage.color = new Color(1, 1, 1, 1);
        itemImage.sprite = item.detailedIcon;
        itemTitle.text = item.name.ToUpper();
        itemDescription.text = item.GetDescription();
        itemCost.text = "$"+item.cost.ToString();
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
            oldItem.parent = null;
        }
        for (int i=inventoryList.items.Count-1; i>=0; i--) {
            Item item = inventoryList.items[i];
            GameObject g = (GameObject) Instantiate(itemPaneTemplate);
            g.transform.parent = gridHolder;
            g.GetComponent<ItemPane>().PopulateSelfInfo(item);
        }
        SelectFirstChild();
    }

    public void PropagateMerchantInfo(Merchant merchant) {
        merchantPortrait.sprite = merchant.merchantPortrait;
        merchantName.text = merchant.merchantName;
        merchantLine.text = merchant.greetingDialogue;
    }

}