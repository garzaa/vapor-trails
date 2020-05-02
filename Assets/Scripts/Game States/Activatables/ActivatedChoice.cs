using UnityEngine;
using System.Collections.Generic;

public class ActivatedChoice : Activatable {

    [SerializeField] bool useNamedChildren = true;
    [SerializeField] List<Choice> choices;

    override public void ActivateSwitch(bool b) {
        if (b) {
            if (useNamedChildren) {
                List<Choice> childChoices = new List<Choice>();
                foreach (Transform t in transform) {
                    if (t.gameObject.activeSelf) {
                        childChoices.Add(new Choice(
                            t.name,
                            t.GetComponent<Activatable>()
                        ));
                    }
                }
                ChoiceUI.OpenChoices(childChoices);
            } else {
                ChoiceUI.OpenChoices(this.choices);
            }
        }
    }
}