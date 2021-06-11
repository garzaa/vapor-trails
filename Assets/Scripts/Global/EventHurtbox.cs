using UnityEngine.Events;

public class EventHurtbox : Hurtbox {

    public UnityEvent hitEvent;

    public override void PropagateHitEvent(Attack attack) {
        base.PropagateHitEvent(attack);
        hitEvent.Invoke();        
    }
}
