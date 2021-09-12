using UnityEngine;
using UnityEngine.UI;
using Rewired;

[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class ActionGlyph : MonoBehaviour {
	public ActionName action;

	GameObject unnamedGlyphTemplate;
	GameObject instantiatedGlyphTemplate;
	string lastController;
	SpriteRenderer gSprite;

	void Start() {
		unnamedGlyphTemplate = Resources.Load("GlyphNameTemplate") as GameObject;
		gSprite = GetComponent<SpriteRenderer>();
		UpdateGlyph();
	}

	void FixedUpdate() {
		string currentController = InputManager.GetLastControllerName();

		if (!currentController.Equals(lastController)) {
			UpdateGlyph();
		}

		lastController = currentController;
	}

	void UpdateGlyph() {
		if (!ReInput.isReady) {
			return;
		}

		Sprite glyphSprite = InputManager.GetGlyph(action);
		if (glyphSprite != null) {
			gSprite.sprite = glyphSprite;
			gSprite.enabled = true;
			if (instantiatedGlyphTemplate != null) {
				instantiatedGlyphTemplate.SetActive(false);
			}
		} else {
			gSprite.enabled = false;
			if (instantiatedGlyphTemplate != null) {
				PopulateControlInfo(instantiatedGlyphTemplate);
				instantiatedGlyphTemplate.SetActive(true);
			} else {
				instantiatedGlyphTemplate = Instantiate(unnamedGlyphTemplate, this.transform);
			}
		}
	}

	void PopulateControlInfo(GameObject glyphTemplate) {
		glyphTemplate.GetComponentInChildren<WorldPointCanvas>().target = this.transform;
		glyphTemplate.GetComponentInChildren<Text>().text = action.name;
	}
}
