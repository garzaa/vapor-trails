using UnityEngine;
using UnityEngine.UI;
using XNode;

/*
	THIS CLASS NEEDS TO KNOW:
	1. where it is in its own copy of the graph
	2. how to match a node to an animator trigger
	3. DATA:
		- how to figure out player's threat bubble
			- ranged attacks to exclude from threat bubble (bullets)
		- its own threat bubble, whether the player is in range of its own attacks
	4. how to do something that doesn't necessarily have a specific animation (maintain spacing)
		with a callback after a certain time has passed (or another thing is triggered)
	5. know whether it's in advantage or disadvantage (player stunned/this stunned)
*/
public class CombatAI : MonoBehaviour {
	#pragma warning disable 0649
	[SerializeField] float reactionTime = 0.2f;
	[SerializeField] CombatAIGraph aiGraph;

	// debug stuff
	[SerializeField] Text combatStateIndicator;
	[SerializeField] Text stateGraphNode;
	#pragma warning restore 0649

	Animator animator;
	AINode currentNode;
	Entity player;
	Entity entity;

	CombatOptionType currentCombatState = CombatOptionType.NEUTRAL;

	void Awake() {
		animator = GetComponent<Animator>();
		entity = GetComponent<Entity>();
	}

	void Start() {
		// enter the root node
		EnterNode(aiGraph.GetRootNode());
		player = GlobalController.pc;
	}

	void Update() {
		currentCombatState = GetCurrentCombatState();
		combatStateIndicator.text = currentCombatState.ToString();
		stateGraphNode.text = currentNode.name;
	}

	void EnterNode(AINode node) {
		currentNode = node;
		if (!node.HasAnimation()) {
			LeaveCurrentNode();
			return;
		}
		animator.Play(node.name, layer:0);
	}

	void LeaveCurrentNode() {
		AINode destination = aiGraph.GetWeightedTransition(currentNode, currentCombatState);
		EnterNode(destination);
	}

	CombatOptionType GetCurrentCombatState() {
		if (InAdvantage()) return CombatOptionType.OFFENSIVE;
		else if (InDisadvantage()) return CombatOptionType.DEFENSIVE;
		else return CombatOptionType.NEUTRAL;
	}

	bool InAdvantage() {
		// also check for frame advantage?
		return !entity.stunned && player.stunned;
	}

	bool InDisadvantage() {
		return entity.stunned && !player.stunned;
	}

	bool PlayerInThreatBubble() {
		// do a normal distance calculation for now, otherwise get some idea of attack 
		return Vector2.Distance(transform.position, player.GetComponent<Transform>().position) < 3f;
	}

	bool InPlayerThreatBubble() {
		return Vector2.Distance(transform.position, player.GetComponent<Transform>().position) < 3f;
	}

	public void OnAttackSequenceFinish() {
		// look at possible transitions out of the current node
		LeaveCurrentNode();
	}
}
