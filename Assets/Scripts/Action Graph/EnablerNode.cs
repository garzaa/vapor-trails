using UnityEngine;
using System.Collections.Generic;

public class EnablerNode : ActionNode {
    public List<GameObject> targets;

    protected override void OnInput() {
        foreach (GameObject g in targets) {
            if (g == null) {
                Debug.Log("retard");
                continue;
            }
            g.SetActive(input.value);
        }
    }
}
