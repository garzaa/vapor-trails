using UnityEngine;
using XNode;
using System.Collections.Generic;

/*
	THIS CLASS NEEDS TO KNOW:
	1. its own node type
	2. animation state name
	3. animation weights for each one
	4. options for neutral, advantage, and disadvantage
*/
[CreateNodeMenu("AI Node")]
[NodeWidth(270)]
public class AINode : Node {
	#pragma warning disable 0649
	[SerializeField] string animationStateName;
	[SerializeField] bool immediatelyChooseState;
	#pragma warning restore 0649

	[Input(backingValue=ShowBackingValue.Never)]
	public AINodeTransition input;

	[Output(dynamicPortList=true, connectionType=ConnectionType.Override)]
	public AINodeTransition[] neutral;

	[Output(dynamicPortList=true, connectionType=ConnectionType.Override)]
	public AINodeTransition[] advantage;

	[Output(dynamicPortList=true, connectionType=ConnectionType.Override)]
	public AINodeTransition[] disadvantage;

	void Awake() {
		name = animationStateName;
	}

	public bool HasAnimation() {
		return !immediatelyChooseState;
	}

	public List<WeightedNode> GetWeightedTransitions(CombatOptionType combatOption) {
		AINodeTransition[] outputs;
		if (combatOption.Equals(CombatOptionType.OFFENSIVE)) outputs = advantage;
		else if (combatOption.Equals(CombatOptionType.DEFENSIVE)) outputs = disadvantage;
		else outputs = neutral;

		List<WeightedNode> values = new List<WeightedNode>();
		float total = 0;
		for (int i=0; i<outputs.Length; i++) {
			total += outputs[i].weight;
		}

		float current = 0;
		for (int i=0; i<outputs.Length; i++) {
			values.Add(new WeightedNode(
					current,
					GetPort(nameof(outputs)+" "+i).Connection.node as AINode
			));
			current += outputs[i].weight / total;
		}

		values.Sort((a, b) => a.weight.CompareTo(b.weight));

		return values;
	}
}

public class WeightedNode {
	public float weight;
	public AINode node;

	public WeightedNode(float w, AINode n) {
		weight = w;
		node = n;
	}	
}
