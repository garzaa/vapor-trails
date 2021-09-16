using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterButton : MonoBehaviour {
	public SaveContainer save;
	public Beacon beacon;

	public void OnSubmit() {
		GlobalController.LoadChapter(save, beacon);
	}
}
