using UnityEngine;
using UnityEngine.UI;

public class ItemViewer : MonoBehaviour {
    public GameObject itemPaneTemplate;
    public Transform gridHolder;

    public Image itemImage;
    public Text itemTitle;
    public Text itemDescription;
    public Text itemCost;
    public ScrollRect scrollView;
    public AudioClip selectSound;

    public void ReactToItemHover(ItemPane itemPane) {
        SoundManager.PlaySound(selectSound);
        scrollView.content.localPosition = scrollView.GetSnapToPositionToBringChildIntoView(itemPane.GetComponent<RectTransform>());
        ShowItemInfo(itemPane.inventoryItem);
    }

    void ShowItemInfo(Item item) {
        itemImage.color = new Color(1, 1, 1, 1);
        itemImage.sprite = item.detailedIcon;
        itemTitle.text = item.name.ToUpper();
        itemDescription.text = item.description;
        itemCost.text = "$"+item.cost.ToString();
        if (item.type.Contains(ItemType.ABILITY)) {
            itemDescription.text += 
                "\n\n<color=white>"
                + ControllerTextChanger.ReplaceText(((AbilityItem) item).instructions)
                + "</color>";
        }
    }
}