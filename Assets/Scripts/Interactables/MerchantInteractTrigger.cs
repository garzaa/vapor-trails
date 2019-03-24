using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Merchant))]
public class MerchantInteractTrigger : Interactable {

    Merchant merchant;

    void Start() {
        merchant = GetComponent<Merchant>();
    }

    override public void Interact(GameObject player) {
        base.Interact(player);
        GlobalController.EnterMerchantDialogue(this.merchant);
    }
}
