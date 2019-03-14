using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Merchant))]
public class MerchantInteractTrigger : Interactable {
    override public void Interact(GameObject player) {
        base.Interact(player);
        
    }
}
