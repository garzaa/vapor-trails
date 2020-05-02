[System.Serializable]
public class Choice {
    public string choiceText;
    public Activatable activatable;

    public Choice(string choiceText, Activatable choiceActivatable) {
        this.choiceText = choiceText;
        this.activatable = choiceActivatable;
    }
}