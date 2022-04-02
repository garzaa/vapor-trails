using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveManager : MonoBehaviour {

	[SerializeField] SaveContainer saveContainer;

	int saveSlot = 1;

	public static Save save {
		get {
			return instance.saveContainer.GetSave();
		}
	}

	protected static SaveManager instance;
	
	PlayerStats playerStats;

	void Awake() {
		instance = this;
		playerStats = GetComponentInChildren<PlayerStats>();

#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += OnPlayModeChange;
#endif
	}

#if UNITY_EDITOR
	// "clean" the runtime data when the editor stops playing to mimic a game exit
	private void OnPlayModeChange(PlayModeStateChange stateChange) {
		if (stateChange == PlayModeStateChange.ExitingPlayMode) {
			saveContainer.CleanEditorRuntime();
		}
	}	
#endif

	public static void LoadGame() {
		TransitionManager.DirtyScene();
		instance.saveContainer.LoadFromSlot(instance.saveSlot);
		GlobalController.LoadSceneToPosition(save.sceneName, save.playerPosition);
	}

	public static void LoadChapter(SaveContainer chapter, Beacon beacon) {
		TransitionManager.DirtyScene();
		TransitionManager.SetChapter(chapter);
		GlobalController.LoadScene(beacon);
	}

	public static void ImportChapter(SaveContainer chapter) {
		instance.saveContainer = chapter;
		instance.saveContainer.SetRuntimeFromSelfData();
		PushStateChange(fakeSceneLoad: true);
	}

	public static void ImportCurrentChapter() {
		instance.saveContainer.SetRuntimeFromSelfData();
		PushStateChange(fakeSceneLoad: true);
	}

	public static void SaveGame(bool autosave=false) {
		if (instance.playerStats.HasAbility(Ability.Heal) && !autosave) {
			AlerterText.Alert("Rebuilding waveform");
			GlobalController.pc.FullHeal();
		}
		if (autosave) AlerterText.AlertImmediate("Autosaving...");
		foreach (ISaveListener saveListener in UtilityMethods.Find<ISaveListener>(includeInactive: true)) {
			saveListener.OnBeforeSave();
		}
		save.playerPosition = GlobalController.pc.transform.position;
		save.sceneName = SceneManager.GetActiveScene().path;
		instance.saveContainer.WriteToDiskSlot(instance.saveSlot);
		if (autosave) AlerterText.AlertImmediate("Autosave complete");
	}

	void OnDisable() {
		if (!Application.isPlaying) return;
		// only do disk IO when a scene is unloaded and not every time an item/state is added
		saveContainer.SyncImmediateStates(saveSlot);
	}

	public static Dictionary<string, object> GetPersistentObject(PersistentObject o) {
		return save.GetPersistentObject(o);
	}

	public static void SavePersistentObject(PersistentObject o) {
		save.SetPersistentObject(o);
	}

	public static void PushStateChange(bool fakeSceneLoad=false) {
		StateChangeRegistry.PushStateChange(fakeSceneLoad);
	}

	public static void AddGameFlag(GameFlag f) {
		save.gameFlags.Add(f);
	}
	
	public static void RemoveGameFlag(GameFlag f) {
		save.gameFlags.Remove(f);
		PushStateChange();
	}

	public static bool HasFlag(GameFlag f) {
		if (save == null || f == GameFlag.None) {
			return false;
		}
		return save.gameFlags.Contains(f);
	}

	public static void AddState(GameState state) {
		if (state == null) return;
		save.gameStates.Add(state.name);
		PushStateChange();
	}

	public static void AddStates(List<GameState> states) {
		foreach (GameState state in states) {
			if (state == null) continue;
			save.gameStates.Add(state.name);
		}
		PushStateChange();
	}

	public static bool HasState(GameState state) {
		return save.gameStates.Contains(state.name);
	}

	public static void RemoveState(GameState state) {
		save.gameStates.Remove(state.name);
		PushStateChange();
	}

}
