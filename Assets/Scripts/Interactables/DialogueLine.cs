using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine : System.Object {
	[TextArea]
	public string lineText;
	public string speakerName;
	public Sprite speakerImage; 
}
