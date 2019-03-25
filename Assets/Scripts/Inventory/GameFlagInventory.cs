using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlagInventory : MonoBehaviour {
    public GameFlag flag;
    public Merchant merchant;
    public List<ItemWrapper> items;

    void Start() {
        if (GlobalController.HasFlag(this.flag)) {
            merchant.AddGameFlagInventory(this);
        }
    }
}