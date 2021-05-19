[System.Serializable]
public class StoredItem {
    public string name;
    public int count;

    public Item item {
        get {
            return ItemDB.GetItem(this.name);
        }
    }

    public StoredItem(Item i) {
        this.name = i.name;
        this.count = 1;
    }

}
