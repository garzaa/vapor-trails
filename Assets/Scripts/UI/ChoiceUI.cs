using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChoiceUI : MonoBehaviour {
    [SerializeField] GameObject choices;
    [SerializeField] GameObject choiceTemplate;

    static ChoiceUI choiceUI;

    void Start() {
        choiceUI = this;
        CloseChoices();
    }

    public static void OpenChoices(List<Choice> choiceList) {
        //populate with a new choice item for everything
        choiceUI.choices.SetActive(true);
        foreach (Choice choice in choiceList) {
            ChoiceBox box = Instantiate(
                choiceUI.choiceTemplate,
                Vector3.zero,
                Quaternion.identity,
                choiceUI.choices.transform
            ).GetComponent<ChoiceBox>();

            box.Populate(choice);
        }
        // select first choice
        Button b = choiceUI.choices.transform.GetChild(0).GetComponent<Button>();
        b.Select();
        // then highlight it (??????)
        b.OnSelect(null);
    }

    public static void CloseChoices() {
        choiceUI.choices.SetActive(false);
        foreach (Transform child in choiceUI.choices.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }
}