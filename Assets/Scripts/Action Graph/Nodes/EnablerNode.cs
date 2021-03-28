using UnityEngine;
using System.Collections.Generic;

public class EnablerNode : ActionNode {
    public GameObject target;

    protected override void OnInput() {
        target.SetActive(input.value);
    }
}
