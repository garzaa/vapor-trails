using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity {

	//unlocks
	PlayerUnlocks abilities;

	//constants
	float maxMoveSpeed = 3f;
	float jumpSpeed = 4.5f;
	float jumpCutoff = 2.0f;
	float hardLandVelocity = -4.5f;
	float terminalVelocity = -10f;
	float dashSpeed = 8;
	float dashCooldownLength = .5f;
	public bool hardFalling = false;
	float ledgeBoostSpeed = 4f;
	public int currentHP = 1;
	public int currentEnergy = 5;
	float invincibilityLength = .5f;
	int healCost = 1;
	int healAmt = 1;
	float backstepCooldownLength = .2f;
	public bool riposteTriggered = false;

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
	public PlayerWings wings;
	PlayerUnlocks unlocks;

	//variables
	bool grounded = false;
	bool touchingWall = false;
	int airJumps;
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
	public bool supercruise = false;
	Coroutine dashTimeout;
	bool pressedUpLastFrame = false;
	bool flashingCyan = false;
	bool cyanLastFrame = false;
	bool runningLastFrame = false;
	bool inBackstep = false;
	bool backstepCooldown = false;
	bool forcedWalking = false;

	//other misc prefabs
	public Transform vaporExplosion;
	public Transform sparkle;
	public Transform dust;
	GameObject instantiatedSparkle = null;

	void Start() {
		unlocks = GetComponentInParent<PlayerUnlocks>();
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		this.facingRight = false;
		currentHP = unlocks.maxHP;
		currentEnergy = unlocks.maxEnergy;
        cyanMaterial = Resources.Load<Material>("Shaders/CyanFlash");
		spr = GetComponent<SpriteRenderer>();
        defaultMaterial = GetComponent<SpriteRenderer>().material;
		eyePosition = transform.Find("EyePosition").transform;
		gun = GetComponent<Gun>();
		interaction = GetComponentInChildren<InteractAppendage>();
		wings = transform.Find("Wings").GetComponent<PlayerWings>();
		anim.SetBool("CanSupercruise", unlocks.supercruise);
		Flip();
		ResetAirJumps();
	}
	
	void Update () {
		CheckHeal();
		CheckFlash();
		UpdateWallSliding();
		Move();
		Shoot();
		Attack();
		Jump();
		Interact();
		UpdateUI();
		CheckFlip();
	}

	void Interact() {
		if (UpButtonPress() && interaction.currentInteractable != null && !inCutscene) {
			SoundManager.InteractSound();
			interaction.currentInteractable.Interact(this.gameObject);
		}
	}

	bool IsForcedWalking() {
		return this.forcedWalking || Input.GetKey(KeyCode.LeftControl);
	}

	void CheckFlash() {
		if (flashingCyan) {
			if (cyanLastFrame) {
				cyanLastFrame = false;
				WhiteSprite();
			} else {
				cyanLastFrame = true;
				CyanSprite();
			}
		}
	}

	void Attack() {
		if (inCutscene) {
			return;
		}

		anim.SetFloat("VerticalInput", Input.GetAxis("Vertical"));
		
		if (!inCutscene) {
			anim.SetBool("SpecialHeld", Input.GetButton("Special"));
		} else {
			anim.SetBool("SpecialHeld", false);
		}

		if (Input.GetButtonDown("Attack") && !frozen && !inMeteor) {
			wings.FoldIn();
			anim.SetTrigger("Attack");
		}

		else if (!grounded && Input.GetButtonDown("Special") && Input.GetAxis("Vertical") < 0 && !dashing) {
			if (unlocks.meteor) MeteorSlam();
		}
	}

	void Move() {
		if (inCutscene) {
			anim.SetFloat("Speed", 0f);
			if (grounded) rb2d.velocity = Vector2.zero;
			anim.SetFloat("VerticalInput", 0f);
			return;
		}

		if (Input.GetButtonDown("Jump") && supercruise) {
			EndSupercruise();
		}

		if (Input.GetButtonDown("Special") && HorizontalInput() && (!frozen || justLeftWall) && Input.GetAxis("Vertical") >= -0.1) {
			if (unlocks.dash) Dash();
		}

		if (frozen) {
			anim.SetFloat("Speed", 0f);
			if (grounded) rb2d.velocity = Vector2.zero;
		}

		if (!frozen && !(stunned || dead)) {
			if (Input.GetAxis("Vertical") < 0 && grounded && !backstepCooldown && Input.GetButtonDown("Attack")) {
				Backstep();
			}
			if (Input.GetAxis("Vertical") < 0 && Input.GetButtonDown("Jump")) {
				if (GetComponent<GroundCheck>().TouchingPlatform() && grounded) {
					DropThroughPlatform();
				}
			}

			float modifier = IsForcedWalking() ? 0.45f : 1f;
			float hInput = Input.GetAxis("Horizontal") * modifier;
			if (!touchingWall) {
				anim.SetFloat("Speed", Mathf.Abs(hInput));
			} else {
				anim.SetFloat("Speed", 0);
			}
			anim.SetFloat("VerticalSpeed", rb2d.velocity.y);

			if (HorizontalInput() && !midSwing) {
				if (Input.GetAxis("Horizontal") != 0) {
					//if they just finished a dash or supercruise, keep their speed around for a bit ;^)
					if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed && 
							(Input.GetAxis("Horizontal") * GetForwardScalar() > 0)) 
					{
						//slow the player down less in the air
						float divisor = 1.01f;
						if (!grounded) {
							divisor = 1.005f;
						}
						rb2d.velocity = new Vector2(
							x:rb2d.velocity.x / divisor,
							y:rb2d.velocity.y
						);
					} else {
						rb2d.velocity = new Vector2(x:(hInput * maxMoveSpeed), y:rb2d.velocity.y);
					}
					movingRight = Input.GetAxis("Horizontal") > 0;
				}
				if (!runningLastFrame && rb2d.velocity.x != 0 && grounded && Mathf.Abs(hInput) > 0.6f && !touchingWall) {
					int scalar = rb2d.velocity.x > 0 ? 1 : -1;
					if (scalar * GetForwardScalar() > 0) {
						BackwardDust();
					} else {
						ForwardDust();
					}
				}
			} 
			//if no movement, stop the player on the ground 
			else if (grounded) {
				rb2d.velocity = new Vector2(x:0, y:rb2d.velocity.y);
				if (runningLastFrame && !touchingWall && !midSwing) {
					ForwardDust();
				}
			} 
			//or slow them down in the air if they haven't just walljumped
			else {
				rb2d.velocity = new Vector2(
					x:rb2d.velocity.x / 1.01f,
					y:rb2d.velocity.y
				);
			}

			//if they're above max move speed, gently slow them
			if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed && !IsForcedWalking()) {
				rb2d.velocity = new Vector2(
					x:rb2d.velocity.x / 1.01f,
					y:rb2d.velocity.y
				);
			}

			runningLastFrame = Mathf.Abs(hInput) > 0.6f;
		}

		if (dashing || supercruise) {
            rb2d.velocity = new Vector2(dashSpeed * GetForwardScalar(), 0);
        }

		else if (inBackstep) {
			rb2d.velocity = new Vector2(-maxMoveSpeed * GetForwardScalar(), 0);
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

		if (wallCheck.TouchingLedge() && !grounded) {
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
			if ((Input.GetAxis("Vertical") >= 0)) StopPlatformDrop();
			if (grounded && (Input.GetAxis("Vertical") >= 0)) {
				if (HorizontalInput()) {
					BackwardDust();
				} else {
					ImpactDust();
				}
				rb2d.velocity = new Vector2(x:rb2d.velocity.x, y:jumpSpeed);
				anim.SetTrigger("Jump");
				InterruptAttack();
				SoundManager.SmallJumpSound();
			}
			else if (unlocks.wallClimb && (touchingWall || justLeftWall)) {
				SoundManager.SmallJumpSound();
				InterruptDash();
				InterruptMeteor();
				if (touchingWall) DownDust();
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
			else if (airJumps > 0 && GetComponent<BoxCollider2D>().enabled && !grounded) {
				SoundManager.JumpSound();
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
		if (Input.GetButtonUp("Jump") && rb2d.velocity.y > jumpCutoff) {
			//if the jump button is released
			//then decrease the y velocity to the jump cutoff
			rb2d.velocity = new Vector2(rb2d.velocity.x, jumpCutoff);
		}
	}

	public void Dash() {
		if (dashCooldown || dashing || parrying || dead || touchingWall) {
			return;
		}
		SoundManager.DashSound();
		StopWallTimeout();
		InterruptAttack();
		inMeteor = false;
		SetInvincible(true);
		FlashCyan();
		envDmgSusceptible = false;
        if (unlocks.damageDash) {
            anim.SetTrigger("DamageDash");
        } else {
			anim.SetTrigger("Dash");
		}
		wings.Open();
		wings.EnableJets();
		wings.Dash();
		dashing = true;
		Freeze();
		if (grounded) {
			BackwardDust();
		}
	}

	public void StopDashing() {
        UnFreeze();
        dashing = false;
        dashTimeout = StartCoroutine(StartDashCooldown(dashCooldownLength));
		envDmgSusceptible = true;
        SetInvincible(false);
		StopFlashingCyan();
		CloseAllHurtboxes();
		if (wings != null) wings.FoldIn();
    }

	void InterruptDash() {
		UnFreeze();
        dashing = false;
        StartCoroutine(StartDashCooldown(dashCooldownLength));
		envDmgSusceptible = true;
        SetInvincible(false);
		StopFlashingCyan();
		CloseAllHurtboxes();
	}

	IEnumerator StartDashCooldown(float seconds) {
        dashCooldown = true;
        yield return new WaitForSeconds(seconds);
        EndDashCooldown();
    }

	public void CheckRiposteTrigger() {
		if (this.riposteTriggered && unlocks.riposte) {
			this.riposteTriggered = false;
			anim.SetTrigger("Riposte");
			rb2d.velocity = Vector2.zero;
			StopFlashingCyan();
			UnFreeze();
			Invoke("EnableBackstep", backstepCooldownLength);
			inBackstep = false;
		}
	}

	void EndDashCooldown() {
		if (dashTimeout != null) {
			StopCoroutine(dashTimeout);
		}
		dashCooldown = false;
	}

	bool HorizontalInput() {
		return Input.GetAxis("Horizontal") != 0;
	}

	public override void OnGroundHit() {
		grounded = true;
		ResetAirJumps();
		InterruptAttack();
		EndDashCooldown();
		StopWallTimeout();
		if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed && Input.GetAxis("Horizontal") * GetForwardScalar() > 0) {
			BackwardDust();
		} else if (Mathf.Abs(rb2d.velocity.x) > maxMoveSpeed/2 && Input.GetAxis("Horizontal") * GetForwardScalar() <= 0) {
			ForwardDust();
		}
		if (inMeteor) {
			LandMeteor();
		}
		anim.SetBool("Grounded", true);
		if (hardFalling) {
			SoundManager.HardLandSound();
			if (HorizontalInput()) {
				BackwardDust();
			} else {
				ImpactDust();
			}
			CameraShaker.Shake(0.05f, 0.1f);
			anim.SetTrigger("HardLand");
		}
		if (terminalFalling) {
			CameraShaker.Shake(0.1f, 0.1f);
		}
	}

	void ResetAirJumps() {
		airJumps = unlocks.doubleJump ? 1 : 0;
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
		touchingWall = wallCheck.TouchingWall() && !dead;
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
		EndDashCooldown();
		EndSupercruise();
		if (unlocks.wallClimb) {
			anim.SetBool("TouchingWall", true);
			if (!grounded) SoundManager.HardLandSound();
		}
		ResetAirJumps();
	}

	void OnWallLeave() {
		if (unlocks.wallClimb) anim.SetBool("TouchingWall", false);

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
		if (this.defaultMaterial != null) spr.material = defaultMaterial;
    }

    public void SetInvincible(bool b) {
        this.invincible = b;
    }

	public void FlashCyan() {
		this.flashingCyan = true;
	}

	public void StopFlashingCyan() {
		WhiteSprite();
		this.flashingCyan = false;
	}

	void MeteorSlam() {
		if (inMeteor || dead) return;
		InterruptBackstep();
		inMeteor = true;
		SetInvincible(true);
		anim.SetTrigger("Meteor");
		anim.SetBool("InMeteor", true);
		wings.Open();
		wings.EnableJets();
		wings.Meteor();
		SoundManager.DashSound();
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
		SoundManager.ExplosionSound();
		CameraShaker.Shake(0.2f, 0.2f);
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
		if (!unlocks.gunEye) {
			return;
		}
		if (Input.GetButtonDown("Projectile") && canShoot && CheckEnergy() >= 1) {
			Sparkle();
			SoundManager.ShootSound();
			if (grounded) {
				BackwardDust();
			}
			gun.Fire(
				forwardScalar: GetForwardScalar(), 
				bulletPos: eyePosition
			);
			LoseEnergy(1);
		}
	}

	public void GainEnergy(int amount) {
		currentEnergy += amount;
		if (currentEnergy > unlocks.maxEnergy) {
			currentEnergy = unlocks.maxEnergy;
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
			wings.Open();
			wings.EnableJets();
			wings.LedgeBoost();
			InterruptDash();
			EndDashCooldown();
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
		if (dead) {
			return;
		}

		if (invincible && !attack.attackerParent.CompareTag(Tags.EnviroDamage)) {
			return;
		}

		if (attack.attackerParent.CompareTag(Tags.EnviroDamage)) {
			if (envDmgSusceptible) {
				OnEnviroDamage();
				InterruptMeteor();
			} else {
				return;
			}
		}

		CameraShaker.Shake(0.2f, 0.1f);
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
		SoundManager.PlayerHurtSound();
		deathParticles.Emit(50);
		currentHP -= dmg;
		if (currentHP <= 0) {
			Die();
		}
	}

	void Die() {
		this.dead = true;
		SoundManager.PlayerDieSound();
		this.envDmgSusceptible = false;
		currentEnergy = 0;
		CameraShaker.Shake(0.2f, 0.1f);
		deathParticles.Emit(50);
		LockInSpace();
		Freeze();
		anim.SetTrigger("Die");
		anim.SetBool("touchingWall", false);
		DisableShooting();
		InterruptEverything();
		ResetAttackTriggers();
		ResetAirJumps();
	}

	public void FinishDyingAnimation() {
		GlobalController.Respawn();
	}

	public void StartRespawning() {
		this.envDmgSusceptible = true;
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
		currentHP = unlocks.maxHP;
		currentEnergy = unlocks.maxEnergy;
	}

	void UpdateUI() {
		healthUI.SetMax(unlocks.maxHP);
		healthUI.SetCurrent(currentHP);
		energyUI.SetMax(unlocks.maxEnergy);
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

	public void EnableShooting() {
		this.canShoot = true;
	}

	public void DisableShooting() {
		this.canShoot = false;
	}

	void DropThroughPlatform() {
		UnFreeze();
		rb2d.velocity = new Vector2(
			rb2d.velocity.x,
			hardLandVelocity
		);
		wings.FoldIn();
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
		EndBackstep();
	}

	public void EnterDialogue() {
		InterruptEverything();
		Freeze();
		LockInSpace();
		DisableShooting();
		inCutscene = true;
		SetInvincible(true);
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
		BackwardDust();
		wings.EnableJets();
		wings.Supercruise();
	}

	//called at the start of the supercruiseMid animation
	public void StartSupercruise() {
		this.supercruise = true;
		BackwardDust();
		//move slightly up to keep them off the ground
		this.transform.Translate(Vector2.up * 0.05f);
		wings.Open();
		wings.EnableJets();
		wings.SupercruiseMid();
		Freeze();
		CameraShaker.Shake(0.1f, 0.1f);
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
		CameraShaker.Shake(0.1f, 0.1f);
		supercruise = false;
		UnFreeze();
		wings.FoldIn();
		rb2d.constraints = rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
		anim.SetTrigger("InterruptSupercruise");
	}

	public void Heal() {
		if (healCost > currentEnergy) {
			return;
		}
		
		if (currentHP < unlocks.maxHP) {
			if (grounded) {
				ImpactDust();
			}
			SoundManager.HealSound();
			currentHP += healAmt;
			currentEnergy -= healCost;
		}
	}

	public void CheckHeal() {
		if (healCost > currentEnergy || currentHP == unlocks.maxHP) {
			anim.SetBool("CanHeal", false);
		} else {
			anim.SetBool("CanHeal", true);
		}
	}

	public float MoveSpeedRatio() {
		return Mathf.Abs(rb2d.velocity.x / maxMoveSpeed);
	}

	bool VerticalInput() {
		return (Input.GetAxis("Vertical") != 0);
	}

	bool UpButtonPress() {
		bool upThisFrame = Input.GetAxis("Vertical") > 0;
		bool b = !pressedUpLastFrame && upThisFrame;
		pressedUpLastFrame = upThisFrame;
		return b;
	}

	void ImpactDust() {
		ForwardDust();
		BackwardDust();
	}

	public void ForwardDust() {
		Debug.Log("ForwardDust()");
		GameObject d = Instantiate(dust, new Vector3(
			this.transform.position.x + 0.32f * GetForwardScalar(),
			this.transform.position.y - GetComponent<BoxCollider2D>().bounds.extents.y + .12f,
			this.transform.position.z
		), Quaternion.identity).gameObject;
		d.transform.localScale = new Vector3(-this.transform.localScale.x, 1, 1);
	}

	public void BackwardDust() {
		GameObject d = Instantiate(dust, new Vector3(
			this.transform.position.x - 0.32f * GetForwardScalar(),
			this.transform.position.y - GetComponent<BoxCollider2D>().bounds.extents.y + .12f,
			this.transform.position.z
		), Quaternion.identity).gameObject;
		d.transform.localScale = new Vector3(this.transform.localScale.x, 1, 1);
	}

	void DownDust() {
		GameObject d = Instantiate(dust, new Vector3(
			this.transform.position.x + 0.16f * GetForwardScalar(),
			this.transform.position.y - .48f,
			this.transform.position.z
		), Quaternion.identity, this.transform).gameObject;
		d.transform.rotation = Quaternion.Euler(0, 0, 90 * GetForwardScalar());
		d.transform.parent = null;
	}

	void Backstep() {
		StopWallTimeout();
		SoundManager.SmallJumpSound();
		anim.SetTrigger("BackStep");
		backstepCooldown = true;
		InterruptAttack();
		inMeteor = false;
		SetInvincible(true);
		FlashCyan();
		envDmgSusceptible = false;
		Freeze();
		ForwardDust();
		inBackstep = true;
	}

	public void EndBackstep() {
		InterruptBackstep();
		BackwardDust();
	}

	void InterruptBackstep() {
		SetInvincible(false);
		StopFlashingCyan();
		UnFreeze();
		Invoke("EnableBackstep", backstepCooldownLength);
		inBackstep = false;
	}

	public void EnableBackstep() {
		backstepCooldown = false;
	}

	void OnEnviroDamage() {
		this.envDmgSusceptible = false;
		Invoke("EnableEnviroDamage", .2f);
	}

	void EnableEnviroDamage() {
		this.envDmgSusceptible = true;
	}

	public void ForceWalking() {
		this.forcedWalking = true;
	}

	public void StopForcedWalking() {
		this.forcedWalking = false;
	}

	public PlayerTriggeredObject CheckInsideTrigger() {
		int layerMask = 1 << LayerMask.NameToLayer(Layers.Triggers);
		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, Vector2.up, .1f, layerMask);
		if (hit) {
			if (hit.transform.GetComponent<PlayerTriggeredObject>() != null) {
				return hit.transform.GetComponent<PlayerTriggeredObject>();
			}
		} 
		return null;
	}

	public void AnimFoostep() {
		SoundManager.FootFallSound();
	}
}
