using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChapterButton : MonoBehaviour, IPointerDownHandler {
	public SaveContainer save;
	public Beacon beacon;

	public void OnPointerDown(PointerEventData eventData) {
		GlobalController.LoadChapter(save, beacon);
	}
}
