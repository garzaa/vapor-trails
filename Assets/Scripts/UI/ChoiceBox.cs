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
        activatable.Activate();
        ChoiceUI.CloseChoices();
    }
}