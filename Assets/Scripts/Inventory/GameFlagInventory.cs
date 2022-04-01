using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlagInventory : MonoBehaviour {
    public GameFlag flag;
    public Merchant merchant;
    public List<Item> items;

    void Start() {
        if (SaveManager.HasFlag(this.flag)) {
            merchant.AddGameFlagInventory(this);
        }
    }
}
