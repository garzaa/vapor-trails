using UnityEngine;

public abstract class StateChangeReactor : MonoBehaviour, IStateChangeListener {
	public void Awake() {
		StateChangeRegistry.Add(this);
	}

	public void OnDestroy() {
		StateChangeRegistry.Remove(this);
	}
	
	public abstract void React(bool fakeSceneLoad);
}
