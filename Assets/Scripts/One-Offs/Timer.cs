using UnityEngine;
using UnityEngine.UI;
using System;
 
public class Timer : PersistentObject {
	public Text timerLabel;
	float time;
	bool enabledForThisScene = true;
	bool paused;


	protected override void SetDefaults() {
		SetDefault(nameof(time), 0f);
		time = GetFloat(nameof(time));
	}

	void Start() {
		if (TransitionManager.sceneData != null && TransitionManager.sceneData.pauseSpeedrunTimer) {
			enabledForThisScene = false;
		}
	}

	void Update() {
		timerLabel.text = TimeSpan.FromSeconds(time).ToString(@"hh\:mm\:ss\.ff");
		
		if (!enabledForThisScene) return;
		if (paused) return;

		time += Time.unscaledDeltaTime;
		
		SetProperty(nameof(time), time);
	}

	public void Pause() {
		paused = true;
	}

	public void Resume() {
		paused = false;
	}
}
