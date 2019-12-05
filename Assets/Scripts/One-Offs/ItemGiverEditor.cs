using UnityEngine;

[ExecuteInEditMode]
public class ItemGiverEditor : MonoBehaviour {
    public ItemWrapper itemWrapper;

    void OnEnable() {
        if (itemWrapper == null) {
            return;
        }
        // need to update: item giver's item
        // thumbnail for sprite mask, will propagate to link automatically
        GetComponent<ItemGiver>().item = this.itemWrapper;
        GetComponentInChildren<SpriteRenderer>().sprite = this.itemWrapper.item.itemIcon;
    }
}