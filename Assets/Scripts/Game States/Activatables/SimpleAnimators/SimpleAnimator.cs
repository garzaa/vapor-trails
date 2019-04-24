using UnityEngine;

public class SimpleAnimator : Activatable {
    public float initialDelay;
    protected bool running = false;

    protected virtual void Start() {
        Invoke("Activate", initialDelay);
        Init();
    }

    override public void Activate() {
        this.running = true;
    }

    override public void ActivateSwitch(bool b) {
        this.running = b;
    }

    void FixedUpdate() {
        if (!running) return;
        Draw();
    }

    virtual protected void Draw() {

    }

    virtual protected void Init() {

    }
}