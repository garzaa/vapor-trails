using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity {

	//constants
	float MaxMoveSpeed = 2.5f;
	float JumpSpeed = 4.5f;
	float JUMP_CUTOFF = 2.0f;
	int maxAirJumps = 1;
	float terminalVelocity = -5f;
	public int baseAttackDamage = 1;
	public bool terminalFalling = false;
	float dashSpeed = 8;
	int flashTimes = 5;
	bool vaporDash = true;
	public bool damageDash = false;

	//linked components
	Rigidbody2D rb2d;
	Animator anim;
	public Transform groundCheckLeft;
	public Transform groundCheckRight;
	public WallCheck wcForwards;
	public GameObject hurtboxes;
	SpriteRenderer spr;
	Material defaultMaterial;
    Material cyanMaterial;

	//variables
	bool grounded = false;
	bool touchingWall = false;
	public int airJumps;
	public bool midSwing = false;
	bool dashCooldown = false;
	public bool dashing = false;
	bool parrying = false;
	Vector2 preDashVelocity;
	bool invincible = false;

	void Start () {
		airJumps = maxAirJumps;
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		this.facingRight = false;
		spr = this.GetComponent<SpriteRenderer>();
        defaultMaterial = spr.material;
        cyanMaterial = Resources.Load<Material>("Shaders/CyanFlash");
	}
	
	void Update () {
		UpdateGrounded();
		UpdateWallSliding();
		Move();
		Attack();
		Jump();
		CheckFlip();
	}

	void Attack() {
		if (Input.GetButtonDown("Attack")) {
			anim.SetTrigger("Attack");
		}
	}

	void Move() {

		if (!frozen) {
			anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));

			if (HorizontalInput() && !midSwing) {
				if (Input.GetAxis("Horizontal") != 0) {
					rb2d.velocity = new Vector2(x:(Input.GetAxis("Horizontal") * MaxMoveSpeed), y:rb2d.velocity.y);
					movingRight = Input.GetAxis("Horizontal") > 0;
				}
			} 
			//if no movement, stop the player on the ground 
			else if (grounded) {
				rb2d.velocity = new Vector2(x:0, y:rb2d.velocity.y);
			} 
			//or slow them down in the air if they haven't just walljumped
			else {
				rb2d.velocity = new Vector2(
					x:rb2d.velocity.x / 1.05f,
					y:rb2d.velocity.y
				);
			}

			if (Input.GetButtonDown("Dash")) {
				Dash();
			}
		}

		if (dashing) {
            rb2d.velocity = new Vector2(dashSpeed * GetForwardScalar(), 0);
        }

		if (rb2d.velocity.y < terminalVelocity) {
			rb2d.velocity = new Vector2(rb2d.velocity.x, terminalVelocity);
			terminalFalling = true;
		} else {
			terminalFalling = false;
		}
	}

	void Jump() {
		if (Input.GetButtonDown("Jump") && !frozen) {
			if (grounded) {
				rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:JumpSpeed);
				InterruptAttack();
				anim.SetTrigger("Jump");
			}
			else if (touchingWall) {
				FreezeFor(.1f);
				rb2d.velocity = new Vector2(x:-2 * GetForwardScalar(), y:JumpSpeed);
				airJumps--;
				anim.SetTrigger("Jump");
			}
			else if (airJumps > 0) {
				rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:JumpSpeed);
				airJumps--;
				anim.SetTrigger("Jump");
			}
		}

		//emulate an analog jump
		//if the jump button is released
		if (Input.GetButtonUp("Jump") && rb2d.velocity.y > JUMP_CUTOFF) {
			//then decrease the y velocity to the jump cutoff
			rb2d.velocity = new Vector2(rb2d.velocity.x, JUMP_CUTOFF);
		}
	}

	public void Dash() {
		if (dashCooldown || dashing || parrying) {
			return;
		}
		InterruptAttack();
		//insert vapor/damage dash perks here
		if (vaporDash || damageDash) {
			SetInvincible(true);
            CyanSprite();
        }
        if (damageDash) {
            anim.SetTrigger("DamageDash");
        } else {
			anim.SetTrigger("Dash");
		}
		dashing = true;
		Freeze();
		preDashVelocity = new Vector2(rb2d.velocity.x, 0);
	}

	public void StopDashing() {
        UnFreeze();
        dashing = false;
        rb2d.velocity = preDashVelocity;
        StartCoroutine(StartDashCooldown(.2f));
		
        if (vaporDash) {
            WhiteSprite();
            SetInvincible(false);
        }
		CloseAllHurtboxes();
    }

	//the same as above without arresting the momentum
	void InterruptDash() {
		UnFreeze();
        dashing = false;
        StartCoroutine(StartDashCooldown(.2f));
		
        if (vaporDash) {
            WhiteSprite();
            SetInvincible(false);
        }
		CloseAllHurtboxes();
	}

	IEnumerator StartDashCooldown(float seconds) {
        dashCooldown = true;
        yield return new WaitForSeconds(seconds);
        dashCooldown = false;
    }

	void UpdateGrounded() {
		//cast two rays in the ground direction to check for intersection
		bool leftGrounded = Physics2D.Linecast(transform.position, groundCheckLeft.position, 1 << LayerMask.NameToLayer(Layers.Ground));
		bool rightGrounded = Physics2D.Linecast(transform.position, groundCheckRight.position, 1 << LayerMask.NameToLayer(Layers.Ground));

		//then check and call updates accordingly
		bool groundedLastFrame = grounded;
		grounded = (leftGrounded || rightGrounded)
			&& rb2d.velocity.y <= 0;

		if (!groundedLastFrame && grounded) {
			OnGroundHit();	
		} else if (groundedLastFrame && !grounded) {
			OnGroundLeave();
		}
	}

	bool HorizontalInput() {
		return Input.GetAxis("Horizontal") != 0;
	}

	void OnGroundHit() {
		ResetAirJumps();
		InterruptAttack();
		anim.SetBool("Grounded", true);
		if (terminalFalling) {
			CameraShaker.SmallShake();
			anim.SetTrigger("HardLand");
		}
	}

	void ResetAirJumps() {
		airJumps = maxAirJumps;
	}

	void OnGroundLeave() {
		anim.SetBool("Grounded", false);
	}

	void InterruptAttack() {
		CloseAllHurtboxes();
		ResetAttackTriggers();
		midSwing = false;
	}

	public void ResetAttackTriggers() {
		anim.ResetTrigger("Attack");
	}

	void UpdateWallSliding() {
		bool touchingLastFrame = touchingWall;
		touchingWall = wcForwards.TouchingWall();
		if (!touchingLastFrame && touchingWall) {
			OnWallHit();
		} 
		else if (touchingLastFrame && !touchingWall) {
			OnWallLeave();
		}
	}

	void OnWallHit() {
		InterruptDash();
		anim.SetBool("TouchingWall", true);
		ResetAirJumps();
	}

	void OnWallLeave() {
		anim.SetBool("TouchingWall", false);
	}

	void FreezeFor(float seconds) {
		Freeze();
		StartCoroutine(WaitAndUnFreeze(seconds));
	}

	IEnumerator WaitAndUnFreeze(float seconds) {
		yield return new WaitForSeconds(seconds);
		UnFreeze();
	}

	public void Freeze() {
		this.frozen = true;
	}

	public void UnFreeze() {
		this.frozen = false;
	}

	public void CloseAllHurtboxes() {
		foreach (Transform hurtbox in hurtboxes.GetComponentInChildren<Transform>()) {
            if (hurtbox.GetComponent<BoxCollider2D>().enabled) {
                hurtbox.GetComponent<BoxCollider2D>().enabled = false;
            } 
        }
	}

	public void CyanSprite() {
        spr.material = cyanMaterial;
    }

    public void WhiteSprite() {
        spr.material = defaultMaterial;
    }

    IEnumerator Hurt(int flashes, bool first) {
        SetInvincible(true);
        CyanSprite();
        yield return new WaitForSeconds(.07f);
        WhiteSprite();
        yield return new WaitForSeconds(.07f);
        if (flashes > 0) {
            StartCoroutine(Hurt(--flashes, first)); //;^)
        } else {
            SetInvincible(false);
        }
    }

    public void SetInvincible(bool b) {
        this.invincible = b;
    }
}
