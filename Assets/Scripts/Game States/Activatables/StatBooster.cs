using UnityEngine;
using System.Collections.Generic;

public class StatBooster : Activatable {
    public int amount;
    public StatType statType;

    override public void Activate() {
        GlobalController.BoostStat(statType, amount);
    }
}