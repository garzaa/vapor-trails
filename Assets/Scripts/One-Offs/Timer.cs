using UnityEngine;
using UnityEngine.UI;
using System;
 
public class Timer : PersistentObject {
	public Text timerLabel;
	float time;
	bool enabledForThisScene;
	bool paused;

	protected override void Start() {
		base.Start();
		enabledForThisScene = true;
		if (TransitionManager.sceneData != null && TransitionManager.sceneData.pauseSpeedrunTimer) {
			enabledForThisScene = false;
		}
	}

	public override void ConstructFromSerialized(SerializedPersistentObject s) {
		if (s == null) return;
		persistentProperties = s.persistentProperties;
		time = 0;
		if (persistentProperties.ContainsKey(nameof(time))) {
			time = (float) persistentProperties[nameof(time)];
		}
	}

	void Update() {
		if (!enabledForThisScene) return;
		if (paused) return;

		time += Time.unscaledDeltaTime;
		
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		timerLabel.text = timeSpan.ToString(@"hh\:mm\:ss\.ff");

		persistentProperties[nameof(time)] = time;
		SaveObjectState();
	}

	public void Pause() {
		paused = true;
	}

	public void Resume() {
		paused = false;
	}

	override public string GetID() {
		return nameof(time);
	}
}
