using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "AbilityItem", order = 2)]
public class AbilityItem : Item {
    public Ability ability;
    [TextArea] public string instructions;

    override public void OnPickup(bool quiet = false) {
        if (!quiet) GlobalController.abilityUIAnimator.GetComponent<AbilityGetUI>().GetItem(this);
        GlobalController.UnlockAbility(this.ability);
    }
}