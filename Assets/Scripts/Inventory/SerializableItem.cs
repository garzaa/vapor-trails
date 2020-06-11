[System.Serializable]
public class SerializableItem {
    public string name;
    public int count;
    
    public SerializableItem(string name, int count) {
        this.name = name;
        this.count = count;
    }
}