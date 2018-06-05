using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour {

	void Start() {
		OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
	}

	void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) { 
		/*
		if (blackScreen) {
			anim.SetBool("blackScreen", false);
		}
		 */
		SceneData sd;
		if (GameObject.Find("SceneData") != null) {
			sd = GameObject.Find("SceneData").GetComponent<SceneData>();

			if (sd.loadOtherSceneAtStart) {
				LoadSceneFade(sd.otherSceneName);
				return;
			}

			GlobalController.ShowTitleText(sd.title, sd.subTitle);

		}
		
	}

	public void LoadSceneFade(string sceneName) {
		/*
		anim.SetBool("blackScreen", true);
		*/
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
