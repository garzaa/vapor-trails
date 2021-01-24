using UnityEngine;
using UnityEngine.UI;

public class ChoiceBox : MonoBehaviour {
    [SerializeField] Text choiceText;
    Activatable activatable;

    public void Populate(Choice choice) {
        choiceText.text = choice.choiceText;
        activatable = choice.activatable;
    }

    public void OnSubmit() {
        if (!DialogueUI.LineFullyRendered()) {
            return;
        }

        // in the case of a generic "exit" button
        if (activatable != null) activatable.Activate();
        ChoiceUI.CloseChoices();
        // if clicked this frame, mimic a dialogue skip input
        // activating dialogue this frame breaks things
        if (!InputManager.GenericContinueInput()) {
            GlobalController.OnDialogueSkip();
        }
    }
}
