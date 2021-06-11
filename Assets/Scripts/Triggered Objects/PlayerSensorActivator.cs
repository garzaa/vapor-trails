using UnityEngine;

public class PlayerSensorActivator : PlayerTriggeredObject {
    PlayerSensor playerSensor;

    override protected void Start() {
        base.Start();
        playerSensor = GetComponentInParent<PlayerSensor>();
    }

    public override void OnPlayerEnter() {
        playerSensor.running = true;
    }

    public override void OnPlayerExit() {
        playerSensor.running = false;
    }
}
