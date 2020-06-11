using UnityEngine;

[ExecuteInEditMode]
public class ItemGiverEditor : MonoBehaviour {
    public Item toGive;

    void OnEnable() {
        if (toGive == null) {
            return;
        }
        GetComponent<ItemGiver>().toGive = this.toGive;
        GetComponentInChildren<SpriteRenderer>().sprite = this.toGive.itemIcon;
        gameObject.name = "Floating "+toGive.name;
    }

    void OnValidate() {
        OnEnable();
    }
}