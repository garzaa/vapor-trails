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

    public static Signal positive = new Signal(true);
    public static Signal negative = new Signal(false);
}
