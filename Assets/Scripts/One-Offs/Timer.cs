using UnityEngine;
using UnityEngine.UI;
using System;
 
public class Timer : PersistentObject {
	public Text timerLabel;
	float time;

	public override void ConstructFromSerialized(SerializedPersistentObject s) {
		if (s == null) return;
		persistentProperties = s.persistentProperties;
		time = 0;
		if (persistentProperties.ContainsKey(nameof(time))) {
			time = (float) persistentProperties[nameof(time)];
		}
	}

	void Update() {
		time += Time.unscaledDeltaTime;
		
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);
		timerLabel.text = timeSpan.ToString(@"hh\:mm\:ss\.ff");

		persistentProperties[nameof(time)] = time;
		SaveObjectState();
	}

	override public string GetID() {
		return nameof(time);
	}
}
