using UnityEngine.Events;

public class EventHurtbox : Hurtbox {

    public UnityEvent hitEvent;

    public override void PropagateHitEvent(Attack attack) {
        hitEvent.Invoke();        
    }
}