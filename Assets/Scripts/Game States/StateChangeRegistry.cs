using UnityEngine;
using System.Collections.Generic;

public class StateChangeRegistry : MonoBehaviour {
	static HashSet<IStateChangeListener> stateChangeListeners = new HashSet<IStateChangeListener>();

	public static void Add(IStateChangeListener r) {
		stateChangeListeners.Add(r);
	}

	public static void Remove(IStateChangeListener r) {
		stateChangeListeners.Remove(r);
	}

	public static void PushStateChange(bool fakeSceneLoad=false) {
		foreach (IStateChangeListener listener in stateChangeListeners) {
			listener.React(fakeSceneLoad);
		}
	}
}
