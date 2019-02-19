using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : Interactable {

    public SeatType seatType;
    public bool requiresFacingLocalRight;
    public Vector2 sitPoint;

    protected override void ExtendedStart() {
        if (sitPoint == null) {
            sitPoint = this.transform.position;
        }
    }

    public override void Interact(GameObject player) {
        
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

        player.transform.position = this.sitPoint;
        pc.Sit();
    }

    void FlipPlayerToOrientation(PlayerController pc) {
        if ((transform.localScale.x == 1) ^ pc.facingRight) {
            pc.Flip();
        }
    }
}

public enum SeatType {
    BENCH,
    LEDGE
}