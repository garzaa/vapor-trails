using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class Item : ScriptableObject {
    public Sprite itemIcon;
    public Sprite detailedIcon;
    public bool stackable;
    public int count = 1;
    public int cost = 0;

    [SerializeField] List<ItemType> type;

    [TextArea] [SerializeField]
    string description;

    public List<GameState> gameStates;

    [SerializeField]
    public List<ItemEffect> itemEffects;

    public SerializableItem MakeSerialized() {
        return new SerializableItem(this.name, this.count);
    }

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
