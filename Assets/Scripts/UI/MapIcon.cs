using UnityEngine;

public class MapIcon : MonoBehaviour {
    public Sprite icon;

    void Start() {
        SpawnMapIcon();
    }

    void SpawnMapIcon() {
		GameObject icon = (GameObject) Instantiate(
			Resources.Load("NPCIcon"),
			this.transform.position,
			Quaternion.identity,
			this.transform
		);
		icon.GetComponent<SpriteRenderer>().sprite = this.icon;
	}
}