using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterButton : MonoBehaviour {
	public GameCheckpoint checkpoint;
	public Beacon beacon;

	public void OnSubmit() {
		SaveManager.LoadChapter(checkpoint, beacon);
	}
}
