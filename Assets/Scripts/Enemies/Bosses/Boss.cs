using UnityEngine;

public class Boss : Enemy {
    public Activatable deathActivatable;
    public Color healthColor; //= new Color(221, 82, 82, 255);

    BarUI bossHealthUI;

    void Start() {
        bossHealthUI = GlobalController.bossHealthUI;
        bossHealthUI.SetBarColor(healthColor);
    }

    public void StartFight() {
        bossHealthUI.gameObject.SetActive(true);
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
    }
}