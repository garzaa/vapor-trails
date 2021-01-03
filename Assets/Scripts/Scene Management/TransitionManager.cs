using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour {

	Beacon currentBeacon;
	bool toPosition = false;
	Vector2 position = Vector2.zero;
	float targetVolume = 1f;
	float originalVolume = 1f;
	readonly float FADE_TIME = 1f;
	float elapsedTime;
	float transitionEndTime;
	public GameObject loadTextUI;
	public Text loadProgressText;
	
	public static SceneData sceneData;

	void Start() {
		//OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
		loadTextUI.SetActive(false);
	}

	void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

	void Update() {
		if (Time.time < transitionEndTime) {
			elapsedTime += Time.deltaTime;
			AudioListener.volume = Mathf.Lerp(originalVolume, targetVolume, elapsedTime/FADE_TIME);
		}
	}

	void FadeAudio(float targetVolume) {
		this.targetVolume = targetVolume;
		originalVolume = AudioListener.volume;
		elapsedTime = 0;
		transitionEndTime = Time.time + FADE_TIME;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		// reset everything and then re-enable according to scene data
		sceneData = null;
		loadTextUI.SetActive(false);
		FadeAudio(1);
		PlayerController pc = GlobalController.pc;
		pc.StopForcedWalking();
		pc.inCutscene = false;
		GlobalController.UnFadeToBlack();
		GlobalController.pauseEnabled = true;
		pc.UnLockInSpace();
		pc.Show();
		pc.ExitCutscene();
		pc.EnableShooting();

		GlobalController.ShowUI();

		if (currentBeacon != null) {
			GlobalController.MovePlayerToBeacon(currentBeacon);
			currentBeacon = null;
		} else if (SubwayManager.playerOnSubway) {
			SubwayManager.ArriveWithPlayer();
		} else if (toPosition) {
			GlobalController.MovePlayerTo(position, fade: true);
			toPosition = false;
		}

		SceneData sd;
		if (GameObject.Find("SceneData") != null) {
			sd = GameObject.Find("SceneData").GetComponent<SceneData>();
			sceneData = sd;

			if (sd.loadOtherSceneAtStart) {
				LoadScene(sd.otherSceneName, null, fade:false);
				return;
			}

			GlobalController.ShowTitleText(sd.title, sd.subTitle);

			if (sd.hideUI) {
				GlobalController.HideUI();
			} else {
				GlobalController.ShowUI();
			}

			if (sd.lockPlayer) {
				pc.EnterCutscene();
			} else if (sd.unlockPlayer) {
				pc.ExitCutscene();
			}

			if (sd.hidePlayer) {
				pc.Hide();
				pc.EnterCutscene();
			}

			if (sd.forceFaceRight && !pc.facingRight) {
				pc.Flip();
			}

			GlobalController.pauseEnabled = sd.enablePausing;

		}

		pc.EnableTriggers();

	}

	public void LoadSceneToPosition(string sceneName, Vector2 position) {
		this.toPosition = true;
		this.position = position;
		LoadScene(sceneName, null);
	}

	public void LoadScene(string sceneName, Beacon beacon, bool fade = true) {
		if (fade) GlobalController.FadeToBlack();
		this.currentBeacon = beacon;

		PlayerController pc = GlobalController.pc;
		pc.EnterCutscene();
		pc.DisableTriggers();

		StartCoroutine(LoadAsync(sceneName, fade));
	}

	IEnumerator LoadAsync(string sceneName, bool waitForFade)
    {
		if (waitForFade) {
			FadeAudio(0);
			yield return new WaitForSeconds(1);
		}

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		asyncLoad.allowSceneActivation = false;
		loadTextUI.SetActive(true);
		
        //wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone) {
			loadProgressText.text = asyncLoad.progress.ToString("P");
			if (asyncLoad.progress >= 0.9f) {
				asyncLoad.allowSceneActivation = true;
				GlobalController.pc.ExitCutscene();
			}

            yield return null;
        }
    }
	
}
