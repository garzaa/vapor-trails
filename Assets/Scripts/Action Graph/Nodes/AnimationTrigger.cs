public class AnimationTrigger : AnimationNode {
    public string triggerName;

    protected override void OnInput() {
        if (input.value) {
            animator.SetTrigger(triggerName);
        } else {
            animator.ResetTrigger(triggerName);
        }
    }
}
