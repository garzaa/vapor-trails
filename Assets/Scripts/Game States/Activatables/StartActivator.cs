using UnityEngine;

public class StartActivator : Activator {
    override public void Start() {
        base.Start();
        Activate();
    }
}