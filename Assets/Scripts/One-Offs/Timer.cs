 using UnityEngine;
 using System.Collections;
 using UnityEngine.UI;
 
 public class Timer : MonoBehaviour {
	public Text timerLabel;
	private float time;

	void Update() {
		time += Time.deltaTime;

		var minutes = time / 60; //Divide the guiTime by sixty to get the minutes.
		var seconds = time % 60;//Use the euclidean division for the seconds.
		var fraction = (time * 100) % 100;

		//update the label value
		timerLabel.text = string.Format ("{0:00}:{1:00}:{2:000}", minutes, seconds, fraction);
	}

	public void Reset() {
		time = 0;
	}
 }