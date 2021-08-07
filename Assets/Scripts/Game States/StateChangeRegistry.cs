using UnityEngine;
using System.Collections.Generic;

public class StateChangeRegistry : MonoBehaviour {
	static HashSet<IStateChangeListener> stateChangeReactors = new HashSet<IStateChangeListener>();

	public static void Add(IStateChangeListener r) {
		stateChangeReactors.Add(r);
	}

	public static void Remove(IStateChangeListener r) {
		stateChangeReactors.Remove(r);
	}

	public static void PushStateChange(bool fakeSceneLoad=false) {
		foreach (StateChangeReactor r in stateChangeReactors) {
			r.React(fakeSceneLoad);
		}
	}
}
