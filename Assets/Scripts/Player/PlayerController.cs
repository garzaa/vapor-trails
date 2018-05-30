using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity {

	//constants
	float MaxMoveSpeed = 2.5f;
	float jumpSpeed = 4.5f;
	float jumpCutoff = 2.0f;
	int maxAirJumps = 1;
	float hardLandVelocity = -5f;
	float terminalVelocity = -10f;
	public int baseAttackDamage = 1;
	float dashSpeed = 8;
	float dashCooldownLength = .5f;
	public bool hardFalling = false;
	float ledgeBoostSpeed = 4f;
	bool damageDash = false;
	int maxHP = 5;
	int currentHP = 5;
	float invincibilityLength = .8f;

	//linked components
	Rigidbody2D rb2d;
	Animator anim;
	public WallCheck wallCheck;
	public GameObject hurtboxes;
	SpriteRenderer spr;
	Material defaultMaterial;
    Material cyanMaterial;
	Transform effectPoint;
	Gun gun;
	public ContainerUI healthUI;
public ContainerUI energyUI;

	//variables
	bool grounded = false;
	bool touchingWall = false;
	public int airJumps;
	public bool midSwing = false;
	bool dashCooldown = false;
	public bool dashing = false;
	bool parrying = false;
	Vector2 preDashVelocity;
	bool inMeteor = false;
	bool terminalFalling = false;
	bool cyan = false;

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
		currentHP = maxHP;
	}
	
	void Update () {
		CheckFlip();
		UpdateWallSliding();
		Move();
		Shoot();
		Attack();
		Jump();
		UpdateUI();
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

		if (!frozen && !stunned) {
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
			terminalFalling = true;
			rb2d.velocity = new Vector2(rb2d.velocity.x, terminalVelocity);
		} else {
			terminalFalling = false;
		}

		if (rb2d.velocity.y < hardLandVelocity) {
			hardFalling = true;
		}
		else {
			hardFalling = false;
		}

		if (wallCheck.TouchingLedge()) {
			LedgeBoost();
		}
	}

	void Jump() {
		if (Input.GetButtonDown("Jump") && !frozen && !wallCheck.TouchingLedge()) {
			if (grounded) {
				rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:jumpSpeed);
				anim.SetTrigger("Jump");
				InterruptAttack();
			}
			else if (touchingWall) {
				InterruptMeteor();
				FreezeFor(.1f);
				rb2d.velocity = new Vector2(x:-2 * GetForwardScalar(), y:jumpSpeed);
				Flip();
				anim.SetTrigger("Jump");
				InterruptAttack();
			}
			else if (airJumps > 0) {
				InterruptMeteor();
				rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:jumpSpeed);
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
		SetInvincible(true);
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
		
        SetInvincible(false);
		CloseAllHurtboxes();
    }

	//the same as above without arresting the momentum
	void InterruptDash() {
		UnFreeze();
        dashing = false;
        StartCoroutine(StartDashCooldown(dashCooldownLength));
		
        SetInvincible(false);
		CloseAllHurtboxes();
	}

	IEnumerator StartDashCooldown(float seconds) {
        dashCooldown = true;
        yield return new WaitForSeconds(seconds);
        dashCooldown = false;
    }

	bool HorizontalInput() {
		return Input.GetAxis("Horizontal") != 0;
	}

	public override void OnGroundHit() {
		grounded = true;
		ResetAirJumps();
		InterruptAttack();
		if (inMeteor) {
			LandMeteor();
		}
		anim.SetBool("Grounded", true);
		if (hardFalling) {
			anim.SetTrigger("HardLand");
		}
		if (terminalFalling) {
			CameraShaker.SmallShake();
		}
	}

	void ResetAirJumps() {
		airJumps = maxAirJumps;
	}

	public override void OnGroundLeave() {
		grounded = false;
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
		cyan = true;
        spr.material = cyanMaterial;
    }

    public void WhiteSprite() {
        spr.material = defaultMaterial;
    }

    public void SetInvincible(bool b) {
        this.invincible = b;
    }

	void MeteorSlam() {
		if (inMeteor) return;
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
		SetInvincible(false);
		//if called while wallsliding
		anim.ResetTrigger("Meteor");
		CameraShaker.MedShake();
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

	public void GainEnergy(int amount) {
		//UIController.UpdateUI()
	}

	public int CheckEnergy() {
		return 1;
	}

	public void LoseEnergy(int amount) {

	}

	void LedgeBoost() {
		if (dashing || inMeteor || Input.GetAxis("Vertical") < 0) {
			return;
		}
		bool movingTowardsLedge = (Input.GetAxis("Horizontal") * GetForwardScalar()) > 0;
		if (Input.GetButtonDown("Jump") || movingTowardsLedge) {
			//anim.SetTrigger("LedgeBoost");
			//provide an upward impulse
			ResetAirJumps();
			InterruptAttack();
			rb2d.velocity = new Vector2(
				x:rb2d.velocity.x * 1.2f * GetForwardScalar(),
				y:ledgeBoostSpeed
			);
		}
	}

	public override void OnHit(Attack attack) {
		if (invincible) {
			return;
		}
		InvincibleFor(this.invincibilityLength);
		CameraShaker.MedShake();
		CyanSprite();
		DamageFor(attack.GetDamage());
		//compute potential stun
		StunFor(attack.GetStunLength());
		//compute potential knockback
		//unfreeze if this enemy is in hitstop to preserve the knockback vector
		//they'll be put back in hitstop afterwards by the incoming attack if necessary
		if (inHitstop) {
			UnLockInSpace();
			inHitstop = false;
		}
		if (attack.knockBack) {
			//knockback based on the position of the attack
			Vector2 kv = attack.GetKnockback();
			bool attackerToLeft = attack.transform.position.x < this.transform.position.x;
			kv.x *= attackerToLeft ? 1 : -1;
			KnockBack(kv);
		}
		if (cyan) {
			cyan = false;
			StartCoroutine(normalSprite());
		}
	}

	IEnumerator normalSprite() {
		yield return new WaitForSeconds(.1f);
		spr.material = defaultMaterial;
	}

	IEnumerator waitAndSetVincible(float seconds) {
		yield return new WaitForSeconds(seconds);
		SetInvincible(false);
	}

	void InvincibleFor(float seconds) {
		SetInvincible(true);
		StartCoroutine(waitAndSetVincible(seconds));
	}


	void DamageFor(int dmg) {
		currentHP -= dmg;
		if (this.currentHP <= 0) {
			Die();
		}
	}

	void Die() {
		Destroy(this.gameObject);
	}

	void UpdateUI() {
		healthUI.SetMax(maxHP);
		healthUI.SetCurrent(currentHP);
	}
}
