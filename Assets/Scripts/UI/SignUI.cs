using UnityEngine;
using UnityEngine.UI;

public class SignUI : UIComponent {

	public Text signTextHidden;
	public Text signTextShown;

	Animator anim;

	public override void Show() {
		anim.SetBool("Visible", true);
	}

	public override void Hide() {
		anim.SetBool("Visible", false);
	}

	public void SetText(string text) {
		signTextHidden.text = text;
		signTextShown.text = text;
	}
}
