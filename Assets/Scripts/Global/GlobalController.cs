using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GlobalController : MonoBehaviour {

	public static GlobalController gc;
	public TitleText editorTitleText;
	public static TitleText titleText;
	static SignUI signUI;
	static BlackFadeUI blackoutUI;
	static DialogueUI dialogueUI;
	public static PlayerController pc;
	public static bool dialogueOpen;
	static bool dialogueOpenedThisFrame = false;
	public bool pauseEnabled = true;
	static bool paused = false;
	public static bool dialogueClosedThisFrame = false;
	static NPC currentNPC;
	public static PlayerFollower playerFollower;
	public static Save save {
		get {
			return gc.saveContainer.GetSave();
		}
	}
	static CloseableUI pauseUI;
	public static bool inAnimationCutscene;
	static bool inAbilityGetUI;
	public static Animator abilityUIAnimator;
	public static BarUI bossHealthUI;
	public static AudioListener audioListener;

	public static bool xboxController = false;
	public static bool playstationController = false;

	static DialogueLine toActivate = null;

	static RespawnManager rm;
	public static InventoryController inventory;

	static Queue<NPC> queuedNPCs = new Queue<NPC>();

	static int saveSlot = 1;
	static ParallaxOption parallaxOption;

	public GameObject talkPrompt;
	public GameObject newDialoguePrompt;

	public bool uiClosedThisFrame = false;
	public static int openUIs = 0;

	static GameObject playerMenu;
	static Coroutine showPlayerRoutine;
	public static BossFightIntro bossFightIntro;

	[SerializeField] SaveContainer saveContainer;

	void Awake() {
		gc = this;
		titleText = editorTitleText;
		dialogueUI = GetComponentInChildren<DialogueUI>();
		signUI = GetComponentInChildren<SignUI>();
		pc = GetComponentInChildren<PlayerController>();
		rm = GetComponent<RespawnManager>();
		playerFollower = gc.GetComponentInChildren<PlayerFollower>();
		blackoutUI = GetComponentInChildren<BlackFadeUI>();
		pauseUI = GetComponentInChildren<PauseUI>();
		abilityUIAnimator = GameObject.Find("AbilityGetUI").GetComponent<Animator>();
		inventory = gc.GetComponentInChildren<InventoryController>();
		parallaxOption = gc.GetComponentInChildren<ParallaxOption>();
		bossHealthUI = GameObject.Find("BossHealthUI").GetComponent<BarUI>();
		bossHealthUI.gameObject.SetActive(false);
		playerMenu = GameObject.Find("PlayerMenu");
		audioListener = gc.GetComponentInChildren<AudioListener>();
		bossFightIntro = gc.GetComponentInChildren<BossFightIntro>(includeInactive:true);

#if UNITY_EDITOR
		EditorApplication.playModeStateChanged += OnPlayModeChange;
#endif
	}

	void Start() {
		saveContainer.OnSceneLoad();
	}

#if UNITY_EDITOR
	// "clean" the runtime data when the editor stops playing to mimic a game exit
	private static void OnPlayModeChange(PlayModeStateChange stateChange) {
		if (stateChange == PlayModeStateChange.ExitingPlayMode) {
			gc.saveContainer.CleanEditorRuntime();
		}
	}	
#endif

	public static void ShowTitleText(string title, string subTitle = null) {
		titleText.ShowText(title, subTitle);
	}

	public static bool HasSavedGame() {
		return BinarySaver.HasFile(saveSlot); 
	}

	public static void NewGamePlus() {
		AlerterText.Alert("Hey idiot you forgot to make NG+");
		return;
	}

	public void NewGame() {
		// replace with a fresh save, everything will be loaded correctly in the next scene
		saveContainer.WipeSave();
		saveContainer.OnSceneLoad();
	}

	public static bool HasBeatGame() {
		return false;
	}

	static void OpenInventory() {
		inventory.ShowInventory();
		pc.EnterCutscene(invincible:false);
	}

	static void CloseInventory() {
		inventory.HideInventory();
		pc.ExitCutscene();
	}

	// don't close menus if the close button is the same
	IEnumerator OpenMenu(GameObject menuTarget) {
		yield return new WaitForEndOfFrame();
		menuTarget.SetActive(true);
	}

	void LateUpdate() {
		if (Input.GetKeyDown(KeyCode.R) && SceneManager.GetActiveScene().name.Equals("TargetTest")) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		if (inAbilityGetUI && InputManager.ButtonDown(Buttons.JUMP)) {
			HideAbilityGetUI();
		}

		bool inInventory = inventory.inventoryUI.animator.GetBool("Shown");
		if ((InputManager.ButtonDown(Buttons.INVENTORY) || InputManager.GenericEscapeInput()) && inInventory) {
			if (inInventory) {
				CloseInventory();
			}
		} else if (inInventory) {
			// avoid any pre-late update weirdness
			pc.EnterCutscene();
		} else if (pauseEnabled && InputManager.ButtonDown(Buttons.INVENTORY) && !pc.inCutscene && pc.IsGrounded()) {
			gc.StartCoroutine(OpenMenu(playerMenu));
		}

		
		if (InputManager.ButtonDown(Buttons.PAUSE) && pauseEnabled && !inInventory && !paused) {
			// pauseUI takes care of unpausing
			Pause();
		}
		
		if (InputManager.GenericContinueInput()) {
			GlobalController.OnDialogueSkip();
		}

		dialogueOpenedThisFrame = false;
		dialogueClosedThisFrame = false;

		UpdateControllerStatus();

#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.LeftBracket)) {
			AlerterText.Alert("Saving game...");
			GlobalController.SaveGame(autosave:false);
			AlerterText.Alert("Game saved");
		} else if (Input.GetKeyDown(KeyCode.RightBracket)) {
			AlerterText.Alert("Loading game...");
			GlobalController.LoadGame();
			AlerterText.Alert("Game loaded");
		}
#endif
	}

	public static void OnDialogueSkip() {
		if (!dialogueOpen || dialogueOpenedThisFrame || inAnimationCutscene) {
			return;
		}

		if (dialogueOpen && openUIs > 1) {
			return;
		}

		if (dialogueUI.slowRendering) {
			dialogueUI.CancelSlowRender();
			return;
		}

		if (dialogueUI.switchingImage) {
			dialogueUI.SwitchSpeakerImage();
		}

		//advance dialogue line or close
		//if necessary, hit the activatable from the previous line
		//and block dialogue/enter cutscene if necessary
		if (toActivate != null) {
			toActivate.activatable.Activate();
			if (toActivate.blocking) {
				//block dialogue line rendering and hide dialogue UI
				EnterCutscene();
				//don't render the dialogue
				toActivate = null;
				return;
			}
			toActivate = null;
		}

		DialogueLine nextLine = currentNPC.GetNextLine();

		if (nextLine != null) {
			dialogueUI.RenderDialogueLine(
				nextLine, 
				currentNPC.hasNextLine() || queuedNPCs.Count>0
			);
			if (nextLine.activatable != null) {
				if (!nextLine.activatesOnLineEnd) {
					nextLine.activatable.Activate();
				} else {
					toActivate = nextLine;
				}
			}
		} else {
			ExitDialogue();
		}
	}

	public static SaveContainer GetSaveContainer() {
		return gc.saveContainer;
	}

	public static void EnterDialogue(NPC npc, bool fromQueue=false) {
		Hitstop.Interrupt();
		if (dialogueOpen) {
			queuedNPCs.Enqueue(npc);
			return;
		}
		if (!pc.IsFacing(npc.gameObject)) {
			pc.ForceFlip();
		}
		pc.EndCombatStanceCooldown();
		if (!fromQueue) dialogueUI.Open();
		currentNPC = npc;
		dialogueOpenedThisFrame = true;
		dialogueUI.ShowNameAndPicture(npc.GetCurrentLine());
		if (npc.centerCameraInDialogue) {
			playerFollower.LookAtPoint(npc.gameObject);
		}
		pc.EnterCutscene(pauseAnimation: false);
	}

	public static void ExitDialogue() {
		dialogueOpen = false;
		dialogueClosedThisFrame = true;
		if (currentNPC != null) {
			currentNPC.CloseDialogue();
			if (currentNPC.centerCameraInDialogue) playerFollower.StopLookingAtPoint();
		}
		currentNPC = null;
		if (queuedNPCs.Count != 0) {
			EnterDialogue(queuedNPCs.Dequeue(), fromQueue: true);
			FinishOpeningLetterboxes();
		} else {
			dialogueUI.Close();
		}
	}

	public static void FinishOpeningLetterboxes() {
		dialogueOpen = true;
		inAnimationCutscene = false;
		DialogueLine nextLine = currentNPC.GetNextLine();
		if (nextLine != null) {
			dialogueUI.RenderDialogueLine(nextLine, currentNPC.hasNextLine(), fromCutscene: true);
			if (nextLine.activatable != null) {
				if (!nextLine.activatesOnLineEnd) {
					nextLine.activatable.Activate();
				} else {
					toActivate = nextLine;
				}
			}
		} else {
			ExitDialogue();
		}
	}

	public static void FinishClosingLetterboxes() {

	}

	public static void OpenSign(string text, Vector2 position) {
		if (!signUI.IsVisible()) {
			signUI.SetText(text);
			signUI.SetPosition(position);
			signUI.Show();
		}
	}

	public static void CloseSign() {
		signUI.Hide();
	}

	public static Vector2 GetPlayerPos() {
		return pc.transform.position;
	}

	//called when the new respawn scene is loaded
	public static void StartPlayerRespawning() {
		pc.StartRespawning();
	}

	public static void AddGameFlag(GameFlag f) {
		if (!save.gameFlags.Contains(f)) {
			save.gameFlags.Add(f);
			PropagateFlagChange();
		}
	}

	// call this for every sub-item change 
	public static void PushStateChange() {
		foreach (IStateUpdateListener listener in FindObjectsOfType<MonoBehaviour>().OfType<IStateUpdateListener>()) {
			listener.OnStateUpdate();
		}
	}

	public static void PropagateFlagChange() {
		PushStateChange();
		foreach (SwitchOnStateImmediate i in FindObjectsOfType<SwitchOnStateImmediate>()) {
			i.ReactToStateChange();
		}
		foreach (StatefulNPC n in FindObjectsOfType<StatefulNPC>()) {
			n.ReactToStateChange();
		}
	}

	public static void RemoveGameFlag(GameFlag f) {
		if (save.gameFlags.Contains(f)) {
			save.gameFlags.Remove(f);
			PropagateFlagChange();
		}
	}

	public static bool HasFlag(GameFlag f) {
		if (save == null || f == GameFlag.None) {
			return false;
		}
		return save.gameFlags.Contains(f);
	}

	public static void PropagateStateChange(bool immediateOnly=true) {
		// all loaded objects, including inactive ones
		List<EnableOnGameState> immediates = (Resources.FindObjectsOfTypeAll(typeof(EnableOnGameState)) as EnableOnGameState[])
			.Where(x => immediateOnly ? x.immediate : true).ToList();
		foreach (EnableOnGameState i in immediates) {
			i.CheckState();
		}

		UpdateStatefulNPCs();

		Animator playerAnimator = pc.GetComponent<Animator>();
		playerAnimator.logWarnings = true;
		foreach (string s in save.gameStates) {
			if (s.StartsWith("anim_")) {
				playerAnimator.SetBool(s, true);
			}
		}

		PushStateChange();
		gc.saveContainer.SyncImmediateStates(saveSlot);
	}

	public static void AddState(GameState state) {
		if (state == null) return;
		if (!save.gameStates.Contains(state.name)) save.gameStates.Add(state.name);
		PropagateStateChange();
		if (state.writeImmediately) {
			gc.saveContainer.SyncImmediateStates(saveSlot);
		}
	}

	public static void AddStates(List<GameState> states) {
		bool writeImmediate = false;
		foreach (GameState state in states) {
			if (!save.gameStates.Contains(state.name)) save.gameStates.Add(state.name);
			if (state.writeImmediately) writeImmediate = true;
		}
		PropagateStateChange();
		if (writeImmediate) {
			gc.saveContainer.SyncImmediateStates(saveSlot);
		}
	}

	public static bool HasState(GameState state) {
		return save.gameStates.Contains(state.name);
	}

	public static void RemoveState(GameState state) {
		save.gameStates.Remove(state.name);
		PropagateStateChange();
	}

	public static void LoadScene(string sceneName, Beacon beacon=null) {
		if (beacon != null && beacon.leftScene != null) {
			string beaconSceneName = beacon.leftScene.scene.SceneName;

			if (SceneManager.GetActiveScene().path.Contains(beaconSceneName)) {
				beaconSceneName = beacon.rightScene.scene.SceneName;
			}

			gc.GetComponent<TransitionManager>().LoadScene(beaconSceneName, beacon);
		} else {
			gc.GetComponent<TransitionManager>().LoadScene(sceneName, beacon);
		}
	}

	public static void LoadScene(SceneContainer sceneContainer, Beacon beacon=null) {
		LoadScene(sceneContainer.scene.SceneName, beacon:beacon);
	}

	public static void LoadSceneToPosition(string sceneName, Vector2 position) {
		gc.GetComponent<TransitionManager>().LoadSceneToPosition(sceneName, position);
	}

	public static void LoadSceneWithSubway(string sceneName) {
		gc.GetComponent<TransitionManager>().LoadSceneWithSubway(sceneName);
	}

	public static void MovePlayerTo(Vector2 position, bool fade=false) {
		if (fade) {
			gc.StartCoroutine(gc.MovePlayerWithFade(position));
			return;
		}
		gc.StartCoroutine(gc.MovePlayerNextFrame(position, fade));
	}

	// make sure the trail renderers don't emit
	IEnumerator MovePlayerNextFrame(Vector2 position, bool fade) {
		pc.DisableTrails();
		if (pc.speedLimiter) pc.speedLimiter.enabled = false;
		yield return new WaitForEndOfFrame();
		pc.transform.position = position;
		playerFollower.SnapToTarget();
		pc.EnableTrails();
		if (fade) {
			yield return new WaitForSecondsRealtime(0.5f);
			UnFadeToBlack();
		}
		pc.ExitCutscene();
		pc.speedLimiter.enabled = true;
	}

	public IEnumerator MovePlayerWithFade(Vector2 position) {
		pc.EnterCutscene();
		FadeToBlack();
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(MovePlayerNextFrame(position, fade:true));
	}

	public static void MovePlayerToBeacon(Beacon beacon) {
		BeaconWrapper b = Object.FindObjectsOfType<BeaconWrapper>().Where(
			x => x.beacon == beacon
		).First();
		if (b != null) {
			MovePlayerTo(b.transform.position);
			if (b.activateOnLoad != null) {
				b.activateOnLoad.Activate();
			}
		} else {
			// if no beacon wrapper, there should at least be a corresponding door
			// with that beacon
			Door d = Object.FindObjectsOfType<Door>().Where(
				x => x.beacon == beacon
			).First();
		}
	}

	public static void ShortBlackFade() {
		gc.StartCoroutine(gc._ShortBlackFade());
	}

	IEnumerator _ShortBlackFade() {
		FadeToBlack();
		yield return new WaitForSecondsRealtime(0.5f);
		UnFadeToBlack();
	}

	public static void FadeToBlack() {
		blackoutUI.Show();
	}

	public static void UnFadeToBlack() {
		blackoutUI.Hide();
	}

	public static void FlashWhite() {
		blackoutUI.FlashWhite();
	}

	public static void ShowUI() {
		foreach (BarUI b in gc.GetComponentsInChildren<BarUI>(includeInactive:true)) {
			b.gameObject.SetActive(true);
		}
		bossHealthUI.gameObject.SetActive(false);
		inventory.moneyUI.gameObject.SetActive(true);
	}

	public static void HideUI() {
		foreach (BarUI b in gc.GetComponentsInChildren<BarUI>()) {
			b.gameObject.SetActive(false);
		}
		inventory.moneyUI.gameObject.SetActive(false);
	}

	public void ExitGame() {
		Application.Quit();
	}

	//called from a cutscene animation to finish it and resume dialogue
	public static void CutsceneCallback() {
		if (!dialogueOpen) return;
		// show the dialogue UI if there's a next line
		// catch NPC being hidden by an activated animation
		if (currentNPC != null && currentNPC.hasNextLine()) {
			dialogueUI.Open();
		} else {
			ExitDialogue();
		}
	}

	// hide dialogue UI but keep the player frozen
	// dialogue being open is a prerequisite for the cutscene state :^(
	public static void EnterCutscene() {
		inAnimationCutscene = true;
		if (dialogueOpen) {
			dialogueUI.Close();
		}
	}

	public static void EnterSlowMotion() {
		Hitstop.Interrupt();
		Time.timeScale = 0.3f;
	}

	public static void ExitSlowMotion() {
		Time.timeScale = 1;
	}

	public static void SlowMotionFor(float seconds) {
		gc.StartCoroutine(gc.TimedSlowMotion(seconds));
	}

	IEnumerator TimedSlowMotion(float seconds) {
		EnterSlowMotion();
		yield return new WaitForSecondsRealtime(seconds);
		ExitSlowMotion();
	}

	public static void LoadGame() {
		gc.saveContainer.LoadFromSlot(saveSlot);
		LoadSceneToPosition(save.sceneName, save.playerPosition);
 	}

	public static void SaveGame(bool autosave=false) {
		if (save.unlocks.HasAbility(Ability.Heal) && !autosave) {
			AlerterText.Alert("Rebuilding waveform");
			pc.FullHeal();
		}
		if (autosave) AlerterText.AlertImmediate("Autosaving...");
		save.currentHP = pc.currentHP;
		save.maxHP = pc.maxHP;
		save.currentEnergy = pc.currentEnergy;
		save.maxEnergy = pc.maxEnergy;
		save.basePlayerDamage = pc.baseDamage;
		save.playerPosition = pc.transform.position;
		save.sceneName = SceneManager.GetActiveScene().path;
		gc.GetComponentInChildren<MapFog>().SaveCurrentMap();
		gc.saveContainer.WriteToDiskSlot(saveSlot);
		if (autosave) AlerterText.AlertImmediate("Autosave complete");
	}

	public static void Pause() {
		if (pc.inCutscene) {
			return;
		}
		paused = true;
		CameraShaker.StopShaking();
		pauseUI.Open();
	}

	public static void Unpause() {
		paused = false;
	}

	public static SerializedPersistentObject GetPersistentObject(string id) {
		if (save == null) {
			return null;
		}
		return save.GetPersistentObject(id);
	}

	public static void SavePersistentObject(SerializedPersistentObject o) {
		if (save == null) {
			return;
		}
		save.SavePersistentObject(o);
	}

	static void UpdateControllerStatus() {
		string[] names = Input.GetJoystickNames();
		for (int x = 0; x < names.Length; x++)
		{
			if (names[x].Length == 19)
			{
				playstationController = true;
				xboxController = false;
			}
			if (names[x].Length == 33)
			{
				playstationController = false;
				xboxController = true;

			}
		}
	}

	public static void AddItem(StoredItem s, bool quiet=false) {
		Item item = s.item;
		if (!quiet) {
            SoundManager.ItemGetSound();
			if (!item.IsType(ItemType.ABILITY)) {
				if (s.count != 1)
					AlerterText.Alert($"{item.name} ({s.count}) acquired");
				else 
					AlerterText.Alert(item.name + " acquired");
			}
        }
		if (item.gameStates != null) {
			AddStates(item.gameStates);
		}
		inventory.AddItem(s, quiet);
		PropagateItemChange();
		PushStateChange();
	}

	public static void PropagateItemChange(bool immediateOnly=true) {
		List<EnableOnItem> immediates = (Resources.FindObjectsOfTypeAll(typeof(EnableOnItem)) as EnableOnItem[])
			.Where(x => immediateOnly ? x.immediate : true).ToList();
		foreach (EnableOnItem i in immediates) {
			i.CheckState();
		}
		UpdateStatefulNPCs();
	}

	static void UpdateStatefulNPCs() {
		foreach (StatefulNPC n in FindObjectsOfType<StatefulNPC>()) {
			n.ReactToStateChange();
		}
	}

	public static void ShowAbilityGetUI() {
		abilityUIAnimator.SetTrigger("Show");
		pc.EnterCutscene();
		// to keep the player from accidentally skipping the animation early
		gc.Invoke("EnterAbilityUI", 1f);
	}

	void EnterAbilityUI() {
		inAbilityGetUI = true;
	}

	public static void HideAbilityGetUI() {
		pc.ExitCutscene();
		SoundManager.InteractSound();
		abilityUIAnimator.SetTrigger("Hide");
		inAbilityGetUI = false;
	}

	public static void UnlockAbility(Ability a) {
		save.UnlockAbility(a);
	}

	public static void EnterMerchantDialogue(Merchant merchant) {
		pc.EnterCutscene();
		inventory.currentMerchant = merchant;
		OpenInventory();
	}

	public static void BoostStat(StatType statType, int amount) {
		switch (statType) {
			case StatType.HEALTH:
				int missing = pc.maxHP-pc.currentHP;
				pc.maxHP += amount;
				pc.currentHP = pc.maxHP-missing;
				break;
			case StatType.ENERGY:
				pc.maxEnergy += amount;
				break;
			case StatType.DAMAGE:
				pc.baseDamage += amount;
				break;
		}
		StatBoostUI.ReactToBoost(statType, amount);
	}

	public static void EnableParallax() {
		parallaxOption.moveParallax = true;
	}

	public static void DisableParallax() {
		parallaxOption.moveParallax = false;
	}

	public static void HidePlayer() {
		if (showPlayerRoutine != null) {
			gc.StopCoroutine(showPlayerRoutine);
		}
		pc.EnterCutscene();
		pc.Hide();
	}

	public static void ShowPlayer() {
		// can't have the player exiting the cutscene early!
		pc.ExitCutscene();
		showPlayerRoutine = gc.StartCoroutine(gc._ShowPlayer());
	}
	
	IEnumerator _ShowPlayer() {
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		pc.Show();
		showPlayerRoutine = null;
	}
}
