using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemPane : MonoBehaviour, ISelectHandler {
    InventoryUI inventoryUI;

    void Start() {
        inventoryUI = GetComponentInParent<InventoryUI>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        inventoryUI.ReactToItemHover(this.GetComponent<RectTransform>());
    }

}