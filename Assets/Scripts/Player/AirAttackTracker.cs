using UnityEngine;
using System.Collections.Generic;

public class AirAttackTracker : MonoBehaviour {
    [SerializeField] List<string> usedAttacks = new List<string>();

    public void Add(string attackName) {
        usedAttacks.Add(attackName);
    }

    public bool Has(string attackName) {
        return usedAttacks.Contains(attackName);
    }

    public void Reset() {
        usedAttacks.Clear();
    }
}