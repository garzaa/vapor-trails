using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class Item : ScriptableObject {
    public Sprite itemIcon;
    public Sprite detailedIcon;
    public bool stackable;
    public int cost = 0;

    public List<ItemType> type;

    [TextArea]
    public string description;

    public List<GameState> gameStates;

    // public List<ItemEffect> itemEffects;

    public virtual void OnPickup(bool quiet) {

    }

    public Item Instance() {
        return (Item) this.MemberwiseClone();
    }

    virtual public string GetDescription() {
        return ControllerTextChanger.ReplaceText(description);
    }

    virtual public bool IsType(ItemType t) {
        return type.Contains(t);
    }
}
