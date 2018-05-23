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
	public Transform groundCheckLeft;
	public Transform groundCheckRight;

	//private variables
	public bool grounded = false;

	void Start () {
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		this.facingRight = false;
	}
	
	void Update () {
		UpdateGrounded();
		Move();
		Jump();
		CheckFlip();
	}

	void Move() {
		anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));

		if (HorizontalInput()) {
			if (Input.GetAxis("Horizontal") != 0) {
				rb2d.velocity = new Vector2(x:(Input.GetAxis("Horizontal") * MaxMoveSpeed), y:rb2d.velocity.y);
				movingRight = Input.GetAxis("Horizontal") > 0;
			}
		} 
		//if no movement, stop the player on the ground 
		else {
			rb2d.velocity = new Vector2(x:0, y:rb2d.velocity.y);
		}
	}

	void Jump() {
		if (grounded) {
			if (Input.GetButtonDown("Jump")) {
				rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:JumpSpeed);
			}
		}
		
		//emulate an analog jump
		//if the jump button is released
		if (Input.GetButtonUp("Jump") && rb2d.velocity.y > JUMP_CUTOFF) {
			//then decrease the y velocity to the jump cutoff
			rb2d.velocity = new Vector2(rb2d.velocity.x, JUMP_CUTOFF);
		}
	}

	void UpdateGrounded() {
		bool leftGrounded = Physics2D.Linecast(transform.position, groundCheckLeft.position, 1 << LayerMask.NameToLayer("Ground"));
		bool rightGrounded = Physics2D.Linecast(transform.position, groundCheckRight.position, 1 << LayerMask.NameToLayer("Ground"));
		Debug.DrawLine(transform.position, groundCheckLeft.position);
		Debug.DrawLine(transform.position, groundCheckRight.position);
		grounded = leftGrounded || rightGrounded;	
	}

	bool HorizontalInput() {
		return Input.GetAxis("Horizontal") != 0;
	}
}
