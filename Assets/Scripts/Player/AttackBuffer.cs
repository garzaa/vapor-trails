using UnityEngine;
using System.Collections;

public class AttackBuffer : MonoBehaviour {
    AttackType attackType = AttackType.NONE;
    Vector2 direction = Vector2.zero;

    Coroutine clearBuffer;

    bool punch, kick;
    const float inputThreshold = 0.2f;

    public AttackType type {
        get {
            return attackType;
        }
    }


    public bool ready {
        get {
            return attackType != AttackType.NONE;
        }
    }

    void Update() {
        punch = InputManager.ButtonDown(Buttons.PUNCH);
        kick = InputManager.ButtonDown(Buttons.KICK);
        if (punch || kick) {
            ResetBufferTimeout();
            
            if (punch) attackType = AttackType.PUNCH;
            else attackType = AttackType.KICK;

            Vector2 ls = InputManager.LeftStick();

            direction = new Vector2(
                    Mathf.Sign(ls.x),
                    Mathf.Approximately(ls.y, 0) ? 0 : ClampZero(ls.y)
                );
            direction = direction * GlobalController.pc.ForwardVector() * new Vector2(1, 2);

            StartBufferTimeout();
        }
    }

    float ClampZero(float input) {
        if (Mathf.Abs(input) < inputThreshold) {
            return 0;
        }
        return Mathf.Sign(input);
    }

    IEnumerator ClearInputs() {
        yield return new WaitForSecondsRealtime(InputManager.GetInputBufferDuration());
        Clear();
    }

    // can be called externally
    public void Clear() {
        ResetBufferTimeout();
        attackType = AttackType.NONE;
        direction = Vector2.zero;
    }

    public bool HasDirection(AttackDirection d) {
        if (d == AttackDirection.ANY) return true;
        return (d==(AttackDirection)direction.x || d==(AttackDirection)direction.y);
    }

    void ResetBufferTimeout() {
        if (clearBuffer != null) StopCoroutine(clearBuffer);
    }

    void StartBufferTimeout() {
        clearBuffer = StartCoroutine(ClearInputs());
    }
}

public enum AttackDirection {
    ANY = 0,
    FORWARD = 1,
    BACKWARD = -1,
    UP = 2,
    DOWN = -2,
}

public enum AttackType {
    NONE = 0,
    PUNCH = 1,
    KICK = 2,
}
