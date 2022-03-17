using Newtonsoft.Json;

[JsonObject]
[System.Serializable]
public class StoredItem {
    public string name;
    public int count;

    [JsonIgnore]
    public Item item {
        get {
            return ItemDB.GetItem(this.name);
        }
    }

    [JsonConstructor]
    public StoredItem(string name, int count) {
        this.name = name;
        this.count = count;
    }

    public StoredItem(Item i) {
        this.name = i.name;
        this.count = 1;
    }

}
