public class AnimationBool : AnimationNode {
    public string boolName;

    protected override void OnInput() {
        animator.SetBool(boolName, input.value);
    }
}
