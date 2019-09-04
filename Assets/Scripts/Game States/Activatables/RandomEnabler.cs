using UnityEngine;
using System.Collections.Generic;

public class RandomEnabler : Activatable {

    public GameObject[] choices;

    override public void ActivateSwitch(bool b) {
        choices[Random.Range(0, choices.Length)].active = true;
    }
}