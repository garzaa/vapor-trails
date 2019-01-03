using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour {

	string currentBeaconName = null;
	bool frozePlayerBeforeTransition = false;
	bool closedJets = false;

	void Start() {
		//OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}

	void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		// reset everything and then re-enable according to scene data
		PlayerController pc = GlobalController.pc;
		pc.StopForcedWalking();
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

		if (!string.IsNullOrEmpty(currentBeaconName)) {
			//in case it was disabled in the previous scene
			GlobalController.MovePlayerTo(currentBeaconName);
			GlobalController.playerFollower.SnapToPlayer();
			GlobalController.playerFollower.EnableFollowing();
			GlobalController.playerFollower.FollowPlayer();
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
				pc.DisableShooting();
			} else {

			}

			if (sd.hidePlayer) {
				pc.Hide();
			}

			GlobalController.pauseEnabled = sd.enablePausing;

		}

		PlayerTriggeredObject triggered = GlobalController.pc.CheckInsideTrigger();
		if (triggered != null) {
			print(triggered.name);
			triggered.OnPlayerEnter();
		}
 
		//then reopen the jets
		if (closedJets) {
			pc.wings.EnableJetTrails();
		}
	}

	public void LoadScene(string sceneName, string beaconName, bool fade = true) {
		if (SceneManager.GetActiveScene().name != sceneName && fade) GlobalController.FadeToBlack();
		this.currentBeaconName = beaconName;
		GlobalController.playerFollower.DisableFollowing();
		GlobalController.playerFollower.DisableSmoothing();

		//preserve dash/supercruise state between scenes
		PlayerController pc = GlobalController.pc;
		pc.LockInSpace();
		//if the wings are open, don't leave a trail if they move around in the level
		closedJets = false; 
		if (pc.wings != null) {
			if (pc.wings.HasOpenJets()) {
				pc.wings.DisableJetTrails();
				closedJets = true;
			}
		}
		frozePlayerBeforeTransition = (pc.dashing || pc.supercruise);

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
