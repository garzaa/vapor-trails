// unfortunately, this class only exists so StatefuLNPC can implement these methods
// because it can't inherit from StateChangeReactor
// note: write better code next time
public interface IStateChangeListener {
	void Awake();
	void React(bool fakeSceneLoad);
	void OnDestroy();
}
