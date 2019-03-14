using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : Interactable {

    public SeatType seatType;
    public bool requiresFacingLocalRight;

    public override void Interact(GameObject player) {
        RemovePrompt();
        Animator anim = player.GetComponent<Animator>();
        PlayerController pc = player.GetComponent<PlayerController>();

        if (requiresFacingLocalRight) {
            FlipPlayerToOrientation(pc);
        }

        anim.SetTrigger("Sit");
        switch (this.seatType) {
            case SeatType.BENCH:
                anim.SetTrigger("Bench");
                break;
            case SeatType.LEDGE:
                anim.SetTrigger("Ledge");
                break;
        }

        pc.transform.position = new Vector2(
            this.transform.position.x,
            pc.transform.position.y
        );

        pc.Sit();
    }

    void FlipPlayerToOrientation(PlayerController pc) {
        if ((transform.localScale.x > 0) ^ pc.facingRight) {
            pc.Flip();
        }
    }
}

public enum SeatType {
    BENCH,
    LEDGE
}