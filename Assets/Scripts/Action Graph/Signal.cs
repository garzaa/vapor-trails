[System.Serializable]
public class Signal {
    public bool value { get; set; }

    public Signal(bool value) {
        this.value = value;
    }

    public Signal Flip() {
        this.value = !value;
        return this;
    }
}
