using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameState", menuName = "Item", order = 1)]
public class Item : ScriptableObject {
    public Sprite itemIcon;
    public Sprite detailedIcon;
    public bool stackable;
    public int count = 1;
    public int cost = 0;

    public List<ItemType> type;

    [TextArea]
    public string description;

    public List<GameState> gameStates;

    public Event testEvent;

    [SerializeField]
    public List<ItemEffect> itemEffects;

    public SerializableItem MakeSerialized() {
        return new SerializableItem(this.name, this.count);
    }

    public virtual void OnPickup(bool quiet=false) {

    }

    public Item Instance() {
        return (Item) this.MemberwiseClone();
    }
}
