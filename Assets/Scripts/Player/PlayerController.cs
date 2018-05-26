using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity {

	//constants
	float MaxMoveSpeed = 2.5f;
	float JumpSpeed = 4.5f;
	float jumpCutoff = 2.0f;
	int maxAirJumps = 1;
	float hardLandVelocity = -5f;
	float terminalVelocity = -10f;
	public int baseAttackDamage = 1;
	float dashSpeed = 8;
	float dashCooldownLength = .5f;
	public bool hardFalling = false;
	int flashTimes = 5;
	bool vaporDash = false;
	bool damageDash = true;

	//linked components
	Rigidbody2D rb2d;
	Animator anim;
	//public Transform groundCheckLeft;
	//public Transform groundCheckRight;
	public WallCheck wallCheck;
	public GameObject hurtboxes;
	SpriteRenderer spr;
	Material defaultMaterial;
    Material cyanMaterial;
	Transform effectPoint;
	Gun gun;
	GroundCheck groundCheck;

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
	bool inMeteor = false;

	//other misc prefabs
	public Transform vaporExplosion;
	public Transform sparkle;
	GameObject instantiatedSparkle = null;

	void Start () {
		airJumps = maxAirJumps;
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		this.facingRight = false;
		spr = this.GetComponent<SpriteRenderer>();
        defaultMaterial = spr.material;
        cyanMaterial = Resources.Load<Material>("Shaders/CyanFlash");
		effectPoint = transform.Find("EffectPoint").transform;
		gun = GetComponent<Gun>();
		groundCheck = GetComponent<GroundCheck>();
	}
	
	void Update () {
		CheckFlip();
		UpdateGrounded();
		UpdateWallSliding();
		Move();
		Shoot();
		Attack();
		Jump();
	}

	void Attack() {
		anim.SetFloat("VerticalInput", Input.GetAxis("Vertical"));

		if (Input.GetButtonDown("Attack")) {
			anim.SetTrigger("Attack");
		}

		else if (!grounded && Input.GetButtonDown("Special") && Input.GetAxis("Vertical") < 0 && !dashing) {
			MeteorSlam();
		}
	}

	void Move() {

		if (!frozen) {
			anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
			anim.SetFloat("VerticalSpeed", rb2d.velocity.y);

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
					x:rb2d.velocity.x / 1.01f,
					y:rb2d.velocity.y
				);
			}

			if (Input.GetButtonDown("Special") && HorizontalInput() && !touchingWall) {
				Dash();
			}
		}

		if (dashing) {
            rb2d.velocity = new Vector2(dashSpeed * GetForwardScalar(), 0);
        }

		if (rb2d.velocity.y < terminalVelocity) {
			rb2d.velocity = new Vector2(rb2d.velocity.x, terminalVelocity);
		} 

		if (rb2d.velocity.y < hardLandVelocity) {
			hardFalling = true;
		}
		else {
			hardFalling = false;
		}
	}

	void Jump() {
		if (Input.GetButtonDown("Jump") && !frozen) {
			if (grounded) {
				rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:JumpSpeed);
				anim.SetTrigger("Jump");
				InterruptAttack();
			}
			else if (touchingWall) {
				InterruptMeteor();
				FreezeFor(.1f);
				rb2d.velocity = new Vector2(x:-2 * GetForwardScalar(), y:JumpSpeed);
				Flip();
				anim.SetTrigger("Jump");
				InterruptAttack();
			}
			else if (airJumps > 0) {
				InterruptMeteor();
				rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:JumpSpeed);
				airJumps--;
				anim.SetTrigger("Jump");
				InterruptAttack();
			}
		}

		//emulate an analog jump
		//if the jump button is released
		if (Input.GetButtonUp("Jump") && rb2d.velocity.y > jumpCutoff) {
			//then decrease the y velocity to the jump cutoff
			rb2d.velocity = new Vector2(rb2d.velocity.x, jumpCutoff);
		}
	}

	public void Dash() {
		if (dashCooldown || dashing || parrying) {
			return;
		}
		InterruptAttack();
		inMeteor = false;
		//insert vapor/damage dash perks here
		if (vaporDash) {
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
        StartCoroutine(StartDashCooldown(dashCooldownLength));
		
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
        StartCoroutine(StartDashCooldown(dashCooldownLength));
		
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
		/* 
		bool leftGrounded = Physics2D.Linecast(transform.position, groundCheckLeft.position, 1 << LayerMask.NameToLayer(Layers.Ground));
		bool rightGrounded = Physics2D.Linecast(transform.position, groundCheckRight.position, 1 << LayerMask.NameToLayer(Layers.Ground));

		//then check and call updates accordingly
		bool groundedLastFrame = grounded;
		grounded = (leftGrounded || rightGrounded)
			&& rb2d.velocity.y <= 0;
		*/
		bool groundedLastFrame = grounded;
		grounded = groundCheck.IsGrounded();
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
		if (inMeteor) {
			LandMeteor();
		}
		anim.SetBool("Grounded", true);
		if (hardFalling) {
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

	void InterruptMeteor() {
		inMeteor = false;
	}

	public void ResetAttackTriggers() {
		anim.ResetTrigger("Attack");
	}

	void UpdateWallSliding() {
		bool touchingLastFrame = touchingWall;
		touchingWall = wallCheck.TouchingWall();
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
		this.inMeteor = false;
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

	void MeteorSlam() {
		inMeteor = true;
		SetInvincible(true);
		anim.SetTrigger("Meteor");

		rb2d.velocity = new Vector2(
			x:0,
			y:terminalVelocity
		);
	}

	void LandMeteor() {
		inMeteor = false;
		//if called while wallsliding
		anim.ResetTrigger("Meteor");
		Instantiate(vaporExplosion, transform.position, Quaternion.identity);
	}

	public void Sparkle() {
		if (instantiatedSparkle == null) {
			instantiatedSparkle = (GameObject) Instantiate(sparkle, effectPoint.position, Quaternion.identity, effectPoint.transform).gameObject as GameObject;
		}
	}

	public void Shoot() {
		if (Input.GetButtonDown("Projectile")) {
			Sparkle();
			gun.Fire(
				forwardScalar: GetForwardScalar(), 
				bulletPos: effectPoint
			);
		}
	}
}
