using UnityEngine;

public class Boss : Enemy {
    public Activatable deathActivatable;

    BarUI bossHealthUI;

    override protected void OnEnable() {
        base.OnEnable();
        bossHealthUI = GlobalController.bossHealthUI;
        bossHealthUI.gameObject.SetActive(true);
    }

    override protected void Update() {
        base.Update();
        bossHealthUI.current = this.hp;
        bossHealthUI.max = this.totalHP;
    }

    override protected void Die() {
        bossHealthUI.gameObject.SetActive(false);
        deathActivatable.Activate();
        base.Die();
    }
}