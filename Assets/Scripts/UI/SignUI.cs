using UnityEngine;
using UnityEngine.UI;

public class SignUI : UIComponent {

	public Text signTextHidden;
	public Text signTextShown;

	Animator anim;
	Camera mainCamera;
	bool trackingWorld = false;
	public Vector2 worldPosition;

	void Start() {
		anim = GetComponent<Animator>();
		mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
	}

	void Update() {
		if (trackingWorld) {
			signTextHidden.GetComponent<RectTransform>().position = mainCamera.WorldToScreenPoint(worldPosition);
		}
	}

	public override void Show() {
		trackingWorld = true;
		anim.SetBool("Visible", true);
	}

	public override void Hide() {
		anim.SetBool("Visible", false);
	}

	//called from the end of the sign hiding animation
	public void FinishHiding() {
		trackingWorld = false;
	}

	public void SetText(string text) {
		signTextHidden.text = text;
		signTextShown.text = text;
	}

	public bool IsVisible() {
		return anim.GetBool("Visible");
	}

	public void SetPosition(Vector2 worldPosition) {
		//keeps the sign corresponding with this vector2 in world space
		this.worldPosition.x = worldPosition.x;
		this.worldPosition.y = worldPosition.y;
	}
}
