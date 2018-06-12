using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity {

	//constants
	float maxMoveSpeed = 3f;
	float jumpSpeed = 4.5f;
	float jumpCutoff = 2.0f;
	int maxAirJumps = 1;
	float hardLandVelocity = -4.5f;
	float terminalVelocity = -10f;
	public int baseAttackDamage = 1;
	float dashSpeed = 8;
	float dashCooldownLength = .5f;
	public bool hardFalling = false;
	float ledgeBoostSpeed = 4f;
	bool damageDash = true;
	int maxHP = 7;
	public int currentHP = 1;
	int maxEnergy = 25;
	public int currentEnergy = 5;
	float invincibilityLength = .5f;

	//linked components
	Rigidbody2D rb2d;
	Animator anim;
	public WallCheck wallCheck;
	public GameObject hurtboxes;
	SpriteRenderer spr;
	Material defaultMaterial;
    Material cyanMaterial;
	Transform eyePosition;
	Gun gun;
	public ContainerUI healthUI;
	public ContainerUI energyUI;
	public ParticleSystem deathParticles;
	InteractAppendage interaction;
	PlayerWings wings;

	//variables
	bool grounded = false;
	bool touchingWall = false;
	public int airJumps;
	public bool midSwing = false;
	bool dashCooldown = false;
	public bool dashing = false;
	bool parrying = false;
	bool inMeteor = false;
	bool terminalFalling = false;
	bool cyan = false;
	bool justLeftWall = false;
	Coroutine currentWallTimeout;
	bool canShoot = true;
	Coroutine platformTimeout;
	public bool inCutscene;
	bool dead = false;
	bool supercruise = false;
	Coroutine dashTimeout;

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
		eyePosition = transform.Find("EyePosition").transform;
		gun = GetComponent<Gun>();
		currentHP = maxHP;
		currentEnergy = maxEnergy;
		interaction = GetComponentInChildren<InteractAppendage>();
		wings = transform.Find("Wings").GetComponent<PlayerWings>();
		Flip();
	}
	
	void Update () {
		CheckFlip();
		UpdateWallSliding();
		Move();
		Shoot();
		Attack();
		Jump();
		Interact();
		UpdateUI();
	}

	void Interact() {
		if (Input.GetButtonDown("Submit") && interaction.currentInteractable != null && !inCutscene) {
			interaction.currentInteractable.Interact(this.gameObject);
		}
	}

	void Attack() {
		anim.SetFloat("VerticalInput", Input.GetAxis("Vertical"));
		anim.SetBool("SpecialHeld", Input.GetButton("Special"));

		if (Input.GetButtonDown("Attack") && !frozen) {
			wings.FoldIn();
			anim.SetTrigger("Attack");
		}

		else if (!grounded && Input.GetButtonDown("Special") && Input.GetAxis("Vertical") < 0 && !dashing) {
			MeteorSlam();
		}
	}

	void Move() {
		if (Input.GetButtonDown("Jump") && supercruise) {
			EndSupercruise();
		}

		if (Input.GetButtonDown("Special") && HorizontalInput() && ((!frozen) || justLeftWall)) {
			Dash();
		}

		if (!frozen && !stunned) {
			if (Input.GetAxis("Vertical") < 0) {
				if (GetComponent<GroundCheck>().TouchingPlatform()) {
					DropThroughPlatform();
				}
			}

			anim.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
			anim.SetFloat("VerticalSpeed", rb2d.velocity.y);

			if (HorizontalInput() && !midSwing) {
				if (Input.GetAxis("Horizontal") != 0) {
					//if they just finished a dash or supercruise, keep their speed around for a bit ;^)
					if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed && 
							(Input.GetAxis("Horizontal") * GetForwardScalar() > 0)) 
					{
						rb2d.velocity = new Vector2(
							x:rb2d.velocity.x / 1.01f,
							y:rb2d.velocity.y
						);
					} else {
						rb2d.velocity = new Vector2(x:(Input.GetAxis("Horizontal") * maxMoveSpeed), y:rb2d.velocity.y);
					}
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

			//if they're above max move speed, gently slow them
			if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed) {
				rb2d.velocity = new Vector2(
					x:rb2d.velocity.x / 1.01f,
					y:rb2d.velocity.y
				);
			}
		}

		if (dashing || supercruise) {
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

		if (touchingWall && !grounded && !HorizontalInput()) {
			rb2d.velocity = new Vector2(0, rb2d.velocity.y);
		}
	}

	void Jump() {
		if (frozen || wallCheck.TouchingLedge() || lockedInSpace) {
			return;
		}

		if (Input.GetButtonDown("Jump")) {
			StopPlatformDrop();
			if (grounded) {
				rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:jumpSpeed);
				anim.SetTrigger("Jump");
				InterruptAttack();
			}
			else if (touchingWall || justLeftWall) {
				InterruptMeteor();
				InterruptAttack();
				FreezeFor(.1f);
				rb2d.velocity = new Vector2(
					//we don't want to boost the player back to the wall if they just input a direction away from it
					x:maxMoveSpeed * GetForwardScalar() * (justLeftWall ? 1 : -1), 
					y:jumpSpeed
				);
				Flip();
				anim.SetTrigger("WallJump");
				StopWallTimeout();
			}
			else if (airJumps > 0) {
				InterruptMeteor();
				rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:jumpSpeed);
				airJumps--;
				anim.SetTrigger("Jump");
				wings.Open();
				wings.EnableJets();
				wings.Jump();
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
		if (dashCooldown || dashing || parrying || dead) {
			return;
		}
		StopWallTimeout();
		InterruptAttack();
		inMeteor = false;
		SetInvincible(true);
		envDmgSusceptible = false;
        if (damageDash) {
            anim.SetTrigger("DamageDash");
        } else {
			anim.SetTrigger("Dash");
		}
		wings.Open();
		wings.EnableJets();
		wings.Dash();
		dashing = true;
		Freeze();
	}

	public void StopDashing() {
        UnFreeze();
        dashing = false;
        dashTimeout = StartCoroutine(StartDashCooldown(dashCooldownLength));
		envDmgSusceptible = true;
        SetInvincible(false);
		CloseAllHurtboxes();
		wings.FoldIn();
    }

	//the same as above without resetting the momentum
	void InterruptDash() {
		UnFreeze();
        dashing = false;
        StartCoroutine(StartDashCooldown(dashCooldownLength));
		envDmgSusceptible = true;
        SetInvincible(false);
		CloseAllHurtboxes();
	}

	IEnumerator StartDashCooldown(float seconds) {
        dashCooldown = true;
        yield return new WaitForSeconds(seconds);
        EndDashCooldown();
    }

	void EndDashCooldown() {
		dashCooldown = false;
		dashTimeout = null;
	}

	bool HorizontalInput() {
		return Input.GetAxis("Horizontal") != 0;
	}

	public override void OnGroundHit() {
		grounded = true;
		ResetAirJumps();
		InterruptAttack();
		StopWallTimeout();
		EndDashCooldown();
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
		StopPlatformDrop();
		grounded = false;
		anim.SetBool("Grounded", false);
	}

	void InterruptAttack() {
		CloseAllHurtboxes();
		ResetAttackTriggers();
		midSwing = false;
	}

	void InterruptMeteor() {
		anim.SetBool("InMeteor", false);
		inMeteor = false;
		wings.FoldIn();
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
		CloseWings();
		InterruptDash();
		EndSupercruise();
		anim.SetBool("TouchingWall", true);
		ResetAirJumps();
	}

	void OnWallLeave() {
		anim.SetBool("TouchingWall", false);

		//if the player just left the wall, they input the opposite direction for a walljump
		//so give them a split second to use a walljump when they're not technically touching the wall
		if (!grounded) {
			currentWallTimeout = StartCoroutine(WallLeaveTimeout());
		}
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
		if (inMeteor || dead) return;
		inMeteor = true;
		SetInvincible(true);
		anim.SetTrigger("Meteor");
		anim.SetBool("InMeteor", true);
		wings.Open();
		wings.EnableJets();
		wings.Meteor();
		rb2d.velocity = new Vector2(
			x:0,
			y:terminalVelocity
		);
	}

	void LandMeteor() {
		wings.FoldIn();
		inMeteor = false;
		anim.SetBool("InMeteor", false);
		rb2d.velocity = Vector2.zero;
		SetInvincible(false);
		//if called while wallsliding
		anim.ResetTrigger("Meteor");
		CameraShaker.MedShake();
		if (currentEnergy > 0) {
			Instantiate(vaporExplosion, transform.position, Quaternion.identity);
		}
	}

	public void Sparkle() {
		if (instantiatedSparkle == null) {
			instantiatedSparkle = (GameObject) Instantiate(sparkle, eyePosition.position, Quaternion.identity, eyePosition.transform).gameObject as GameObject;
		}
	}

	public void Shoot() {
		if (Input.GetButtonDown("Projectile") && canShoot && CheckEnergy() >= 1) {
			Sparkle();
			gun.Fire(
				forwardScalar: GetForwardScalar(), 
				bulletPos: eyePosition
			);
			LoseEnergy(1);
		}
	}

	public void GainEnergy(int amount) {
		currentEnergy += amount;
		if (currentEnergy > maxEnergy) {
			currentEnergy = maxEnergy;
		}
	}

	public int CheckEnergy() {
		return currentEnergy;
	}

	public void LoseEnergy(int amount) {
		currentEnergy -= amount;
		if (currentEnergy < 0) {
			currentEnergy = 0;
		}
	}

	void LedgeBoost() {
		if (inMeteor || Input.GetAxis("Vertical") < 0 || supercruise) {
			return;
		}
		bool movingTowardsLedge = (Input.GetAxis("Horizontal") * GetForwardScalar()) > 0;
		if (movingTowardsLedge) {
			EndDashCooldown();
			wings.Open();
			wings.EnableJets();
			wings.LedgeBoost();
			InterruptDash();
			//provide an upward impulse
			ResetAirJumps();
			InterruptAttack();
			rb2d.velocity = new Vector2(
				x:maxMoveSpeed * GetForwardScalar(),
				y:ledgeBoostSpeed
			);
		}
	}

	public override void OnHit(Attack attack) {
		if (invincible && !attack.attackerParent.CompareTag(Tags.EnviroDamage)) {
			return;
		}
		if (attack.attackerParent.CompareTag(Tags.EnviroDamage)) {
			InterruptMeteor();
		}
		CameraShaker.MedShake();
		InterruptSupercruise();
		DamageFor(attack.GetDamage());
		if (this.currentHP == 0) {
			return;
		}
		InvincibleFor(this.invincibilityLength);
		CyanSprite();
		//compute potential stun
		StunFor(attack.GetStunLength());
		//compute potential knockback
		//unfreeze if this enemy is in hitstop to preserve the first knockback vector
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

	IEnumerator WaitAndSetVincible(float seconds) {
		yield return new WaitForSeconds(seconds);
		SetInvincible(false);
	}

	void InvincibleFor(float seconds) {
		SetInvincible(true);
		StartCoroutine(WaitAndSetVincible(seconds));
	}


	void DamageFor(int dmg) {
		currentHP -= dmg;
		if (currentHP <= 0) {
			Die();
		}
	}

	void Die() {
		this.dead = true;
		CameraShaker.BigShake();
		deathParticles.Emit(50);
		LockInSpace();
		Freeze();
		anim.SetTrigger("Die");
		DisableShooting();
		InterruptEverything();
		ResetAttackTriggers();
		ResetAirJumps();
	}

	public void FinishDyingAnimation() {
		GlobalController.Respawn();
	}

	public void StartRespawning() {
		anim.SetTrigger("Respawn");
	}

	public void StartRespawnAnimation() {
		
	}

	public void EndRespawnAnimation() {
		UnFreeze();
		UnLockInSpace();
		EnableShooting();
		InvincibleFor(1f);
		FullHeal();
		this.dead = false;
	}

	void FullHeal() {
		currentHP = maxHP;
		currentEnergy = maxEnergy;
	}

	void UpdateUI() {
		healthUI.SetMax(maxHP);
		healthUI.SetCurrent(currentHP);
		energyUI.SetMax(maxEnergy);
		energyUI.SetCurrent(currentEnergy);
	}

	IEnumerator WallLeaveTimeout() {
		justLeftWall = true;
		anim.SetBool("JustLeftWall", true);
		yield return new WaitForSeconds(.1f);
		justLeftWall = false;
		anim.SetBool("JustLeftWall", false);
	}

	void StopWallTimeout() {
		if (currentWallTimeout != null) {
			StopCoroutine(currentWallTimeout);
		}
		anim.SetBool("JustLeftWall", false);
		justLeftWall = false;
	}

	void EnableShooting() {
		this.canShoot = true;
	}

	void DisableShooting() {
		this.canShoot = false;
	}

	void DropThroughPlatform() {
		UnFreeze();
		rb2d.velocity = new Vector2(
			rb2d.velocity.x,
			hardLandVelocity
		);
		GetComponent<BoxCollider2D>().enabled = false;
		platformTimeout = StartCoroutine(EnableCollider(0.5f));
	}

	IEnumerator EnableCollider(float seconds) {
		yield return new WaitForSeconds(seconds);
		StopPlatformDrop();
	}

	void StopPlatformDrop() {
		if (platformTimeout != null) {
			StopCoroutine(platformTimeout);
		}
		GetComponent<BoxCollider2D>().enabled = true;
	}

	void InterruptEverything() {
		ResetAttackTriggers();
		InterruptAttack();
		InterruptDash();
		InterruptMeteor();
		InterruptSupercruise();
	}

	public void EnterDialogue() {
		InterruptEverything();
		SetInvincible(true);
		Freeze();
		LockInSpace();
		DisableShooting();
		inCutscene = true;
	}

	public void ExitDialogue() {
		UnFreeze();
		UnLockInSpace();
		EnableShooting();
		SetInvincible(false);
		inCutscene = false;
	}

	//called from animator
	public void CloseWings() {
		wings.FoldIn();
	}

	public bool IsGrounded() {
		return this.grounded;
	}

	public void OpenSupercruiseWings() {
		wings.Open();
		wings.EnableJets();
		wings.Supercruise();
	}

	//called at the start of the supercruiseMid animation
	public void StartSupercruise() {
		this.supercruise = true;
		//move slightly up to keep them off the ground
		this.transform.Translate(Vector2.up * 0.05f);
		wings.Open();
		wings.EnableJets();
		wings.SupercruiseMid();
		Freeze();
		CameraShaker.MedShake();
		//keep them level
		rb2d.constraints = rb2d.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
	}

	public void EndSupercruise() {
		if (!supercruise) return;
		supercruise = false;
		UnFreeze();
		wings.FoldIn();
		anim.SetTrigger("EndSupercruise");
		rb2d.constraints = rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
	}

	//when the player hits a wall or dies 
	public void InterruptSupercruise() {
		if (!supercruise) return;
		CameraShaker.SmallShake();
		supercruise = false;
		UnFreeze();
		wings.FoldIn();
		rb2d.constraints = rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
		anim.SetTrigger("InterruptSupercruise");
	}
}
