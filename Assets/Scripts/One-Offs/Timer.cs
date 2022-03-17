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
	}

	void Start() {
		if (TransitionManager.sceneData != null && TransitionManager.sceneData.pauseSpeedrunTimer) {
			enabledForThisScene = false;
		}
		time = GetProperty<float>(nameof(time));
	}

	void Update() {
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		timerLabel.text = timeSpan.ToString(@"hh\:mm\:ss\.ff");
		
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
