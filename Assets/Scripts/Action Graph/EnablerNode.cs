using UnityEngine;
using System.Collections.Generic;

public class EnablerNode : IActionNode {
    public List<GameObject> targets;

    protected override void OnInput() {
        foreach (GameObject g in targets) {
            g.SetActive(input.value);
        }
    }
}
