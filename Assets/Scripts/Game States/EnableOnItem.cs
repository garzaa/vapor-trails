using UnityEngine;
using UnityEngine.Serialization;

public class EnableOnItem : MonoBehaviour {
    [SerializeField] ItemWrapper wantedItem;
    [SerializeField] Item wanted;
    public bool immediate = true;

    public bool setDisabled = false;

    void Start() {
        CheckState();
    }

    public void CheckState() {
        Item i = GlobalController.inventory.items.GetItem(wanted);
        bool hasItem = (i != null && i.count >= wantedItem.item.count);
        if (setDisabled) gameObject.SetActive(!hasItem);
        else gameObject.SetActive(hasItem);
    }
}