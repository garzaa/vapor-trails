﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GlobalController : MonoBehaviour {

	#pragma warning disable 0649
	[SerializeField] Text versionUIText;
	#pragma warning restore 0649

	public static GlobalController gc;
	public TitleText editorTitleText;
	public static TitleText titleText;
	static SignUI signUI;
	static BlackFadeUI blackoutUI;
	static DialogueUI dialogueUI;
	static CinemachineInterface cmInterface;
	public static PlayerController pc;
	public static bool dialogueOpen;
	static bool dialogueOpenedThisFrame = false;
	public bool pauseEnabled = true;
	static bool paused = false;
	public static bool dialogueClosedThisFrame = false;
	static NPC currentNPC;
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

	public GameObject talkPrompt;
	public GameObject newDialoguePrompt;

	public bool uiClosedThisFrame = false;
	public static int openUIs = 0;

	static GameObject playerMenu;
	static Coroutine showPlayerRoutine;

	static StateChangeRegistry stateChangeRegistry;
	static PlayerStats playerStats;

	void Awake() {
		gc = this;
		titleText = editorTitleText;
		dialogueUI = GetComponentInChildren<DialogueUI>();
		signUI = GetComponentInChildren<SignUI>();
		pc = GetComponentInChildren<PlayerController>();
		rm = GetComponent<RespawnManager>();
		blackoutUI = GetComponentInChildren<BlackFadeUI>();
		pauseUI = GetComponentInChildren<PauseUI>();
		abilityUIAnimator = GameObject.Find("AbilityGetUI").GetComponent<Animator>();
		inventory = gc.GetComponentInChildren<InventoryController>();
		bossHealthUI = GameObject.Find("BossHealthUI").GetComponent<BarUI>();
		bossHealthUI.gameObject.SetActive(false);
		playerMenu = GameObject.Find("PlayerMenu");
		audioListener = gc.GetComponentInChildren<AudioListener>();
		cmInterface = gc.GetComponentInChildren<CinemachineInterface>();
		playerStats = GetComponentInChildren<PlayerStats>();
	}

	void Start() {
		versionUIText.text = Application.version;
	}

	public static void ShowTitleText(string title, string subTitle = null) {
		titleText.ShowText(title, subTitle);
	}

	public static bool HasSavedGame() {
		return JsonSaver.HasFile(saveSlot); 
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
			SaveManager.SaveGame(autosave:false);
			AlerterText.Alert("Game saved");
		} else if (Input.GetKeyDown(KeyCode.RightBracket)) {
			AlerterText.Alert("Loading game...");
			SaveManager.LoadGame();
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
			cmInterface.LookAtPoint(npc.gameObject.transform);
		}
		pc.EnterCutscene();
	}

	public static void ExitDialogue() {
		dialogueOpen = false;
		dialogueClosedThisFrame = true;
		if (currentNPC != null) {
			currentNPC.CloseDialogue();
			if (currentNPC.centerCameraInDialogue) {
				cmInterface.StopLookingAtPoint(currentNPC.gameObject.transform);
			}
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

	//called when the new respawn scene is loaded
	public static void StartPlayerRespawning() {
		pc.StartRespawning();
	}

	// TODO: encapsulate this better in TransitionManager
	public static void LoadScene(string sceneName, Beacon beacon=null) {
		if (beacon != null && beacon.leftScene != null) {
			string beaconSceneName = beacon.leftScene.ScenePath;

			if (SceneManager.GetActiveScene().path.Contains(beaconSceneName)) {
				beaconSceneName = beacon.rightScene.ScenePath;
			}

			gc.GetComponent<TransitionManager>().LoadScene(beaconSceneName, beacon);
		} else {
			gc.GetComponent<TransitionManager>().LoadScene(sceneName, beacon);
		}
	}

	public static void LoadScene(Beacon beacon) {
		string beaconSceneName = beacon.rightScene.ScenePath;

		if (SceneManager.GetActiveScene().path.Contains(beaconSceneName)) {
			beaconSceneName = beacon.leftScene.ScenePath;
		}

		gc.GetComponent<TransitionManager>().LoadScene(beaconSceneName, beacon);
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
		pc.EnableTrails();
		if (fade) {
			yield return new WaitForSecondsRealtime(0.5f);
			UnFadeToBlack();
		}
		GetComponentInChildren<PlayerCameraPoint>().SnapToPlayer();
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
		Debug.Log("adding item "+s.name);
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
			SaveManager.AddStates(item.gameStates);
		}
		inventory.AddItem(s, quiet);
		SaveManager.PushStateChange();
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
		playerStats.UnlockAbility(a);
	}

	public static void EnterMerchantDialogue(Merchant merchant) {
		pc.EnterCutscene();
		inventory.currentMerchant = merchant;
		OpenInventory();
	}

	public static void BoostStat(StatType statType, int amount) {
		switch (statType) {
			case StatType.HEALTH:
				pc.BoostHealth(amount);
				break;
			case StatType.ENERGY:
				pc.BoostEnergy(amount);
				break;
			case StatType.DAMAGE:
				pc.BoostBaseDamage(amount);
				break;
		}
		StatBoostUI.ReactToBoost(statType, amount);
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
