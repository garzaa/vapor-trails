using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour {

	Beacon currentBeacon = Beacon.None;
	bool frozePlayerBeforeTransition = false;
	bool toPosition = false;
	Vector2 position = Vector2.zero;
	float targetVolume = 1f;
	float originalVolume = 1f;
	readonly float FADE_TIME = 1f;
	float elapsedTime;
	float transitionEndTime;
	public GameObject loadTextUI;
	public Text loadProgressText;

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
		loadTextUI.SetActive(false);
		FadeAudio(1);
		PlayerController pc = GlobalController.pc;
		pc.StopForcedWalking();
		pc.inCutscene = false;
		GlobalController.UnFadeToBlack();
		GlobalController.playerFollower.EnableFollowing();
		GlobalController.playerFollower.FollowPlayer();
		GlobalController.playerFollower.EnableSmoothing();
		GlobalController.pauseEnabled = true;
		pc.UnLockInSpace();
		// if the PC wasn't dashing or in supercruise
		if (!frozePlayerBeforeTransition) {
			pc.SetInvincible(false);
			pc.UnFreeze();
			//and reset to wait for the next transition
			frozePlayerBeforeTransition = false;
		}
		pc.Show();
		pc.EnableShooting();

		GlobalController.ShowUI();

		if (currentBeacon != Beacon.None) {
			GlobalController.MovePlayerToBeacon(currentBeacon);
			GlobalController.playerFollower.SnapToPlayer();
			GlobalController.playerFollower.EnableFollowing();
			GlobalController.playerFollower.FollowPlayer();
			currentBeacon = Beacon.None;
		} else if (SubwayManager.playerOnSubway) {
			SubwayManager.ArriveWithPlayer();
		} else if (toPosition) {
			GlobalController.MovePlayerTo(position);
			toPosition = false;
			GlobalController.playerFollower.SnapToPlayer();
			GlobalController.playerFollower.EnableFollowing();
			GlobalController.playerFollower.FollowPlayer();
		}

		SceneData sd;
		if (GameObject.Find("SceneData") != null) {
			sd = GameObject.Find("SceneData").GetComponent<SceneData>();

			if (sd.loadOtherSceneAtStart) {
				LoadScene(sd.otherSceneName, Beacon.None, fade:false);
				return;
			}

			GlobalController.ShowTitleText(sd.title, sd.subTitle);

			if (sd.hideUI) {
				GlobalController.HideUI();
			} else {
				GlobalController.ShowUI();
			}

			if (sd.lockPlayer) {
				pc.LockInSpace();
				pc.SetInvincible(true);
				pc.Freeze();
				pc.DisableShooting();
			} else if (sd.unlockPlayer) {
				pc.UnLockInSpace();
				pc.SetInvincible(false);
				pc.UnFreeze();
				pc.EnableShooting();
			}

			if (sd.hidePlayer) {
				pc.Hide();
			}

			if (sd.forceFaceRight && !pc.facingRight) {
				pc.Flip();
			}

			GlobalController.pauseEnabled = sd.enablePausing;

		}

		PlayerTriggeredObject triggered = GlobalController.pc.CheckInsideTrigger();
		if (triggered != null) {
			triggered.OnPlayerEnter();
		}

	}

	public void LoadSceneToPosition(string sceneName, Vector2 position) {
		this.toPosition = true;
		this.position = position;
		LoadScene(sceneName, Beacon.None);
	}

	public void LoadScene(string sceneName, Beacon beacon, bool fade = true) {
		if (fade) GlobalController.FadeToBlack();
		this.currentBeacon = beacon;
		GlobalController.playerFollower.DisableFollowing();
		GlobalController.playerFollower.DisableSmoothing();

		//preserve dash/supercruise state between scenes
		PlayerController pc = GlobalController.pc;
		pc.LockInSpace();
		frozePlayerBeforeTransition = pc.supercruise;

		StartCoroutine(LoadAsync(sceneName, fade));
	}

	IEnumerator LoadAsync(string sceneName, bool waitForFade)
    {
		if (waitForFade) {
			FadeAudio(0);
			yield return new WaitForSeconds(1);
		}

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		loadTextUI.SetActive(true);
		asyncLoad.allowSceneActivation = false;
		
        //wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone) {
			loadProgressText.text = asyncLoad.progress.ToString("P");
			if (asyncLoad.progress >= .9f) {
				asyncLoad.allowSceneActivation = true;
			}

            yield return null;
        }
    }
	
}
