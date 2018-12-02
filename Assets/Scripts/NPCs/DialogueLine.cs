using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine : System.Object {
	[TextArea]
	public string lineText;
	public string speakerName;
	public Sprite speakerImage; 
	public Voice voiceSound = Voice.Medium;
	public Activatable activatable;
	public bool activatesOnLineEnd;
	public bool blocking;
	public GameFlag gameFlag;
}

public enum Voice : int {
	Low,
	Medium,
	High
}