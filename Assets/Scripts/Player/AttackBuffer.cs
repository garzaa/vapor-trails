using UnityEngine;
using System.Collections;

public class AttackBuffer : MonoBehaviour {
    const float buffer = 0.1f;
    [SerializeField, NodeEnum] AttackType attackType = AttackType.NONE;
    [SerializeField, NodeEnum] Vector2 direction = Vector2.zero;

    Coroutine clearBuffer;

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
        bool punch = Input.GetButtonDown(Buttons.PUNCH);
        bool kick = Input.GetButtonDown(Buttons.KICK);
        if (punch || kick) {
            ResetBufferTimeout();
            
            if (punch) attackType = AttackType.PUNCH;
            else attackType = AttackType.KICK;

            direction = InputManager.LeftStick().normalized * GlobalController.pc.ForwardVector() * new Vector2(1, 2);

            Debug.Log("Setting attack buffer");
            Debug.Log(direction.ToString());

            //StartBufferTimeout();
        }
    }

    IEnumerator ClearInputs() {
        yield return new WaitForSecondsRealtime(buffer);
        Clear();
    }


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
    KICK = 2
}
