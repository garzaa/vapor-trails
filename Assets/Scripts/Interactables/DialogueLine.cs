using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[System.Serializable]
public class DialogueLine : System.Object {
	[TextArea]
	public string[] possibleLines;
	public string speakerName;
	public Sprite speakerImage; 
}
