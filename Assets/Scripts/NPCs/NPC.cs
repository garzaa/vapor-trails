using UnityEngine;

public class NPC : Interactable {
	protected NPCConversations conversations;

	protected PersistentNPC persistence;

	bool interactedFromPlayer = false;
	protected bool inDialogue = false;

	public bool dialogueAnimationBool = false;
	public Animator dialogueAnimator;
	static readonly string DIALOGUE_ANIM_BOOL = "InDialogue";

	public bool generateMapIcon = true;
	public Sprite icon;

	public bool centerCameraInDialogue = true;

	public int currentConversation = 0;
	public int currentDialogueLine = 0;

	public NPC(NPCConversations c) {
		this.conversations = c;
	}

	protected override void ExtendedStart() {
		conversations = GetComponent<NPCConversations>();
		persistence = GetComponent<PersistentNPC>();
		// narsty hack to not display the prompt for multi-box NPC signs
		if (generateMapIcon && !name.ToLower().Contains("sign")) {
			SpawnMapIcon();
		}
	}

	protected virtual void SpawnMapIcon() {
		GameObject icon = (GameObject) Instantiate(
			Resources.Load("NPCIcon"),
			this.transform.position,
			Quaternion.identity,
			this.transform
		);
		icon.GetComponent<SpriteRenderer>().sprite = this.icon;
	}

	override public void InteractFromPlayer(GameObject player) {
		interactedFromPlayer = true;
		Interact(player);
	}

	override public void Interact(GameObject player) {
		if (GlobalController.dialogueClosedThisFrame) {
			return;
		}

		base.Interact(player);

		if (dialogueAnimationBool) {
			dialogueAnimator.SetBool(DIALOGUE_ANIM_BOOL, true);
		}

		//if there's a sign object attached
		if (GetComponent<Sign>() != null) {
			GlobalController.CloseSign();
		}

		//start at the beginning of whatever conversation
		currentDialogueLine = 0;

		//no need to restart the last conversation if it's been reached
		//the NPC conversation will take care of it
		inDialogue = true;
		GlobalController.EnterDialogue(this);
	}

	override public void AddPrompt() {
		promptPrefab = AtLastConversation() ? GlobalController.gc.talkPrompt : GlobalController.gc.newDialoguePrompt;
		base.AddPrompt();
	}

	public int GetConversationsHash() {
		return conversations.PersistentHashCode();
	}

	public bool AtLastConversation() {
		return currentConversation >= conversations.conversations.Count;
	}

	public DialogueLine GetNextLine() {
		if (currentDialogueLine < conversations[currentConversation].Count()) {
			FinishDialogueLine(currentConversation, currentDialogueLine-1);
			currentDialogueLine++;
			return conversations[currentConversation][currentDialogueLine-1];
		} else {
			currentConversation++;
			return null;
		}
	}

	public bool hasNextLine() {
		return currentDialogueLine < conversations[currentConversation].Count();
	}

	public virtual void FinishDialogueLine(int conversationNumber, int lineNumber) {
		
	}

	//called whenever auxiliary conversations are added or removed
	public void ReactToLineRemoval() {
		//unfortunate nomenclature
		//but if the NPC is somewhere in the auxiliary conversations, reset it
		if (currentConversation > conversations.conversations.Count) {
			currentConversation = conversations.conversations.Count - 1;
		}
		//if there are new conversations to index into, start at the first one there
		if (conversations.auxConversations.Count > 0) {
			currentConversation = conversations.conversations.Count;
		}
	}

	public DialogueLine GetCurrentLine() {
		return conversations[currentConversation][currentDialogueLine];
	}

	virtual public void CloseDialogue() {
		inDialogue = false;
		if (persistence) {
			persistence.ReactToDialogueClose();
		}
		if (interactedFromPlayer) {
			AddPrompt();
		}
		if (dialogueAnimationBool) {
			dialogueAnimator.SetBool(DIALOGUE_ANIM_BOOL, false);
		}
	}

}
