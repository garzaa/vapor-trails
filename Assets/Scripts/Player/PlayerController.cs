using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity {
	public const float moveSpeed = 3.5f;
	const float jumpSpeed = 3.8f;
	const float jumpCutoff = 2.0f;
	const float hardLandSpeed = -4f;
	const float dashSpeed = 7f;
	const float terminalFallSpeed = -10f;
	const float dashCooldownLength = .6f;
	const float ledgeBoostSpeed = 2f;
	const float stunLength = 0.4f;
	const float parryLength = 10f/60f;
	const float coyoteTime = 0.1f;
	const float airControlAmount = 10f;
	const float restingGroundDistance = 0.3f;
	bool hardFalling = false;

	//these will be loaded from the save
	public int currentHP;
	public int currentEnergy;
	public int maxEnergy;
	public int maxHP;

	public int parryCount = 0;
	public int baseDamage = 1;
	float invincibilityLength = 1f;
	float selfDamageHitstop = .2f;
	int healCost = 1;
	int healAmt = 1;
	float jumpBufferDuration = 0.1f;
	float combatCooldown = 2f;
	float combatStanceCooldown = 4f;
	float sdiMultiplier = 0.1f;
	float preDashSpeed;
	bool perfectDashPossible;
	bool earlyDashInput;
	public bool canInteract = true;
	bool canFlipKick = true;
	bool canShortHop = true;
	Vector2 lastSafeOffset;
	GameObject lastSafeObject;
	SpeedLimiter speedLimiter;
	public GameObject parryEffect;
	bool canParry = false;
	bool movingForwardsLastFrame;
	float missedInputCooldown = 20f/60f;

	//linked components
	Rigidbody2D rb2d;
	Animator anim;
	public WallCheck wallCheck;
	public GameObject hurtboxes;
	SpriteRenderer spr;
	Material defaultMaterial;
    Material cyanMaterial;
	Transform gunEyes;
	public Gun gun;
	public ContainerUI energyUI;
	public BarUI healthBarUI;
	public BarUI energyBarUI;
	public ParticleSystem deathParticles;
	InteractAppendage interaction;
	PlayerUnlocks unlocks;
	public GameObject targetingSystem;
	TrailRenderer[] trails;
	List<SpriteRenderer> spriteRenderers;
	GroundCheck groundCheck;

	//variables
	public bool grounded = false;
	WallCheckData wall = null;
	int airJumps;
	int airDashes = 1;
	bool dashCooldown = false;
	public bool dashing = false;
	bool inMeteor = false;
	bool terminalFalling = false;
	public bool justLeftWall = false;
	bool justLeftGround = false;
	Coroutine currentWallTimeout;
	bool canShoot = true;
	Coroutine platformTimeout;
	public bool inCutscene;
	bool dead = false;
	Coroutine dashTimeout;
	bool pressedUpLastFrame = false;
	bool runningLastFrame = false;
	bool forcedWalking = false;
	bool bufferedJump = false;
	bool justFlipped = false;
	public ActiveInCombat[] combatActives;

	public PlayerStates currentState;

	//other misc prefabs
	public GameObject selfHitmarker;
	public Transform vaporExplosion;
	public Transform sparkle;
	public GameObject parryParticles;
	GameObject instantiatedSparkle = null;

	string[] deathText = {
		"WARNING: WAVEFORM DESTABILIZED",
		"Core dumped",
		"16: 0xD34DB4B3",
		"ERROR: NO_WAVE",
		"",
		"LOOKAHEAD BRANCH PRUNED",
		"RESETTING"
	};

	void Start() {
		unlocks = GetComponentInParent<SaveWrapper>().save.unlocks;
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		groundCheck = GetComponent<GroundCheck>();
		this.facingRight = false;
        cyanMaterial = Resources.Load<Material>("Shaders/CyanFlash");
		spr = GetComponent<SpriteRenderer>();
        defaultMaterial = GetComponent<SpriteRenderer>().material;
		gunEyes = transform.Find("GunEyes").transform;
		gun = GetComponentInChildren<Gun>();
		interaction = GetComponentInChildren<InteractAppendage>();
		RefreshAirMovement();
		lastSafeOffset = this.transform.position;
		speedLimiter = GetComponent<SpeedLimiter>();
		spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(includeInactive:true));
		combatActives = GetComponentsInChildren<ActiveInCombat>(includeInactive:true);
	}
	
	void Update() {
		Jump();
		Move();
		Shoot();
		Attack();
		Interact();
		Taunt();
		UpdateAnimationParams();
		UpdateUI();
		CheckFlip();
		UpdateWallSliding();
		// Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name);
	}

	void Taunt() {
		if (frozen || stunned) return;
		anim.SetFloat(Buttons.XTAUNT, Input.GetAxis(Buttons.XTAUNT));
		anim.SetFloat(Buttons.YTAUNT, Input.GetAxis(Buttons.YTAUNT));
		if (InputManager.TauntInput()) {
			anim.SetTrigger("Taunt");
		}
	}
	
	void Interact() {
		if (stunned) return;
		if (UpButtonPress() && interaction.currentInteractable != null && !inCutscene && canInteract && grounded) {
			SoundManager.InteractSound();
			InterruptEverything();
			interaction.currentInteractable.InteractFromPlayer(this.gameObject);
			canInteract = false;
			StartCoroutine(InteractTimeout());
		}
	}

	bool IsForcedWalking() {
		return this.forcedWalking || Input.GetKey(KeyCode.LeftControl);
	}

	public void Parry() {
		if (parryCount == 0) {	
			FirstParry();
		} else {
			Hitstop.Run(0.05f);
			StartCombatStanceCooldown();
			Instantiate( 
				parryEffect, 
				// move it forward and to the right a bit
				(Vector2) this.transform.position + (Random.insideUnitCircle * 0.2f) + (Vector2.right*this.ForwardVector()*0.15f), 
				Quaternion.identity,
				this.transform
			);
		}
		parryCount += 1;
		SoundManager.PlaySound(SoundManager.sm.parry);
		canParry = true;
		GainEnergy(1);
		StartCombatCooldown();
		// parries can chain together as long as there's a hit every 0.5 seconds
		CancelInvoke("EndParryWindow");
		Invoke("EndParryWindow", 0.5f);
	}

	public void FirstParry() {
		AlerterText.Alert("Autoparry active");
		GetComponent<AnimationInterface>().SpawnFollowingEffect(2);
		anim.SetTrigger("Parry");
		Instantiate(parryParticles, this.transform.position, Quaternion.identity);
		CameraShaker.Shake(0.1f, 0.1f);
		Hitstop.Run(0.5f);
	}

	public void EndShortHopWindow() {
		canShortHop = false;
	}

	void Attack() {
		if (inCutscene || dead || stunned || wall != null) {
			return;
		}

		if (!inCutscene) { 
			anim.SetFloat("VerticalInput", InputManager.VerticalInput()); 
		}
		
		if (!inCutscene && !forcedWalking) {
			anim.SetBool("SpecialHeld", InputManager.Button(Buttons.SPECIAL));
		} else {
			anim.SetBool("SpecialHeld", false);
		}


		if (InputManager.ButtonDown(Buttons.ATTACK) && !inMeteor) {
			anim.SetTrigger(Buttons.ATTACK);
		} else if (InputManager.ButtonDown(Buttons.PUNCH)) {
			anim.SetTrigger(Buttons.PUNCH);
			// use one trigger to get to the attack state machine, because there are a bunch of transitions to it
			if (!InAttackStates()) anim.SetTrigger(Buttons.ATTACK);
		}
		else if (InputManager.ButtonDown(Buttons.KICK) && !inMeteor) {
			anim.SetTrigger(Buttons.KICK);
			if (!InAttackStates()) anim.SetTrigger(Buttons.ATTACK);
		}
		else if (InputManager.ButtonDown(Buttons.SPECIAL) && InputManager.HasHorizontalInput() && (!frozen || justLeftWall) && Mathf.Abs(InputManager.VerticalInput()) < 0.7f) {
			if (unlocks.HasAbility(Ability.Dash)) {
				Dash();
			}
		}
		else if (InputManager.ButtonDown(Buttons.SPECIAL) && InputManager.VerticalInput() < -0.2f && wall == null && !inMeteor) {
			if (!grounded) {
				if (unlocks.HasAbility(Ability.Meteor)) {
					MeteorSlam();
				}
			} else {
				//Reflect();
			}
		} 
		else if (InputManager.ButtonDown(Buttons.SPECIAL) && canFlipKick && (wall == null) && !grounded && InputManager.VerticalInput() > 0.7f) {
			OrcaFlip();
		} 
		else if (InputManager.BlockInput() && !canParry && unlocks.HasAbility(Ability.Parry)) {
			InterruptEverything();
			anim.SetTrigger(Buttons.BLOCK);
			// i made the poor decision to track the timings with BlockBehaviour.cs
		}
	}

	void Move() {
		if (inCutscene || dead || stunned || frozen) {
			anim.SetFloat("Speed", 0f);
			//if (grounded) rb2d.velocity = Vector2.zero;
			anim.SetFloat("VerticalInput", 0f);
			anim.SetBool("HorizontalInput", false);
			anim.SetFloat("VerticalSpeed", 0f);
			return;
		}

		if (InputManager.Button(Buttons.SURF) 
			&& unlocks.HasAbility(Ability.Surf)
			&& Mathf.Abs(rb2d.velocity.x) > 2f
		) {
			anim.SetBool("Surf", true);
			return;
		} 

		anim.SetBool("Surf", false);

		anim.SetBool("HorizontalInput",  InputManager.HasHorizontalInput());
		anim.SetFloat("VerticalSpeed", rb2d.velocity.y);

		if (InputManager.VerticalInput() < -0.8f && InputManager.ButtonDown(Buttons.JUMP)) {
			EdgeCollider2D[] platforms = groundCheck.TouchingPlatforms();
			if (platforms != null && grounded) {
				DropThroughPlatforms(platforms);
			}
		}

		anim.SetBool("InputBackwards", InputBackwards());

		float modifier = IsForcedWalking() ? 0.4f : 1f;
		float hInput = InputManager.HorizontalInput() * modifier;
		// you can't push forward + down on sticks, so do this
		if (hInput >= 0.5f) hInput = 1f;

		anim.SetFloat("Speed", Mathf.Abs(hInput));

		if (InputManager.HorizontalInput() != 0) {
			float targetXSpeed = hInput * moveSpeed;
			
			// if moving above max speed and not decelerating
			if (IsSpeeding() && MovingForwards()) {
				targetXSpeed = rb2d.velocity.x;
			}
			// if decelerating in the air
			else if (!grounded) {
				targetXSpeed = Mathf.Lerp(rb2d.velocity.x, targetXSpeed, Time.deltaTime * airControlAmount);
			}

			rb2d.velocity = new Vector2(
				targetXSpeed,
				rb2d.velocity.y
			);
		}
		
		movingRight = InputManager.HorizontalInput() > 0;

		//if they've just started running
		if (!runningLastFrame && rb2d.velocity.x != 0 && grounded && Mathf.Abs(hInput) > 0.6f) {
			int scalar = rb2d.velocity.x > 0 ? 1 : -1;
			if (scalar * ForwardScalar() > 0) {
				BackwardDust();
			} else {
				ForwardDust();
			}
			HairBackwards();
		}
		runningLastFrame = Mathf.Abs(hInput) > 0.6f;
		
		if (rb2d.velocity.y < terminalFallSpeed) {
			terminalFalling = true;
			rb2d.velocity = new Vector2(rb2d.velocity.x, terminalFallSpeed);
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

		movingForwardsLastFrame = MovingForwards();

		// due to frame skips or other weird shit, add a little self-healing here
		if (!grounded && rb2d.velocity.y == 0f && !frozen) {
			//Invoke("HealGroundTimeout", 0.5f);
		} else if (grounded || (!grounded && rb2d.velocity.y != 0f)) {
			CancelInvoke("HealGroundTimeout");
		}
	}

	public bool IsSpeeding() {
		return rb2d != null && speedLimiter.IsSpeeding();
	}

	void Jump() {
		if ((frozen && !dashing) || lockedInSpace) {
			return;
		}

		if (InputManager.ButtonDown(Buttons.JUMP)) {			
			if ((grounded || (justLeftGround && rb2d.velocity.y < 0.1f)) && (InputManager.VerticalInput()>=-0.7 || groundCheck.TouchingPlatforms() == null)) {
				GroundJump();
				return;
			}

			if (unlocks.HasAbility(Ability.WallClimb) && (wall != null)) {
				WallJump();
				return;
			}

			if (airJumps > 0 && GetComponent<BoxCollider2D>().enabled && !grounded) {
				if (anim.GetFloat("GroundDistance") < restingGroundDistance+0.05f) {
					GroundJump();
				} else {
					AirJump();
				}
				return;
			}

			if (!grounded) {
				//buffer a jump for a short amount of time for when the player hits the ground/wall
				bufferedJump = true;
				Invoke("CancelBufferedJump", jumpBufferDuration);
				return;
			}
		}

		// shorthop
		if (InputManager.ButtonUp(Buttons.JUMP) && rb2d.velocity.y > jumpCutoff && canShortHop) {
			//if the jump button is released
			//then decrease the y velocity to the jump cutoff
			rb2d.velocity = new Vector2(rb2d.velocity.x, jumpCutoff);
		}
	}

	void GroundJump() {
		if (InputManager.HasHorizontalInput()) {
			BackwardDust();
		} else {
			ImpactDust();
		}
		rb2d.velocity = new Vector2(
			x:rb2d.velocity.x, 
			y:jumpSpeed
		);
		anim.SetTrigger(Buttons.JUMP);
		InterruptAttack();
		SoundManager.SmallJumpSound();
	}

	void WallJump() {
		EndShortHopWindow();
		SoundManager.SmallJumpSound();
		InterruptMeteor();
		FreezeFor(0.1f);
		if (wall!=null) DownDust();
		rb2d.velocity = new Vector2(
			//we don't want to boost the player back to the wall if they just input a direction away from it
			x:moveSpeed * ForwardScalar() * (justLeftWall ? 1 : -1) * 1.2f, 
			y:jumpSpeed
		);
		anim.SetTrigger("WallJump");
		StopWallTimeout();
	}

	void AirJump() {
		SoundManager.SmallJumpSound();
		EndShortHopWindow();
		InterruptMeteor();
		rb2d.velocity = new Vector2(
			x:rb2d.velocity.x, 
			y:jumpSpeed
		);
		ImpactDust();
		airJumps--;
		anim.SetTrigger(Buttons.JUMP);
		InterruptAttack();
		ChangeAirspeed();		
	}

	// instantly change airspeed this frame, for air jumps and other abilitites
	void ChangeAirspeed() {
		// allow instant delta-v on air jump, like in smash bruddas
		float targetXSpeed = InputManager.HorizontalInput() * moveSpeed;
		// if moving above max speed and not decelerating, or no input
		if ((IsSpeeding() && MovingForwards()) || targetXSpeed == 0) {
			targetXSpeed = rb2d.velocity.x;
		}
		rb2d.velocity = new Vector2(
			targetXSpeed, 
			rb2d.velocity.y
		);
		movingRight = InputManager.HorizontalInput() > 0;
	}

	public void Dash() {
		if (dead || frozen || stunned || inCutscene || currentState.Equals(PlayerStates.SURF)) {
			return;
		}

		if (dashCooldown) {
			earlyDashInput = true;
			Invoke("EndEarlyDashInput", missedInputCooldown);
			return;
		}

		/*
		if (!grounded && airDashes < 1) {
			return;
		}
		airDashes--;
		*/

		StartCombatStanceCooldown();
		CameraShaker.MedShake();
		anim.SetTrigger("Dash");
	}

	public void StartDashAnimation(bool backwards) {
		preDashSpeed = Mathf.Abs(rb2d.velocity.x);
		float additive = 0f;
		// backdash always comes from initial fdash, so subtract fdash speed if necessary	
		if (backwards && preDashSpeed>dashSpeed) {
			additive = preDashSpeed - dashSpeed;
		}
		float newSpeed = ((backwards ? additive : preDashSpeed) + dashSpeed);
		rb2d.velocity = new Vector2(
			ForwardScalar() * newSpeed, 
			Mathf.Max(rb2d.velocity.y, 0)
		);
		if (perfectDashPossible && !earlyDashInput) {
			AlerterText.Alert("Recycling boost");
			perfectDashPossible = false;
			CancelInvoke("ClosePerfectDashWindow");
			GainEnergy(1); 
			SoundManager.ShootSound();
		}
		InterruptAttack();
		// if backwards, it'll already have been called
		// don't want a duplicate sound effect
		if (!backwards) SoundManager.DashSound();
		inMeteor = false;
		dashing = true;
		if (grounded) {
			BackwardDust();
		}
		Freeze();
		if (dashTimeout != null) StopCoroutine(dashTimeout);
        dashTimeout = StartCoroutine(StartDashCooldown(dashCooldownLength));
	}

	private void EndEarlyDashInput() {
		earlyDashInput = false;
	}

	public void StopDashAnimation() {
        UnFreeze();
        dashing = false;
        dashTimeout = StartCoroutine(StartDashCooldown(dashCooldownLength));
		StartCombatCooldown();
    }

	private void ClosePerfectDashWindow() {
		perfectDashPossible = false;
	}

	public bool MovingForwards() {
		return (InputManager.HorizontalInput() * ForwardScalar()) > 0;
	}

	override public void ForceFlip() {
		base.ForceFlip();
		justFlipped = true;
		anim.SetBool("JustFlipped", true);
		Invoke("EndFlipWindow", coyoteTime);
	}

	void EndFlipWindow() {
		justFlipped = false;
		anim.SetBool("JustFlipped", false);
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
			dashCooldown = false;
			FlashCyan();
			perfectDashPossible = true;
			Invoke("ClosePerfectDashWindow", 0.2f);
		}
	}

	public void OnLedgeStep() {
	}

	public override void OnGroundHit(float impactSpeed) {
		grounded = true;
		canShortHop = true;
		RefreshAirMovement();
		InterruptAttack();
		StopWallTimeout();
		SaveLastSafePos();
		if (rb2d.velocity.y < -1.5f) ImpactDust();
		anim.SetBool("Grounded", true);
		if (inMeteor) {
			LandMeteor();
			return;
		}
		if (wall!=null && wall.direction==ForwardScalar()) {
			Flip();
		}
		if (hardFalling && !bufferedJump) {
			hardFalling = false;
			rb2d.velocity = new Vector2(
				// the player can be falling backwards
				// don't multiply by HInput to be kinder to controller users
				(Mathf.Abs(rb2d.velocity.x) + (Mathf.Abs(impactSpeed / 4f))) * ForwardScalar(),
				impactSpeed
			);

			if (currentState != PlayerStates.DIVEKICK) {
				CameraShaker.Shake(0.1f, 0.1f);
				SoundManager.HardLandSound();
				if (InputManager.HasHorizontalInput()) {
					BackwardDust();
				} else {
					ImpactDust();
				}
			}

			if (currentState == PlayerStates.DIVEKICK) {
				currentState = PlayerStates.NORMAL;
				//anim.SetInteger("SubState", 100);
			} else if (InputManager.HasHorizontalInput()) {
				anim.SetTrigger("Roll");
			} else {
				rb2d.velocity = new Vector2(
					0,
					rb2d.velocity.y
				);
				anim.SetTrigger("HardLand");
			}
		}
		if (terminalFalling) {
			CameraShaker.Shake(0.2f, 0.1f);
		}
		if (bufferedJump) {
			GroundJump();
			CancelBufferedJump();
		}
	}

	public void CheckFlip() {
        if (frozen || lockedInSpace) {
            return;
        }
        Rigidbody2D rb2d;
        if ((rb2d = GetComponent<Rigidbody2D>()) != null && InputManager.HasHorizontalInput()) {
            if (!facingRight && rb2d.velocity.x > 0 && movingRight)
            {
                Flip();
            }
            else if (facingRight && rb2d.velocity.x < 0 && !movingRight)
            {
                Flip();
            }
        }
    }

	public void OrcaFlip() {
		if (!unlocks.HasAbility(Ability.UpSlash)) {
			return;
		}
		EndShortHopWindow();
		EndDashCooldown();
		InterruptEverything();
		SoundManager.JumpSound();
		canFlipKick = false;
		rb2d.velocity = new Vector2(
			rb2d.velocity.x,
			jumpSpeed * 1.5f
		);
		anim.SetTrigger("UpSlash");
		ChangeAirspeed();
	}

	void RefreshAirMovement() {
		canFlipKick = true;
		airDashes = 1;
		airJumps = unlocks.HasAbility(Ability.DoubleJump) ? 1 : 0;
	}

	void SaveLastSafePos() {
		// save the safe position as an offset of the groundCheck's last hit ground
		GameObject currentGround = groundCheck.currentGround;
		if (currentGround == null || currentGround.GetComponent<UnsafeGround>() != null) {
			return;
		}

		lastSafeObject = groundCheck.currentGround;
		lastSafeOffset = this.transform.position - lastSafeObject.transform.position;
	}


	IEnumerator ReturnToSafety(float delay) {
		yield return new WaitForSecondsRealtime(delay);
		if (this.currentHP <= 0) {
			yield break;
		}
		rb2d.velocity = Vector2.zero;
		FreezeFor(0.4f);
		if (lastSafeObject != null)	{
			GlobalController.MovePlayerTo(lastSafeObject.transform.position + (Vector3) lastSafeOffset);
		}
		UnLockInSpace();
		// override invincibility after the teleport so the player doesn't keep taking env damage
		envDmgSusceptible = true;
	}

	public override void OnGroundLeave() {
		grounded = false;
		justLeftGround = true;
		anim.SetBool("Grounded", false);
		anim.SetBool("JustLeftGround", true);
		StartCoroutine(GroundLeaveTimeout());
	}

	IEnumerator GroundLeaveTimeout() {
		yield return new WaitForSecondsRealtime(coyoteTime);
		anim.SetBool("JustLeftGround", false);
		justLeftGround = false;
	}

	void InterruptAttack() {
		ResetAttackTriggers();
	}

	void InterruptMeteor() {
		anim.SetBool("InMeteor", false);
		inMeteor = false;
	}

	public void ResetAttackTriggers() {
		anim.ResetTrigger(Buttons.ATTACK);
		anim.ResetTrigger(Buttons.PUNCH);
		anim.ResetTrigger(Buttons.KICK);
		anim.ResetTrigger("Hurt");
	}

	void UpdateWallSliding() {
		bool touchingLastFrame = wall!=null;
		wall = wallCheck.GetWall();

		if (!touchingLastFrame && (wall!=null)) {
			OnWallHit(wall);
		} else if (touchingLastFrame && (wall==null)) {
			OnWallLeave();
			return;
		}
		
		if (wall != null) {
			FlipToWall(wall);
		}
	}

	void OnWallHit(WallCheckData wall) {
		EndDashCooldown();
		InterruptMeteor();
		anim.SetBool("TouchingWall", true);
		RefreshAirMovement();
		if (bufferedJump && unlocks.HasAbility(Ability.WallClimb)) {
			WallJump();
			CancelBufferedJump();
		}
	}

	void FlipToWall(WallCheckData wall) {
		if (wall.direction != ForwardScalar()) {
			ForceFlip();
		}
	}

	void OnWallLeave() {
		anim.SetBool("TouchingWall", false);
		//if the player just left the wall, they input the opposite direction for a walljump
		//so give them a split second to use a walljump when they're not technically touching the wall
		if (!grounded) {
			currentWallTimeout = StartCoroutine(WallLeaveTimeout());
		}
		// they'll end up jumping backwards away from the wall, that's dumb
		if (!InputManager.HasHorizontalInput() && !MovingForwards()) {
			ForceFlip();
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
		if (anim != null) anim.speed = 1f;
	}

	public void FlashCyan() {
		foreach (SpriteRenderer x in spriteRenderers) {
			x.material = cyanMaterial;
		}
		StartCoroutine(NormalSprite());
    }

    public void SetInvincible(bool b) {
        this.invincible = b;
		// the opposite of invincible - damage susceptible
		this.envDmgSusceptible = !b;
    }

	void Reflect() {
		anim.SetTrigger("Reflect");
	}

	void MeteorSlam() {
		if (inMeteor || dead) return;
		inMeteor = true;
		anim.SetTrigger("Meteor");
		anim.SetBool("InMeteor", true);
		SoundManager.DashSound();
		rb2d.velocity = new Vector2(
			x:0,
			y:terminalFallSpeed
		);
		StartCombatCooldown();
	}

	void LandMeteor() {
		inMeteor = false;
		anim.SetBool("InMeteor", false);
		rb2d.velocity = Vector2.zero;
		//if called while wallsliding
		anim.ResetTrigger("Meteor");
		SoundManager.ExplosionSound();
		if (currentEnergy > 0) {
			Instantiate(vaporExplosion, transform.position, Quaternion.identity);
		}
		CameraShaker.BigShake();
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
		if (InputManager.ButtonDown(Buttons.PROJECTILE) && canShoot && CheckEnergy() >= 4) {
			Sparkle();
			SoundManager.ShootSound();
			BackwardDust();
			gun.Fire(
				forwardScalar: ForwardScalar(), 
				bulletPos: gunEyes
			);
			LoseEnergy(2);
		}
	}

	public void GainEnergy(int amount) {
		bool notFull = (currentEnergy < maxEnergy);
		currentEnergy += amount;
		if (currentEnergy > maxEnergy) {
			currentEnergy = maxEnergy;
		}
		if (notFull && (currentEnergy == maxEnergy)) AlerterText.Alert("Fully charged");
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

	public override void OnHit(Attack attack) {
		if (dead) {
			return;
		}
		
		CombatMusic.EnterCombat();

		if (!canParry && invincible && !attack.attackerParent.CompareTag(Tags.EnviroDamage)) {
			return;
		}

		bool isEnvDmg = attack.attackerParent.CompareTag(Tags.EnviroDamage);

		if (isEnvDmg) {
			if (envDmgSusceptible) {
				OnEnviroDamage(attack.GetComponent<EnviroDamage>());
				InterruptMeteor();
				if (LayerMask.LayerToName(attack.attackerParent.gameObject.layer) == Layers.Water) {
					RefreshAirMovement();
				}
			}
		} else if (canParry) {
			Parry();
			return;
		}

		CameraShaker.Shake(0.2f, 0.1f);
		StartCombatStanceCooldown();
		DamageBy(attack);
		CancelInvoke("StartParryWindow");

		if (this.currentHP == 0) return;
		
		if (isEnvDmg) InvincibleFor(this.invincibilityLength);

		StunFor(stunLength);
		if (attack.knockBack) {
			Vector2 kv = attack.GetKnockback();
			bool attackerToLeft = attack.attackerParent.transform.position.x < this.transform.position.x;
			if ((attackerToLeft && facingRight) || (!attackerToLeft && !facingRight)) ForceFlip();
			KnockBack(kv);
		}
		//sdi
		rb2d.MovePosition(transform.position + ((Vector3) InputManager.MoveVector()*sdiMultiplier));
	}

	override public void StunFor(float seconds) {
		if (staggerable) {
            stunned = true;
            CancelInvoke("UnStun");
			Animator anim = GetComponent<Animator>();
			anim.SetTrigger("OnHit");
			anim.SetBool("Stunned", true);
			// play immediate in hitstun1
			anim.Update(0.1f);
            Invoke("UnStun", seconds);
		}
	}

	public void EnableSkeleton() {
		anim.SetBool("Skeleton", true);
	}

	public void DisableSkeleton() {
		anim.SetBool("Skeleton", false);
	} 

	IEnumerator NormalSprite() {
		yield return new WaitForSecondsRealtime(.1f);
		spriteRenderers.ForEach(x => {
			x.material = defaultMaterial;
		});
		if (spr != null) {
        	spr.material = defaultMaterial;
		}
	}

	IEnumerator WaitAndSetVincible(float seconds) {
		yield return new WaitForSeconds(seconds);
		SetInvincible(false);
	}

	void InvincibleFor(float seconds) {
		SetInvincible(true);
		StartCoroutine(WaitAndSetVincible(seconds));
	}

	void DamageBy(Attack attack) {
		if (attack.damage == 0) return;

		Instantiate(selfHitmarker, this.transform.position, Quaternion.identity, null);
		SoundManager.PlayerHurtSound();
		currentHP -= attack.GetDamage();

		Hitstop.Run(selfDamageHitstop);

		if (currentHP <= 0) {
			Die(attack);
		} else if (currentHP > 0 && attack.GetDamage() > 0) {	
			AlerterText.Alert($"WAVEFORM INTEGRITY {currentHP}");
		} else if (currentHP < 4) {
        	AlerterText.Alert("<color=red>WAVEFORM CRITICAL</color>");
		}

	}

	void Die(Attack fatalBlow) {
		AlerterText.AlertList(deathText);
		AlerterText.Alert("CAUSE OF DEATH:");
		AlerterText.Alert(fatalBlow.attackName);
		// if the animation gets interrupted or something, use this as a failsafe
		Invoke("FinishDyingAnimation", 3f);
		this.dead = true;
		SoundManager.PlayerDieSound();
		currentEnergy = 0;
		CameraShaker.Shake(0.2f, 0.1f);
		LockInSpace();
		Freeze();
		anim.SetTrigger("Die");
		anim.SetBool("TouchingWall", false);
		InterruptEverything();
		EndCombatStanceCooldown();
		ResetAttackTriggers();
	}

	public void FinishDyingAnimation() {
		CancelInvoke("FinishDyingAnimation");
		GlobalController.Respawn();
	}

	public void StartRespawning() {
		anim.SetTrigger("Respawn");
		anim.SetBool("Skeleton", false);
		EndRespawnAnimation();
	}

	public void StartRespawnAnimation() {
		Freeze();
	}

	public void EndRespawnAnimation() {
		ResetAttackTriggers();
		RefreshAirMovement();
		UnFreeze();
		InputManager.UnfreezeInputs();
		UnLockInSpace();
		EnableShooting();
		InvincibleFor(1f);
		FullHeal();
		this.dead = false;
		anim.SetTrigger("InstantKneel");
	}

	public bool IsDead() {
		return this.dead;
	}

	public void FullHeal() {
		currentHP = maxHP;
		currentEnergy = maxEnergy;
	}

	void UpdateUI() {
		energyBarUI.current = currentEnergy;
		energyBarUI.max = maxEnergy;
		healthBarUI.current = currentHP;
		healthBarUI.max = maxHP;
	}

	IEnumerator WallLeaveTimeout() {
		justLeftWall = true;
		// for wall jump animations
		anim.SetBool("JustLeftWall", true);
		yield return new WaitForSeconds(coyoteTime * 2f);
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

	void DropThroughPlatforms(EdgeCollider2D[] platforms) {
		foreach (EdgeCollider2D platform in platforms) {
			platform.enabled = false;
			platformTimeout = StartCoroutine(EnableCollider(0.5f, platform));
		}
		rb2d.velocity = new Vector2(
			rb2d.velocity.x,
			hardLandSpeed
		);
	}

	IEnumerator EnableCollider(float seconds, EdgeCollider2D platform) {
		yield return new WaitForSeconds(seconds);
		platform.enabled = true;
	}

	void InterruptEverything() {
		InterruptAttack();
		InterruptMeteor();
	}

	public void EnterCutscene(bool invincible = true) {
		InterruptEverything();
		Freeze();
		LockInSpace();
		DisableShooting();
		inCutscene = true;
		SetInvincible(invincible);
		anim.Update(0.5f);
		anim.speed = 0f;
	}

	// exitCutscene is called instead of exitInventory
	// the only difference is invincibility
	// this is now only called by the merchant UI
	public void EnterInventory() {
		InterruptEverything();
		Freeze();
		LockInSpace();
		DisableShooting();
		inCutscene = true;
	}

	public void ExitCutscene() {
		if (TransitionManager.sceneData != null) {
			if (TransitionManager.sceneData.hidePlayer || TransitionManager.sceneData.hidePlayer) {
				return;
			}
		}
		UnFreeze();
		UnLockInSpace();
		EnableShooting();
		anim.speed = 1f;
		SetInvincible(false);
		inCutscene = false;
		anim.speed = 1f;
	}

	public bool IsGrounded() {
		// scene load things
		if (groundCheck == null) return false;
		return groundCheck.IsGrounded();
	}

	public void Heal() {
		if (healCost > currentEnergy || currentHP >= maxHP) {
			return;
		}
		
		if (grounded) {
			ImpactDust();
		}

		SoundManager.HealSound();
		currentHP += healAmt;
		currentEnergy -= healCost;
	}

	public void UpdateAnimationParams() {
		if (healCost > currentEnergy || currentHP >= maxHP || !unlocks.HasAbility(Ability.Heal)) {
			anim.SetBool("CanHeal", false);
		} else {
			anim.SetBool("CanHeal", true);
		}
		anim.SetBool("CanHeartbreak", unlocks.HasAbility(Ability.Heartbreaker));
		anim.SetBool("CanDoubleJump", airJumps > 0);
		anim.SetBool("CanOrcaFlip", canFlipKick);
	}

	public float MoveSpeedRatio() {
		if (rb2d == null) return 0;
		if (speedLimiter.maxSpeedX == 0) return 0;
		return Mathf.Abs(rb2d.velocity.x / speedLimiter.maxSpeedX);
	}

	bool VerticalInput() {
		return (InputManager.VerticalInput() != 0);
	}

	bool UpButtonPress() {
		bool upThisFrame = InputManager.VerticalInput() > 0.9;
		bool b = !pressedUpLastFrame && upThisFrame;
		pressedUpLastFrame = upThisFrame;
		return b;
	}

	void OnEnviroDamage(EnviroDamage e) {
		if (!grounded && e.returnPlayerToSafety) {
			LockInSpace();
			// these two together = ez?
			InvincibleFor(this.invincibilityLength);	
			StartCoroutine(ReturnToSafety(selfDamageHitstop));
		}
	}

	public void ForceWalking() {
		//this.forcedWalking = true;
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
		this.baseDamage = s.basePlayerDamage;
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
		for (int i=0; i<combatActives.Length; i++) {
			combatActives[i].gameObject.SetActive(true);
		}
	}

	public void EndCombatCooldown() {
		for (int i=0; i<combatActives.Length; i++) {
			combatActives[i].gameObject.SetActive(false);
		}
		anim.SetBool("CombatMode", false);
	}

	// called from PlayerCombatBehaviour
	public void StartCombatStanceCooldown() {
		anim.SetLayerWeight(1, 1);
		CancelInvoke("EndCombatStanceCooldown");
		Invoke("EndCombatStanceCooldown", combatStanceCooldown);
	}

	public void EndCombatStanceCooldown() {
		anim.SetLayerWeight(1, 0);
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

	public void StartParryWindow() {
		CancelInvoke("EndParryWindow");
		canParry = true;
		StartCombatCooldown();	
		Invoke("EndParryWindow", parryLength);
	}

	public void EndParryWindow() {
		canParry = false;
		parryCount = 0;
	}

	public void OnAttackLand(Attack attack) {
		// ResetAirJumps();	
	}

	public void OnBoost(AcceleratorController accelerator) {
		RefreshAirMovement();
		InterruptMeteor();
		StartCombatCooldown(); 
		EndShortHopWindow();
		anim.SetTrigger(Buttons.JUMP);
		rb2d.MovePosition((Vector2) accelerator.transform.position + (Vector2.up * 0.32f).Rotate(accelerator.transform.rotation.eulerAngles.z));
		Vector2 v  = accelerator.GetBoostVector();
		rb2d.velocity = new Vector2(
			v.x == 0 ? rb2d.velocity.x : v.x,
			v.y
		);
	}

	void HealGroundTimeout() {
		anim.SetInteger("SubState", 200);
		OnGroundHit(0f);
	}

	public bool InAttackStates() {
		int currentState = anim.GetInteger("SubState");
		return (currentState == 110) || (currentState == 210);
	}
}