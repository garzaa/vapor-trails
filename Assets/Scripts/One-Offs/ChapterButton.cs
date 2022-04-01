using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterButton : MonoBehaviour {
	public SaveContainer save;
	public Beacon beacon;

	public void OnSubmit() {
		SaveManager.LoadChapter(save, beacon);
	}
}
