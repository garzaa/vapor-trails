using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
        // set a small delay to prevent accidental misinput
        choiceUI.StartCoroutine(choiceUI.OpenSelfChoices(choiceList));
    }

    public static void CloseChoices() {
        choiceUI.choices.SetActive(false);
        foreach (Transform child in choiceUI.choices.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    IEnumerator OpenSelfChoices(List<Choice> choiceList) {
    yield return new WaitForSeconds(0.3f);
        choices.SetActive(true);
        foreach (Choice choice in choiceList) {
            ChoiceBox box = Instantiate(
                choiceTemplate,
                Vector3.zero,
                Quaternion.identity,
                choices.transform
            ).GetComponent<ChoiceBox>();

            box.Populate(choice);
        }
        // select first choice
        Button b = choices.transform.GetChild(0).GetComponent<Button>();
        b.Select();
        // then highlight it (??????)
        b.OnSelect(null);
    }
}