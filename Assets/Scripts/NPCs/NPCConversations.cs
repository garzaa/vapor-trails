using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCConversations : MonoBehaviour {

	public List<Conversation> conversations;

	public int Count() {
		return conversations.Count;
	}

	public Conversation this[int i] {
		get { return conversations[i]; }
	}
}
