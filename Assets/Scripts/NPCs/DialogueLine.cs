using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueLine : System.Object {
	[TextArea]
	public string lineText;
	public string speakerName;
	public Sprite speakerImage; 
	public int voiceIndex = 0;
	public Activatable activatable;
	public bool activatesOnLineEnd;
	public bool blocking;
	public string gameFlag;
}
