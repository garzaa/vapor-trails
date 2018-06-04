using UnityEngine;

[System.Serializable]
public class Conversation : System.Object {
	public DialogueLine[] lines;

	public int Count() {
		return lines.Length;
	}

	public DialogueLine this[int i] {
		get { return lines[i]; }
	}
}
