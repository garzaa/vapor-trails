using UnityEngine;
using XNode;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using System;
using XNodeEditor;
#endif

/*
	THIS CLASS NEEDS TO KNOW:
	1. NOTHING ABOUT STATE!! it is a scriptable object
	2. how to get a weighted transition to the next node
	3. how to get the first node
*/
[CreateAssetMenu]
public class CombatAIGraph : NodeGraph {
	#pragma warning disable 0649
	[SerializeField] AINode rootNode;
	#pragma warning restore 0649

	public AINode GetRootNode() {
		return rootNode;
	}

	public AINode GetWeightedTransition(AINode currentNode, CombatOptionType optionType) {
		List<WeightedNode> weightedNodes = currentNode.GetWeightedTransitions(optionType);
		float choice = UnityEngine.Random.value;
		foreach(WeightedNode w in weightedNodes) {
			if (w.weight < choice) return w.node;
		}
		return weightedNodes[weightedNodes.Count-1].node;
	}
}

#if UNITY_EDITOR
[CustomNodeGraphEditor(typeof(CombatAIGraph))]
public class AIGraphEditor : NodeGraphEditor {
	public override string GetNodeMenuName(Type type) {
		if (typeof(AINode).IsAssignableFrom(type)) {
			return base.GetNodeMenuName(type);
		} else return null;
	}
}
#endif
