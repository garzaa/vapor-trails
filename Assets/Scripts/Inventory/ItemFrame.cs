using UnityEngine;
using UnityEngine.UI;

public class ItemFrame : MonoBehaviour {
    [SerializeField]
    Item targetItem;

    [SerializeField] Image itemImage;

    Button button;
    bool initialized = false;

    void Start() {
        itemImage.sprite = targetItem.itemIcon;
        button = GetComponent<Button>();
        initialized = true;
    }
    
    void OnEnable() {
        if (!initialized) Start();
        if (GlobalController.inventory.items.HasItem(targetItem)) {
            itemImage.enabled = true;
            button.interactable = true;
        } else {
            itemImage.enabled = false;
            button.interactable = false;
        }
    }
}