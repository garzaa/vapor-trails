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
		GlobalController.pc.EndSupercruise();
		GlobalController.pc.StopDashing();
		GlobalController.pc.StopForcedWalking();
		GlobalController.UnFadeToBlack();
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
				LoadSceneFade(sd.otherSceneName, null);
				return;
			}

			GlobalController.ShowTitleText(sd.title, sd.subTitle);

			if (sd.hideUI) {
				GlobalController.HideUI();
			} else {
				GlobalController.ShowUI();
			}

		}

		PlayerTriggeredObject triggered = GlobalController.pc.CheckInsideTrigger();
		if (triggered != null) {
			triggered.OnPlayerEnter();
		}

		GlobalController.playerFollower.EnableFollowing();
		GlobalController.playerFollower.FollowPlayer();
		GlobalController.playerFollower.EnableSmoothing();
	}

	public void LoadSceneFade(string sceneName, string beaconName) {
		print("trying to load scene "+sceneName+" from current scene "+SceneManager.GetActiveScene().name);
		if (SceneManager.GetActiveScene().name != sceneName) GlobalController.FadeToBlack();
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
