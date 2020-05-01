using UnityEngine;
using UnityEngine.UI;

public class StatBoostUI : CloseableUI {

    static Animator animator;
    [SerializeField] static StatBoostUI sb;
    [SerializeField] Text titleText;
    [SerializeField] Text deltaText;
    float uiOpenTime;

    void Start() {
        if (animator == null) animator = GetComponent<Animator>();
        if (sb == null) sb = this;
    }

    public static void ReactToBoost(StatType statType, int amount) {
        sb.ShowBoostUI(statType, amount);
    }

    void ShowBoostUI(StatType statType, int amount) {
        uiOpenTime = Time.time;
        titleText.text = statType.ToString()+" BOOSTED";
        deltaText.text = "Increased by " + amount;
        base.Open();
    }

    void LateUpdate() {
        if (open && (Time.time > uiOpenTime+0.5f) && InputManager.GenericContinueInput()) {
            base.Close();
        }
    }
}