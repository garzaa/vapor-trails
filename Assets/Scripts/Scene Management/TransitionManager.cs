using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour {

	string currentBeaconName = null;

	void Start() {
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}

	void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		// reset everything and then re-enable according to scene data
		GlobalController.pc.EndSupercruise();
		GlobalController.pc.StopDashing();
		GlobalController.pc.StopForcedWalking();
		GlobalController.UnFadeToBlack();
		GlobalController.playerFollower.EnableFollowing();
		GlobalController.playerFollower.FollowPlayer();
		GlobalController.playerFollower.EnableSmoothing();
		PlayerController pc = GlobalController.pc;
		pc.LockInSpace();
		pc.SetInvincible(false);
		pc.Freeze();
		pc.Show();

		if (!string.IsNullOrEmpty(currentBeaconName)) {
			//in case it was disabled in the previous scene
			GlobalController.playerFollower.EnableFollowing();
			GlobalController.playerFollower.FollowPlayer();
			GlobalController.MovePlayerTo(currentBeaconName);
			currentBeaconName = null;
		}
		SceneData sd;
		if (GameObject.Find("SceneData") != null) {
			sd = GameObject.Find("SceneData").GetComponent<SceneData>();

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
				pc.LockInSpace();
				pc.SetInvincible(true);
				pc.Freeze();
			}

			if (sd.hidePlayer) {
				pc.Hide();
			}

		}

		PlayerTriggeredObject triggered = GlobalController.pc.CheckInsideTrigger();
		if (triggered != null) {
			print(triggered.name);
			triggered.OnPlayerEnter();
		}
	}

	public void LoadScene(string sceneName, string beaconName, bool fade = true) {
		if (SceneManager.GetActiveScene().name != sceneName && fade) GlobalController.FadeToBlack();
		this.currentBeaconName = beaconName;
		GlobalController.playerFollower.DisableSmoothing();
		StartCoroutine(LoadAsync(sceneName));
	}

	IEnumerator LoadAsync(string sceneName)
    {
        // The Application loads the Scene in the background at the same time as the current Scene.
        //This is particularly good for creating loading screens. You could also load the Scene by build //number.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
	
}
