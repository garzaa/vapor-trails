using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour {

	public Transition transition;
	public static SceneData sceneData;

	float targetVolume = 1f;
	float originalVolume = 1f;
	const float FADE_TIME = 1f;
	float elapsedTime;
	float transitionEndTime;
	public GameObject loadTextUI;
	public Text loadProgressText;

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		loadTextUI.SetActive(false);
		ApplySceneData();
		LoadFromTransition();
		transition.Clear();
	}

	void OnEnable() {
		loadTextUI.SetActive(false);
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

	private void LoadFromTransition() {
		GlobalController.UnFadeToBlack();
		FadeAudio(1);

		if (transition.subway) {
			SubwayManager.ArriveWithPlayer();
		} else if (transition.beacon != null) {
			GlobalController.MovePlayerToBeacon(transition.beacon);
		} else {
			GlobalController.MovePlayerTo(transition.position);
		}
	}

	private void ApplySceneData() {
		SceneData sd = GameObject.FindObjectOfType<SceneData>();
		if (sd != null) {
			TransitionManager.sceneData = sd;
			if (sd.loadOtherSceneAtStart) {
				LoadScene(sd.otherSceneName, null, fade: false);
				return;
			}

			GlobalController.ShowTitleText(sd.title, sd.subTitle);
			PlayerController pc = GlobalController.pc;

			if (sd.hideUI) {
				GlobalController.HideUI();
			} else {
				GlobalController.ShowUI();
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
	}

	public void LoadSceneToPosition(string sceneName, Vector2 position) {
		transition.position = position;
		LoadScene(sceneName, null);
	}

	public void LoadScene(string sceneName, Beacon beacon, bool fade = true) {
		if (fade) GlobalController.FadeToBlack();
		transition.beacon = beacon;

		GlobalController.pc.EnterCutscene();

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
			}

            yield return null;
        }
    }
	
}
