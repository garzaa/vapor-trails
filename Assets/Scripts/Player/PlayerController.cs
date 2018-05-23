using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity {

	//public constants
	float MaxMoveSpeed = 2.5f;
	float JumpSpeed = 4;
	float JUMP_CUTOFF = 2.0f;

	//linked components
	Rigidbody2D rb2d;
	Animator anim;

	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		this.facingRight = false;
	}
	
	void Update () {
		Move();
		Jump();
	}

	void Move() {
		anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));

		if (HorizontalInput()) {
			if (Input.GetAxis("Horizontal") != 0) {
				rb2d.velocity = new Vector2(x:(Input.GetAxis("Horizontal") * MaxMoveSpeed), y:rb2d.velocity.y);
			}
		} 
		//if no movement, stop the player on the ground 
		else {
			rb2d.velocity = new Vector2(x:0, y:rb2d.velocity.y);
		}

		//flip sprites depending on movement direction
        if (!facingRight && rb2d.velocity.x > 0)
        {
            Flip();
        }
        else if (facingRight && rb2d.velocity.x < 0)
        {
            Flip();
        }
	}

	void Jump() {
		if (Input.GetButtonDown("Jump")) {
			rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:JumpSpeed);
		}
		//emulate an analog jump
        //if the jump button is released
        if (Input.GetButtonUp("Jump") && rb2d.velocity.y > JUMP_CUTOFF) {
            //then decrease the y velocity to the jump cutoff
            rb2d.velocity = new Vector2(rb2d.velocity.x, JUMP_CUTOFF);
        }
	}

	bool HorizontalInput() {
		return Input.GetAxis("Horizontal") != 0;
	}
}
