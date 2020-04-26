using UnityEngine;
using System.Collections.Generic;

public class ActivatedChoice : Activatable {

    public List<Choice> choices;

    override public void ActivateSwitch(bool b) {
        if (b) {
            ChoiceUI.OpenChoices(this.choices);
        }
    }
}