using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalController : MonoBehaviour {

	public static GlobalController gc;
	public TitleText editorTitleText;
	public static TitleText titleText;
	static SignUI signUI;
	static BlackFadeUI blackoutUI;
	static DialogueUI dialogueUI;
	public static PlayerController pc;
	static bool dialogueOpen;
	static bool dialogueOpenedThisFrame = false;
	public static bool dialogueClosedThisFrame = false;
	static bool cutsceneNext = false;
	static NPC currentNPC;
	public static PlayerFollower playerFollower;
	static Save save;
	static bool inCutscene;

	static DialogueLine toActivate = null;

	static RespawnManager rm;

	void Awake() {
		gc = this;
		titleText = editorTitleText;
		DontDestroyOnLoad(this);
		dialogueUI = GetComponentInChildren<DialogueUI>();
		signUI = GetComponentInChildren<SignUI>();
		pc = GetComponentInChildren<PlayerController>();
		rm = GetComponent<RespawnManager>();
		playerFollower = gc.GetComponentInChildren<PlayerFollower>();
		save = gc.GetComponent<Save>();
		blackoutUI = GetComponentInChildren<BlackFadeUI>();
	}

	public static void ShowTitleText(string title, string subTitle = null) {
		titleText.ShowText(title, subTitle);
	}

	void LateUpdate() {
		if (
			dialogueOpen 
			&& (Input.GetButtonDown("Submit") || Input.GetButtonDown("Jump"))
			&& !dialogueOpenedThisFrame
			&& !inCutscene
			) {
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
					Debug.Log("entered blocking cutscnee");
					toActivate = null;
					return;
				}
				toActivate = null;
			}

			DialogueLine nextLine = currentNPC.GetNextLine();

			if (nextLine != null) {
				dialogueUI.RenderDialogueLine(nextLine);
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
		dialogueOpenedThisFrame = false;
		dialogueClosedThisFrame = false;
	}

	public static void EnterDialogue(NPC npc) {
		dialogueUI.ShowNameAndPicture(npc.GetCurrentLine());
		dialogueUI.Show();
		pc.EnterDialogue();
		currentNPC = npc;
		dialogueOpenedThisFrame = true;
	}

	public static void ExitDialogue() {
		Debug.Log("Exited Dialogue");
		dialogueOpen = false;
		dialogueUI.Hide();
		dialogueClosedThisFrame = true;
		pc.ExitDialogue();
		currentNPC.CloseDialogue();
		currentNPC = null;
	}

	public static void FinishOpeningLetterboxes() {
		Debug.Log("finished opening letterboxes");
		dialogueOpen = true;
		inCutscene = false;
		DialogueLine nextLine = currentNPC.GetNextLine();
		if (nextLine != null) {
			Debug.Log("rendering next line: " + nextLine.lineText);
			dialogueUI.RenderDialogueLine(nextLine);
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

	public static void Respawn() {
		rm.RespawnPlayer();
	}

	//called when the new respawn scene is loaded
	public static void StartPlayerRespawning() {
		pc.StartRespawning();
	}

	public static void AddGameFlag(string f) {
		if (!save.gameFlags.Contains(f)) {
			save.gameFlags.Add(f);
		}
	}

	public static void RemoveGameFlag(string f) {
		if (save.gameFlags.Contains(f)) {
			save.gameFlags.Remove(f);
		}
	}

	public static bool HasFlag(string f) {
		if (save == null) {
			return false;
		}
		return save.gameFlags.Contains(f);
	}

	public static void LoadScene(string sceneName, string beaconName=null) {
		gc.GetComponent<TransitionManager>().LoadScene(sceneName, beaconName);
	}

	public static void MovePlayerTo(string objectName, bool smoothing=false) {
		if (!smoothing) {
			playerFollower.DisableSmoothing();
		}
		pc.transform.position = GameObject.Find(objectName).transform.position;
		if (!smoothing) {
			playerFollower.EnableSmoothing();
		}
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
		foreach (ContainerUI c in gc.GetComponentsInChildren<ContainerUI>()) {
			c.Show();
		}
	}

	public static void HideUI() {
		foreach (ContainerUI c in gc.GetComponentsInChildren<ContainerUI>()) {
			c.Hide();
		}
	}

	public void ExitGame() {
		Application.Quit();
	}

	//called from a cutscene animation to finish it and resume dialogue
	public static void CutsceneCallback() {
		// show the dialogue UI
		dialogueUI.Show();
		Debug.Log("CutsceneCallback");
	}

	// hide dialogue UI but keep the player frozen
	public static void EnterCutscene() {
		Debug.Log("EnterCutscene");
		inCutscene = true;
		dialogueUI.Hide();
	}
}
