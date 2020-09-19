using UnityEngine;

public class Boss : Enemy {
    public Activatable deathActivatable;

    BarUI bossHealthUI;
    
    public Color healthColor = new Color(221, 82, 82, 255);
    public bool startFightOnEnable = false;
    GameEvent startBossFight;
    GameEvent stopBossFight;

    virtual protected void Start() {
        bossHealthUI = GlobalController.bossHealthUI;
        bossHealthUI.SetBarColor(healthColor);
        startBossFight = Resources.Load("ScriptableObjects/Events/StartBossFight") as GameEvent;
        stopBossFight = Resources.Load("ScriptableObjects/Events/StopBossFight") as GameEvent;

        if (startFightOnEnable) StartFight();
    }

    public void StartFight() {
        // state machine bugs
        if (this.hp <= 0) return;
        bossHealthUI.gameObject.SetActive(true);
        startBossFight.Raise();
    }

    override protected void Update() {
        base.Update();
        bossHealthUI.current = this.hp;
        bossHealthUI.max = this.totalHP;
    }

    override protected void Die() {
        bossHealthUI.gameObject.SetActive(false);
        if (deathActivatable != null) deathActivatable.Activate();
        base.Die();
        stopBossFight.Raise();
    }

    void OnDisable() {
        if (startFightOnEnable) {
            stopBossFight.Raise();
        }
    }
}