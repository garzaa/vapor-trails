using UnityEngine;

public class Boss : Enemy {
    public Activatable deathActivatable;

    public bool victoryEffectOnDeath;
    GameObject victoryEffect;

    BarUI bossHealthUI;
    
    public Color healthColor = new Color(221, 82, 82, 255);
    public bool startFightOnEnable = false;
    GameEvent startBossFight;
    GameEvent stopBossFight;

    public BossInfo bossInfo;
    public GameObject cameraAnchor;

    virtual protected void Start() {
        bossHealthUI = GlobalController.bossHealthUI;
        bossHealthUI.SetBarColor(healthColor);
        startBossFight = Resources.Load("ScriptableObjects/Events/StartBossFight") as GameEvent;
        stopBossFight = Resources.Load("ScriptableObjects/Events/StopBossFight") as GameEvent;

        victoryEffect = Resources.Load("Effects/Final Blow Prefab") as GameObject;

        if (startFightOnEnable) StartFight();
    }

    public void StartFight() {
        // state machine bugs
        if (this.hp <= 0) return;
        bossHealthUI.gameObject.SetActive(true);
        ShowIntro();
        startBossFight.Raise();
    }

    public void ShowIntro() {
        if (bossInfo.bossFightImage != null) {
            BossFightIntro.ShowIntro(this.bossInfo);
        }
    }

    override protected void Update() {
        base.Update();
        bossHealthUI.current = this.hp;
        bossHealthUI.max = this.totalHP;
    }

    override protected void Die() {
        bossHealthUI.gameObject.SetActive(false);
        if (deathActivatable != null) deathActivatable.Activate();
        if (victoryEffectOnDeath) Instantiate(victoryEffect, transform.position, Quaternion.identity, null);
        base.Die();
        stopBossFight.Raise();
    }

    void OnDisable() {
        if (startFightOnEnable) {
            stopBossFight.Raise();
            bossHealthUI.gameObject.SetActive(false);
        }
    }
}
