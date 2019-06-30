using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity {
	//constants
	public float moveSpeed = 3.5f;
	float jumpSpeed = 4.5f;
	float jumpCutoff = 2.0f;
	float hardLandSpeed = -4.5f;
	public float dashSpeed = 8f;
	float terminalSpeed = -10f;
	float superCruiseSpeed = 12f;
	float dashCooldownLength = .5f;
	public bool hardFalling = false;
	float ledgeBoostSpeed = 4f;
	public int currentHP = 1;
	public int currentEnergy = 5;
	public int maxEnergy = 5;
	public int maxHP = 5;
	public int baseDamage = 1;
	float invincibilityLength = .5f;
	int healCost = 1;
	int healAmt = 1;
	float backstepCooldownLength = .2f;
	float jumpBufferDuration = 0.1f;
	float combatCooldown = 2f;
	float preDashSpeed;
	bool perfectDashPossible;
	bool earlyDashInput;
	public bool canInteract = true;
	bool canUpSlash = true;
	bool canShortHop = true;
	Vector2 lastSafeOffset;
	GameObject lastSafeObject;
	SpeedLimiter speedLimiter;

	//linked components
	Rigidbody2D rb2d;
	Animator anim;
	public WallCheck wallCheck;
	public GameObject hurtboxes;
	SpriteRenderer spr;
	Material defaultMaterial;
    Material cyanMaterial;
	Transform gunEyes;
	Gun gun;
	public ContainerUI healthUI;
	public ContainerUI energyUI;
	public ParticleSystem deathParticles;
	InteractAppendage interaction;
	PlayerUnlocks unlocks;
	public GameObject targetingSystem;
	TrailRenderer[] trails;

	//variables
	bool grounded = false;
	bool touchingWall = false;
	int airJumps;
	bool dashCooldown = false;
	public bool dashing = false;
	bool inMeteor = false;
	bool terminalFalling = false;
	bool cyan = false;
	public bool justLeftWall = false;
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
	bool backstepCooldown = false;
	bool forcedWalking = false;
	bool bufferedJump = false;


	//other misc prefabs
	public Transform vaporExplosion;
	public Transform sparkle;
	public Transform dust;
	public GameObject impactParticles;
	GameObject instantiatedSparkle = null;

	string[] deathText = {
		"WARNING: WAVEFORM CRITICAL",
		"Attempting backup",
		"Backup failed!",
		"WARNING: WAVEFORM DESTABILIZED",
		"Shutting down",
		"Critical degradation detected",
		"Error: segmentation fault",
		"Core dumped",
		"8: 0xD34DB4B3"
	};

	void Start() {
		unlocks = GetComponentInParent<PlayerUnlocks>();
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		this.facingRight = false;
		currentHP = 5; //unlocks.maxHP;
		currentEnergy = 5; //unlocks.maxEnergy;
		maxEnergy = 5;
        cyanMaterial = Resources.Load<Material>("Shaders/CyanFlash");
		spr = GetComponent<SpriteRenderer>();
        defaultMaterial = GetComponent<SpriteRenderer>().material;
		gunEyes = transform.Find("GunEyes").transform;
		gun = GetComponentInChildren<Gun>();
		interaction = GetComponentInChildren<InteractAppendage>();
		anim.SetBool("CanSupercruise", unlocks.HasAbility(Ability.Supercruise));
		Flip();
		ResetAirJumps();
		lastSafeOffset = this.transform.position;
		speedLimiter = GetComponent<SpeedLimiter>();
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
		if (Input.GetKeyDown(KeyCode.R)) {
			if (GameObject.FindObjectOfType<Timer>() != null) {
				GameObject.FindObjectOfType<Timer>().Reset();
				this.transform.position = new Vector2(0, -1.69f);
				currentHP = maxHP;
				currentEnergy = maxEnergy;
			}
		}
	}

	void Interact() {
		if (UpButtonPress() && interaction.currentInteractable != null && !inCutscene && canInteract && grounded) {
			SoundManager.InteractSound();
			interaction.currentInteractable.InteractFromPlayer(this.gameObject);
			canInteract = false;
			StartCoroutine(InteractTimeout());
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

	public void AnimationLandMeteor() {
	}

	public void Parry() {
		if (grounded && unlocks.HasAbility(Ability.Parry)) {
			anim.SetTrigger("Parry");
		}
	}

	public void EndShortHopWindow() {
		canShortHop = false;
	}

	void Attack() {
		if (inCutscene) {
			return;
		}

		if (!inCutscene) { 
			anim.SetFloat("VerticalInput", InputManager.VerticalInput()); 
		}
		
		if (!inCutscene && !forcedWalking) {
			anim.SetBool("SpecialHeld", InputManager.Button(Inputs.SPECIAL));
		} else {
			anim.SetBool("SpecialHeld", false);
		}


		if (InputManager.ButtonDown(Inputs.ATTACK) && !frozen && !inMeteor) {
			anim.SetTrigger(Inputs.ATTACK);
		}
		else if (!grounded && InputManager.Button(Inputs.SPECIAL) && InputManager.VerticalInput() < 0 && !dashing && !supercruise) {
			if (unlocks.HasAbility(Ability.Meteor)) {
				MeteorSlam();
			}
		} 
		else if (InputManager.Button(Inputs.SPECIAL) && canUpSlash && InputManager.VerticalInput() > 0 && !dashing && !supercruise && !touchingWall) {
			UpSlash();
		}

		else if (InputManager.ButtonDown(Inputs.ATTACK) && InputManager.VerticalInput() < 0 && supercruise) {
			anim.SetTrigger(Inputs.ATTACK);
		}
	}

	void Airbrake() {
		rb2d.velocity = Vector2.zero;
		SoundManager.JumpSound();
		EndSupercruise();
	}

	void Move() {
		if (inCutscene) {
			anim.SetFloat("Speed", 0f);
			if (grounded) rb2d.velocity = Vector2.zero;
			anim.SetFloat("VerticalInput", 0f);
			return;
		}

		anim.SetBool("HorizontalInput",  InputManager.HasHorizontalInput());
		anim.SetFloat("VerticalSpeed", rb2d.velocity.y);

		if (InputManager.ButtonDown(Inputs.JUMP) && supercruise) {
			EndSupercruise();
		}

		if (supercruise && !grounded && !touchingWall && !MovingForwards() && InputManager.HorizontalInput() != 0) {
			Airbrake();
			return;
		}

		if (supercruise && rb2d.velocity.x == 0) {
			InterruptSupercruise();
		}

		if (InputManager.Button(Inputs.SPECIAL) && InputManager.HasHorizontalInput() && (!frozen || justLeftWall) && Mathf.Abs(InputManager.VerticalInput()) <= 0.2f) {
			if (unlocks.HasAbility(Ability.Dash)) {
				Dash();
			} 
		}

		if (!frozen && !(stunned || dead)) {
			if (InputManager.VerticalInput() < 0) {
				EdgeCollider2D platform = GetComponent<GroundCheck>().TouchingPlatform();
				if (platform != null && grounded) {
					DropThroughPlatform(platform);
				}
			}

			anim.SetBool("InputBackwards", InputBackwards());

			float modifier = IsForcedWalking() ? 0.4f : 1f;
			float hInput = InputManager.HorizontalInput() * modifier;
			if (!touchingWall && !wallCheck.TouchingLedge()) {
				anim.SetFloat("Speed", Mathf.Abs(hInput));
			} else {
				anim.SetFloat("Speed", 0);
			}

			if (InputManager.HorizontalInput() != 0) {
				float xVec= hInput * moveSpeed;
				if (IsSpeeding() && MovingForwards()) {
					xVec = rb2d.velocity.x;
				}
				rb2d.velocity = (new Vector2(
					xVec, 
					rb2d.velocity.y)
				);
				movingRight = InputManager.HorizontalInput() > 0;
			}
			//if they've just started running
			if (!runningLastFrame && rb2d.velocity.x != 0 && grounded && Mathf.Abs(hInput) > 0.6f && !touchingWall) {
				int scalar = rb2d.velocity.x > 0 ? 1 : -1;
				if (scalar * ForwardScalar() > 0) {
					BackwardDust();
				} else {
					ForwardDust();
				}
				HairBackwards();
			}

			runningLastFrame = Mathf.Abs(hInput) > 0.6f;
		}

		if (dashing) {
			rb2d.velocity = new Vector2(
				ForwardScalar() * (dashSpeed + preDashSpeed), 
				Mathf.Max(rb2d.velocity.y, 0)
			);
		} else if (supercruise) {
			float maxV = Mathf.Max(Mathf.Abs(superCruiseSpeed), Mathf.Abs(rb2d.velocity.x)) * ForwardScalar();
			rb2d.velocity = new Vector2(maxV, 0);
		}

		if (rb2d.velocity.y < terminalSpeed) {
			terminalFalling = true;
			rb2d.velocity = new Vector2(rb2d.velocity.x, terminalSpeed);
		} else {
			terminalFalling = false;
		}

		if (rb2d.velocity.y < hardLandSpeed && !inMeteor) {
			hardFalling = true;
			anim.SetBool("FastFalling", true);
		}
		else if (!IsGrounded()) {
			hardFalling = false;
			anim.SetBool("FastFalling", false);
		}

		if (wallCheck.TouchingLedge() && !grounded) {
			LedgeBoost();
		}

		if (touchingWall && !grounded && !InputManager.HasHorizontalInput()) {
			rb2d.velocity = new Vector2(0, rb2d.velocity.y);
		}
	}

	public bool IsSpeeding() {
		if (rb2d == null) return false;
		return speedLimiter.IsSpeeding();
	}

	void Jump() {
		if (frozen || (wallCheck.TouchingLedge() && !grounded) || lockedInSpace) {
			return;
		}

		if (InputManager.ButtonDown(Inputs.JUMP)) {
			if (grounded && (InputManager.VerticalInput() >= 0)) {
				GroundJump();
			}
			else if (unlocks.HasAbility(Ability.WallClimb) && (touchingWall || justLeftWall)) {
				WallJump();
			}
			else if (airJumps > 0 && GetComponent<BoxCollider2D>().enabled && !grounded) {
				AirJump();
			}
			else if (!grounded) {
				//buffer a jump for a short amount of time for when the player hits the ground/wall
				bufferedJump = true;
				Invoke("CancelBufferedJump", jumpBufferDuration);
			}
		}

		//emulate an analog jump
		if (InputManager.ButtonUp(Inputs.JUMP) && rb2d.velocity.y > jumpCutoff && canShortHop) {
			//if the jump button is released
			//then decrease the y velocity to the jump cutoff
			rb2d.velocity = new Vector2(rb2d.velocity.x, jumpCutoff);
		}
	}

	float AdditiveJumpSpeed() {
		// return rb2d.velocity.y > 0 ? rb2d.velocity.y : 0;
		return 0;
	}

	void GroundJump() {
		SaveLastSafePos();
		if (InputManager.HasHorizontalInput()) {
			BackwardDust();
		} else {
			ImpactDust();
		}
		rb2d.velocity = new Vector2(
			x:rb2d.velocity.x, 
			y:jumpSpeed + AdditiveJumpSpeed()
		);
		anim.SetTrigger(Inputs.JUMP);
		InterruptAttack();
		SoundManager.SmallJumpSound();
	}

	void WallJump() {
		EndShortHopWindow();
		SoundManager.SmallJumpSound();
		InterruptDash();
		InterruptMeteor();
		if (touchingWall) DownDust();
		InterruptAttack();
		FreezeFor(.1f);
		rb2d.velocity = new Vector2(
			//we don't want to boost the player back to the wall if they just input a direction away from it
			x:moveSpeed * ForwardScalar() * (justLeftWall ? 1 : -1), 
			y:jumpSpeed + AdditiveJumpSpeed()
		);
		Flip();
		anim.SetTrigger("WallJump");
		currentWallTimeout = StartCoroutine(WallLeaveTimeout());
	}

	void AirJump() {
		SoundManager.JumpSound();
		EndShortHopWindow();
		InterruptMeteor();
		rb2d.velocity = new Vector2(
			x:rb2d.velocity.x, 
			y:jumpSpeed + AdditiveJumpSpeed() + jumpSpeed/4
		);
		ImpactDust();
		airJumps--;
		anim.SetTrigger(Inputs.JUMP);
		InterruptAttack();
	}

	public void Dash() {
		if (dashCooldown || dashing || dead || touchingWall) {
			// you can't just buttonmash to get the timing right
			if (dashCooldown) {
				earlyDashInput = true;
				Invoke("EndEarlyDashInput", 0.2f);
			}
			return;
		}
		preDashSpeed = Mathf.Abs(rb2d.velocity.x);
		rb2d.velocity = new Vector2(
			ForwardScalar() * (dashSpeed + preDashSpeed), 
			Mathf.Max(rb2d.velocity.y, 0)
		);
		if (perfectDashPossible && !earlyDashInput) {
			AlerterText.Alert("Recycling DASH velocity");
			perfectDashPossible = false;
			CancelInvoke("ClosePerfectDashWindow");
			this.GainEnergy(1);
			SoundManager.ShootSound();
		}
		SoundManager.DashSound();
		StopWallTimeout();
		InterruptAttack();
		inMeteor = false;
        if (unlocks.HasAbility(Ability.DamageDash)) {
            anim.SetTrigger("DamageDash");
			envDmgSusceptible = false;
			SetInvincible(true);
        } else {
			anim.SetTrigger("Dash");
		}
		dashing = true;
		if (grounded) {
			BackwardDust();
		}
		Freeze();
	}

	private void EndEarlyDashInput() {
		earlyDashInput = false;
	}

	public void StopDashing() {
        UnFreeze();
        dashing = false;
        dashTimeout = StartCoroutine(StartDashCooldown(dashCooldownLength));
		envDmgSusceptible = true;
        SetInvincible(false);
		CloseAllHurtboxes();
		if (MovingForwards() && InputManager.Button(Inputs.SPECIAL) && unlocks.HasAbility(Ability.Supercruise)) {
			anim.SetTrigger("StartSupercruise");
		}
    }

	private void ClosePerfectDashWindow() {
		perfectDashPossible = false;
	}

	public bool MovingForwards() {
		return (InputManager.HorizontalInput() * ForwardScalar()) > 0;
	}

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
		if (dashTimeout != null) {
			StopCoroutine(dashTimeout);
		}
		if (dashCooldown) {
			FlashCyanOnce();
			dashCooldown = false;
			perfectDashPossible = true;
			Invoke("ClosePerfectDashWindow", 0.1f);
		}
	}

	public void OnLedgeStep() {
		SaveLastSafePos();
	}

	public override void OnGroundHit() {
		grounded = true;
		canShortHop = true;
		ResetAirJumps();
		InterruptAttack();
		StopWallTimeout();
		SaveLastSafePos();
		if (rb2d.velocity.y > 0 && InputManager.Button(Inputs.JUMP)) {
			LedgeBoost();
			return;
		}
		ImpactDust();
		if (inMeteor) {
			LandMeteor();
		}
		anim.SetBool("Grounded", true);
		if (hardFalling && !bufferedJump) {
			hardFalling = false;
			if (InputManager.HasHorizontalInput() && canFlip) {
				anim.SetTrigger("Roll");
			} else {
				anim.SetTrigger("HardLand");
			}
			SoundManager.HardLandSound();
			if (InputManager.HasHorizontalInput()) {
				BackwardDust();
			} else {
				ImpactDust();
			}
			CameraShaker.Shake(0.05f, 0.1f);
		}
		if (terminalFalling) {
			CameraShaker.Shake(0.1f, 0.1f);
		}
		if (bufferedJump) {
			GroundJump();
			CancelBufferedJump();
		}
	}

	public void UpSlash() {
		if (!unlocks.HasAbility(Ability.UpSlash)) {
			return;
		}
		EndShortHopWindow();
		ImpactDust();
		SoundManager.JumpSound();
		canUpSlash = false;
		rb2d.velocity = new Vector2(
			rb2d.velocity.x,
			jumpSpeed * 1.3f
		);
		anim.SetTrigger("UpSlash");
	}

	void ResetAirJumps() {
		canUpSlash = true;
		airJumps = unlocks.HasAbility(Ability.DoubleJump) ? 1 : 0;
	}

	void SaveLastSafePos() {
		// save the safe position as an offset of the groundCheck's last hit ground
		lastSafeObject = GetComponent<GroundCheck>().currentGround;
		lastSafeOffset = this.transform.position - lastSafeObject.transform.position;
	}


	void ReturnToSafety() {
		if (this.currentHP <= 0) {
			return;
		}
		GlobalController.MovePlayerTo(lastSafeObject.transform.position + (Vector3) lastSafeOffset, withParallax:true);
		UnLockInSpace();
	}

	public override void OnGroundLeave() {
		grounded = false;
		anim.SetBool("Grounded", false);
	}

	void InterruptAttack() {
		ResetAttackTriggers();
	}

	void InterruptMeteor() {
		anim.SetBool("InMeteor", false);
		inMeteor = false;
	}

	public void ResetAttackTriggers() {
		anim.ResetTrigger(Inputs.ATTACK);
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
		InterruptDash();
		if (dashCooldown) {
			EndDashCooldown();
		}
		EndSupercruise();
		//hold to wallclimb
		anim.SetBool("TouchingWall", true);
		if (!grounded) SoundManager.HardLandSound();
		ResetAirJumps();
		if (bufferedJump && unlocks.HasAbility(Ability.WallClimb)) {
			WallJump();
			CancelBufferedJump();
		}
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
		if (this.defaultMaterial != null) spr.material = defaultMaterial;
    }

    public void SetInvincible(bool b) {
        this.invincible = b;
    }

	public void FlashCyanOnce() {
		CyanSprite();
		Invoke("WhiteSprite", 0.1f);
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
		SoundManager.DashSound();
		rb2d.velocity = new Vector2(
			x:0,
			y:terminalSpeed
		);
	}

	void LandMeteor() {
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
			instantiatedSparkle = (GameObject) Instantiate(sparkle, gunEyes.position, Quaternion.identity, gunEyes.transform).gameObject as GameObject;
		}
	}

	public void Shoot() {
		if (!unlocks.HasAbility(Ability.GunEyes) || inCutscene) {
			return;
		}
		if (InputManager.ButtonDown(Inputs.PROJECTILE) && canShoot && CheckEnergy() >= 1) {
			Sparkle();
			SoundManager.ShootSound();
			BackwardDust();
			gun.Fire(
				forwardScalar: ForwardScalar(), 
				bulletPos: gunEyes
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
		if (inMeteor || InputManager.VerticalInput() < 0 || supercruise || rb2d.velocity.y > jumpSpeed) {
			return;
		}
		bool movingTowardsLedge = (InputManager.HorizontalInput() * ForwardScalar()) > 0;
		if (movingTowardsLedge) {
			anim.SetTrigger(Inputs.JUMP);
			InterruptDash();
			EndDashCooldown();
			ResetAirJumps();
			InterruptAttack();
			rb2d.velocity = new Vector2(
				x:(IsSpeeding() ? rb2d.velocity.x : speedLimiter.maxSpeedX * ForwardScalar()),
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
				OnEnviroDamage(attack.GetComponent<EnviroDamage>());
				InterruptMeteor();
				if (LayerMask.LayerToName(attack.attackerParent.gameObject.layer) == Layers.Water) {
					ResetAirJumps();
				}
			} else {
				return;
			}
		}

		CameraShaker.Shake(0.2f, 0.1f);
		Hitstop.Run(0.2f);
		InterruptSupercruise();
		DamageFor(attack.GetDamage());
		if (this.currentHP == 0) {
			return;
		} else if (currentHP == 1) {
			AlerterText.Alert("WARNING");
			AlerterText.Alert("BODY WAVEFORM CRITICALLY DAMAGED");
		}
		InvincibleFor(this.invincibilityLength);
		CyanSprite();
		StunFor(attack.GetStunLength());
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
		if (dashing) {
			InterruptDash();
		}
		anim.SetTrigger("Hurt");
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
		currentHP -= dmg;
		if (currentHP <= 0) {
			Die();
		}
	}

	void Die() {
		AlerterText.AlertList(deathText);
		// if the animation gets interrupted or something, use this as a failsafe
		Invoke("FinishDyingAnimation", 3f);
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
		CancelInvoke("FinishDyingAnimation");
		GlobalController.Respawn();
	}

	public void StartRespawning() {
		this.envDmgSusceptible = true;
		anim.SetTrigger("Respawn");
	}

	public void StartRespawnAnimation() {
		Freeze();
		LockInSpace();
	}

	public void EndRespawnAnimation() {
		ResetAttackTriggers();
		ResetAirJumps();
		UnFreeze();
		UnLockInSpace();
		EnableShooting();
		InvincibleFor(1f);
		FullHeal();
		this.dead = false;
	}

	public bool IsDead() {
		return this.dead;
	}

	public void FullHeal() {
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

	public void EnableShooting() {
		this.canShoot = true;
	}

	public void DisableShooting() {
		this.canShoot = false;
	}

	void DropThroughPlatform(EdgeCollider2D platform) {
		UnFreeze();
		InterruptEverything();
		rb2d.velocity = new Vector2(
			rb2d.velocity.x,
			hardLandSpeed
		);
		platform.enabled = false;
		platformTimeout = StartCoroutine(EnableCollider(0.1f, platform));
	}

	IEnumerator EnableCollider(float seconds, EdgeCollider2D platform) {
		yield return new WaitForSeconds(seconds);
		StopPlatformDrop(platform);
	}

	void StopPlatformDrop(EdgeCollider2D platform) {
		if (platformTimeout != null) {
			StopCoroutine(platformTimeout);
		}
		platform.enabled = true;
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

	public bool IsGrounded() {
		return GetComponent<GroundCheck>().IsGrounded();
	}

	//called at the start of the supercruiseMid animation
	public void StartSupercruise() {
		preDashSpeed = Mathf.Abs(rb2d.velocity.x);
		SoundManager.DashSound();
		this.supercruise = true;
		anim.ResetTrigger("InterruptSupercruise");
		anim.ResetTrigger("EndSupercruise");
		BackwardDust();
		Freeze();
		CameraShaker.Shake(0.1f, 0.1f);
		//keep them level
		rb2d.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
	}

	public void EndSupercruise() {
		if (!supercruise) return;		
		StartCombatCooldown();
		supercruise = false;
		UnFreeze();
		rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
		anim.SetTrigger("EndSupercruise");
	}

	//when the player hits a wall or dies 
	public void InterruptSupercruise() {
		if (!supercruise) return;
		StartCombatCooldown();
		CameraShaker.Shake(0.1f, 0.1f);
		supercruise = false;
		UnFreeze();
		rb2d.constraints = rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
		anim.SetTrigger("InterruptSupercruise");
	}

	public void Heal() {
		if (healCost > currentEnergy) {
			return;
		}
		
		if (currentHP < maxHP) {
			if (grounded) {
				ImpactDust();
			}
			SoundManager.HealSound();
			currentHP += healAmt;
			currentEnergy -= healCost;
		}
	}

	public void CheckHeal() {
		if (healCost > currentEnergy || currentHP == maxHP || !unlocks.HasAbility(Ability.Heal)) {
			anim.SetBool("CanHeal", false);
		} else {
			anim.SetBool("CanHeal", true);
		}
	}

	public float MoveSpeedRatio() {
		if (rb2d == null) return 0;
		return Mathf.Abs(rb2d.velocity.x / speedLimiter.maxSpeedX);
	}

	bool VerticalInput() {
		return (InputManager.VerticalInput() != 0);
	}

	bool UpButtonPress() {
		bool upThisFrame = InputManager.VerticalInput() > 0;
		bool b = !pressedUpLastFrame && upThisFrame;
		pressedUpLastFrame = upThisFrame;
		return b;
	}

	void ImpactDust() {
		ForwardDust();
		BackwardDust();
	}

	public void ForwardDust() {
		if (!grounded) return;
 		GameObject d = Instantiate(dust, new Vector3(
			this.transform.position.x + 0.32f * ForwardScalar(),
			this.transform.position.y - GetComponent<BoxCollider2D>().bounds.extents.y + .12f,
			this.transform.position.z
		), Quaternion.identity).gameObject;
		d.transform.localScale = new Vector3(-this.transform.localScale.x, 1, 1);
	}

	public void BackwardDust() {
		if (!grounded) return;
		GameObject d = Instantiate(dust, new Vector3(
			this.transform.position.x - 0.32f * ForwardScalar(),
			this.transform.position.y - GetComponent<BoxCollider2D>().bounds.extents.y + .12f,
			this.transform.position.z
		), Quaternion.identity).gameObject;
		d.transform.localScale = new Vector3(this.transform.localScale.x, 1, 1);
	}

	void DownDust() {
		GameObject d = Instantiate(dust, new Vector3(
			this.transform.position.x + 0.16f * ForwardScalar(),
			this.transform.position.y - .48f,
			this.transform.position.z
		), Quaternion.identity, this.transform).gameObject;
		d.transform.rotation = Quaternion.Euler(0, 0, 90 * ForwardScalar());
		d.transform.parent = null;
	}

	void Backstep() {
		StopWallTimeout();
		SoundManager.ShootSound();
		anim.SetTrigger("BackStep");
		backstepCooldown = true;
		InterruptAttack();
		inMeteor = false;
		SetInvincible(true);
		envDmgSusceptible = false;
		Freeze();
	}

	public void EndBackstep() {
		InterruptBackstep();
		BackwardDust();
	}

	void InterruptBackstep() {
		SetInvincible(false);
		UnFreeze();
		Invoke("EnableBackstep", backstepCooldownLength);
	}

	public void EnableBackstep() {
		backstepCooldown = false;
	}

	void OnEnviroDamage(EnviroDamage e) {
		this.envDmgSusceptible = false;
		if (!grounded && e.returnPlayerToSafety) {
			LockInSpace();
			Invoke("ReturnToSafety", 0.2f);
		}
		StunFor(e.stunLength);
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

	void CancelBufferedJump() {
		this.CancelInvoke("CancelBufferedJump");
		this.bufferedJump = false;
	}

	override public void UnLockInSpace() {
		base.UnLockInSpace();
		this.transform.rotation = Quaternion.identity;
	}

	public void LoadFromSaveData(Save s) {
		this.unlocks = s.unlocks;
		this.maxEnergy = s.maxEnergy;
		this.maxHP = s.maxHP;
		this.currentEnergy = s.currentEnergy;
		this.currentHP = s.currentHP;
		UpdateUI();
	}

	IEnumerator InteractTimeout() {
		yield return new WaitForSecondsRealtime(1);
		canInteract = true;
	}

	public override void Hide() {
		anim.SetBool("Hidden", true);
	}

	public override void Show() {
		if (anim == null) {
			anim = GetComponent<Animator>();
		}
		anim.SetBool("Hidden", false);
	}

	public void Sit() {
		FreezeFor(1f);
	}

	// called from the beginning of special move animations
	public void StartCombatCooldown() {
		anim.SetBool("CombatMode", true);
		CancelInvoke("EndCombatCooldown");
		Invoke("EndCombatCooldown", combatCooldown);
	}

	public void EndCombatCooldown() {
		anim.SetBool("CombatMode", false);
	}

	// called from animations
	public void HairForwards() {
		anim.SetTrigger("HairForwards");
	}

	// also called from animation
	public void HairBackwards() {
		anim.SetTrigger("HairBackwards");
	}

	public void DisableTrails() {
		trails = GetComponentsInChildren<TrailRenderer>();
		foreach (TrailRenderer t in trails) {
			t.gameObject.SetActive(false);
		}
	}

	public void EnableTrails() {
		foreach (TrailRenderer t in trails) {
			t.gameObject.SetActive(true);
		}
	}
	
	bool InputBackwards() {
		return (ForwardScalar() * Mathf.Ceil(InputManager.HorizontalInput())) < 0;
	}
}
