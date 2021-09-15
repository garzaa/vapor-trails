using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Entity {
	public const float moveSpeed = 3.5f;
	const float jumpSpeed = 3.8f;
	const float jumpCutoff = 2.0f;
	const float hardLandDistance = .64f * 4f;
	const float dashSpeed = 6f;
	const float terminalFallSpeed = -10f;
	const float dashCooldownLength = .6f;
	const float parryWindowLength = 4f/16f;
	const float coyoteTime = 0.1f;

	const float airControlAmount = 10f;
	const float peakJumpControlMod = 1.5f;
	const float peakJumpCutoff = -1f;

	public const int gunCost = 2;

	const float restingGroundDistance = 0.3f;

	//these will be loaded from the save
	public int currentHP;
	public int maxHP;
	public int currentEnergy;
	public int maxEnergy;
	public GameOptions options;

	public int parryCount = 0;
	public int baseDamage = 1;
	float selfDamageHitstop = .2f;
	int healCost = 1;
	int healAmt = 1;
	float combatCooldown = 2f;
	float combatStanceCooldown = 3f;
	float sdiMultiplier = 0.2f;
	float preDashSpeed;
	bool perfectDashPossible;
	bool earlyDashInput;
	public bool canInteract = true;
	bool canFlipKick = true;
	bool canShortHop = true;
	Vector2 lastSafeOffset;
	GameObject lastSafeObject;
	public SpeedLimiter speedLimiter;
	public GameObject parryEffect;
	bool canParry = false;
	bool movingForwardsLastFrame;
	float missedInputCooldown = 20f/60f;

	//linked components
	Rigidbody2D rb2d;
	Animator anim;
	public WallCheck wallCheck;
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
	AirAttackTracker airAttackTracker;
	GroundData groundData;

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
	Coroutine coyoteTimeout;
	Coroutine exitCutsceneRoutine;
	bool canShoot = true;
	Coroutine platformTimeout;
	public bool inCutscene;
	bool dead = false;
	Coroutine dashTimeout;
	bool pressedUpLastFrame = false;
	bool runningLastFrame = false;
	bool forcedWalking = false;
	bool bufferedJump = false;
	int panicJumpInputs = 0;
	Vector2 velocityLastFrame;
	float fallStart;
	public ActiveInCombat[] combatActives;
	public PlayerAttackGraph attackGraph;

	public PlayerStates currentState;

	public GameObject playerRig;

	//other misc prefabs
	public GameObject selfHitmarker;
	public Transform vaporExplosion;
	public Transform sparkle;
	public GameObject parryParticles;
	public GameObject shieldBreak;
	GameObject instantiatedSparkle = null;
	GameObject diamondShine;
	GameEvent deathEvent;
	PlayerGroundCheck groundCheck;
	PlayerGrabber playerGrabber;


	string[] deathText = {
		"WARNING: WAVEFORM DESTABILIZED",
		"Core dumped",
		"16: 0xD34DB4B3",
		"ERROR: NO_WAVE",
		"",
		"LOOKAHEAD BRANCH PRUNED",
		"RESETTING"
	};

	void Awake() {
		DisableTriggers();
	}

	void Start() {
		unlocks = GlobalController.save.unlocks;
		rb2d = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		options = GlobalController.save.options;
		this.facingRight = false;
        cyanMaterial = Resources.Load<Material>("Shaders/CyanFlash");
		spr = GetComponent<SpriteRenderer>();
        defaultMaterial = GetComponent<SpriteRenderer>().material;
		gunEyes = transform.Find("GunEyes").transform;
		gun = GetComponentInChildren<Gun>();
		interaction = GetComponentInChildren<InteractAppendage>();
		lastSafeOffset = this.transform.position;
		speedLimiter = GetComponent<SpeedLimiter>();
		spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(includeInactive:true));
		combatActives = GetComponentsInChildren<ActiveInCombat>(includeInactive:true);
		diamondShine = Resources.Load("Effects/DiamondShine") as GameObject;
		airAttackTracker = GetComponent<AirAttackTracker>();
		RefreshAirMovement();
		deathEvent = Resources.Load("ScriptableObjects/Events/Player Death") as GameEvent;
		groundCheck = GetComponent<PlayerGroundCheck>();
		groundData = groundCheck.groundData;
		LoadFromSaveData(GlobalController.save);
		EnableTriggers();
	}

    protected override void OnEnable() {
        base.OnEnable();
		
		if (groundData != null) {
			if (groundData.grounded) {
				OnGroundHit(0f);
			}
		}
    }

	void LoadFromSaveData(Save s) {
		this.unlocks = s.unlocks;
		this.maxEnergy = s.maxEnergy;
		this.maxHP = s.maxHP;
		this.currentEnergy = s.currentEnergy;
		this.currentHP = s.currentHP;
		this.baseDamage = s.basePlayerDamage;
		this.options = s.options;
		UpdateUI();
	}

	void OnCollisionEnter2D(Collision2D collision2D) {
		if (this.playerGrabber != null) {
			this.playerGrabber.ReleasePlayer();
		}
	}

	void Update() {
		TrackFallDistance();
		CheckGroundData();
		Jump();
		Move();
		Shoot();
		Attack();
		Interact();
		UpdateAnimationParams();
		UpdateUI();
		CheckFlip();
		UpdateWallSliding();
		// Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name);
	}

	void TrackFallDistance() {
		if (rb2d.velocity.y<0 && velocityLastFrame.y>=0) {
			fallStart = transform.position.y;
		}
	}

	void CheckGroundData() {
		if (groundData.hitGround) {
			OnGroundHit(rb2d.velocity.y);
		} 
		else if (groundData.leftGround) {
			OnGroundLeave();
		}
	}
	
	void Interact() {
		if (stunned) return;

		if (InputManager.ButtonDown(Buttons.INTERACT)
			&& interaction.currentInteractable != null
			&& !inCutscene
			&& canInteract
			&& grounded
		) {
			EndCombatStanceCooldown();
			SoundManager.InteractSound();
			InterruptEverything();
			interaction.currentInteractable.InteractFromPlayer(this.gameObject);
			canInteract = false;
			StartCoroutine(InteractTimeout());
		}
	}

	bool IsForcedWalking() {
		return this.forcedWalking || InputManager.Button(Buttons.WALK);
	}

	public void Parry(Attack attack) {
		if (parryCount == 0) {	
			FirstParry(attack);
		} else {
			Hitstop.Run(0.05f);
			StartCombatStanceCooldown();
			Instantiate( 
				parryEffect, 
				GetParryEffectPosition(attack),
				Quaternion.identity,
				this.transform
			);
		}
		if (!IsFacing(attack.attackerParent.gameObject)) ForceFlip();
		parryCount += 1;
		SoundManager.PlaySound(SoundManager.sm.parry);
		canParry = true;
		StartCombatCooldown();
		// parries can chain together as long as there's a hit every 0.5 seconds
		CancelInvoke("EndParryWindow");
		Invoke("EndParryWindow", 0.5f);
	}

	public void FirstParry(Attack attack) {
		AlerterText.Alert("Autoparry active");
		anim.SetTrigger("Parry");
		Instantiate(
			diamondShine, 
			GetParryEffectPosition(attack),
			Quaternion.identity,
			null
		);
		Instantiate(parryParticles, this.transform.position, Quaternion.identity);
		CameraShaker.Shake(0.1f, 0.1f);
		Hitstop.Run(0.4f);
	}

	public Vector2 GetParryEffectPosition(Attack attack) {
		return Vector2.MoveTowards(transform.position, attack.transform.position, 0.16f);
	}

	public void EndShortHopWindow() {
		canShortHop = false;
	}

	void Attack() {
		if (inCutscene || dead || stunned) {
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

		UpdateAttackGraph();

		if (grounded) {
			if (InputManager.AttackInput() && !InAttackStates()) {
				anim.SetTrigger(Buttons.ATTACK);
			}
		} else {
			if (InputManager.ButtonDown(Buttons.PUNCH)) {
				anim.SetTrigger(Buttons.PUNCH);
				if (!InAttackStates()) anim.SetTrigger(Buttons.ATTACK);
			}
			else if (InputManager.ButtonDown(Buttons.KICK) && !inMeteor) {
				anim.SetTrigger(Buttons.KICK);
				if (!InAttackStates()) anim.SetTrigger(Buttons.ATTACK);
			}
		}

		if (InputManager.ButtonDown(Buttons.SPECIAL) && InputManager.HasHorizontalInput() && (!frozen || justLeftWall) && Mathf.Abs(InputManager.VerticalInput()) < 0.5f) {
			if (unlocks.HasAbility(Ability.Dash)) {
				Dash();
			}
		}
		else if (InputManager.ButtonDown(Buttons.SPECIAL) && InputManager.VerticalInput()<0 && wall == null && !inMeteor) {
			if (!grounded) {
				if (unlocks.HasAbility(Ability.Meteor)) {
					MeteorSlam();
				}
			} else {
				//Reflect();
			}
		} 
		else if (InputManager.ButtonDown(Buttons.SPECIAL) && canFlipKick && (wall == null) && !grounded && InputManager.VerticalInput() > 0.2f) {
			OrcaFlip();
		} 
		else if (InputManager.ButtonDown(Buttons.BLOCK) && !canParry && unlocks.HasAbility(Ability.Parry) && currentEnergy >= 1) {
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

		if (grounded) {
			playerRig.transform.rotation = Quaternion.Euler(playerRig.transform.rotation.eulerAngles.x, playerRig.transform.rotation.eulerAngles.y, groundData.normalRotation); 
		} else {
			playerRig.transform.rotation = Quaternion.identity;
		}

		anim.SetBool("InputBackwards", InputBackwards());

		float modifier = IsForcedWalking() ? 0.4f : 1f;
		float hInput = InputManager.HorizontalInput() * modifier;
		// you can't push forward + down on sticks, so do this
		if (hInput >= 0.5f) hInput = 1f;

		if (wall!=null && wall.direction==Mathf.Sign(hInput)) {
			anim.SetFloat("Speed", 0f);
			return;
		}

		anim.SetFloat("Speed", Mathf.Abs(hInput));

		Vector2 targetVelocity = rb2d.velocity;

		if (InputManager.HorizontalInput() != 0) {
			float targetXSpeed = hInput * moveSpeed;

			if (grounded && InAttackStates()) {
				targetXSpeed = rb2d.velocity.x;
			}
			
			// if moving above max speed and not decelerating
			if (IsSpeeding() && MovingForwards()) {
				targetXSpeed = rb2d.velocity.x;
			}

			// if in the air, without coyote time
			else if (!grounded && !justLeftGround) {
				// better air control at the jump peak
				float controlMod = 1f;
				if (rb2d.velocity.y < 0 && rb2d.velocity.y > peakJumpCutoff) controlMod *= peakJumpControlMod;

				targetXSpeed = Mathf.Lerp(
					rb2d.velocity.x,
					targetXSpeed,
					Time.deltaTime * airControlAmount * controlMod
				);
			}

			targetVelocity = new Vector2(
				targetXSpeed,
				(groundData.grounded) ? 0 : rb2d.velocity.y
			);
		
		} 


		if (!InputManager.HasHorizontalInput()) {
			// if falling without input, snap fall to a vertical line
			if (!grounded && (wall == null) && rb2d.velocity.y < 0 && Mathf.Abs(rb2d.velocity.x) < 2f) {
				targetVelocity.x = 0;
			}
		}

		// mild slope handling
		if (grounded) {
			targetVelocity = targetVelocity.Rotate(groundData.normalRotation);

			// if jumped but didn't release the ground colliders yet
			if (rb2d.velocity.y > targetVelocity.y+2f) {
				targetVelocity.y = rb2d.velocity.y;
			}

		}
		
		rb2d.velocity = targetVelocity;
		
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

		if (!inMeteor && (fallStart-rb2d.transform.position.y > hardLandDistance)) {
			anim.SetBool("FastFalling", true);
		}
		else if (!IsGrounded()) {
			anim.SetBool("FastFalling", false);
		}

		movingForwardsLastFrame = MovingForwards();
		velocityLastFrame = rb2d.velocity;
	}

	public bool IsSpeeding() {
		return rb2d != null && speedLimiter.IsSpeeding();
	}

	void Jump() {
		if ((frozen && !dashing) || lockedInSpace || stunned) {
			return;
		}

		if (InputManager.ButtonDown(Buttons.JUMP)) {			
			if (unlocks.HasAbility(Ability.WallClimb) && (wall != null)) {
				WallJump();
				return;
			}

			if ((grounded || (justLeftGround && rb2d.velocity.y < 0.1f))) {
				if (groundData.platforms.Count > 0 && InputManager.VerticalInput() < -0.2f) {
					DropThroughPlatforms(groundData.platforms);
					return;
				}
				GroundJump();
				return;
			}

			if (airJumps > 0 && GetComponent<BoxCollider2D>().enabled && !grounded) {
				// player can "ground" jump a little before they hit the ground
				if (anim.GetFloat("GroundDistance") < restingGroundDistance+0.05f && !justLeftGround) {
					GroundJump();
				} else {
					AirJump();
				}
				return;
			}

			if (airJumps <= 0) {
				panicJumpInputs++;
				// only set the trigger once to keep it from lingering in flail animation and queuing up another transition
				if (panicJumpInputs == 3) {
					anim.SetTrigger("Flail");
				}
			}

			if (!grounded) {
				//buffer a jump for a short amount of time for when the player hits the ground/wall
				bufferedJump = true;
				Invoke("CancelBufferedJump", InputManager.GetInputBufferDuration());
				return;
			}
		}

		// shorthop
		if (InputManager.ButtonUp(Buttons.JUMP) && options.shortHop  && rb2d.velocity.y > jumpCutoff && canShortHop) {
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
		// disable ground check for a little while to not snap back to ground
		groundCheck.DisableFor(0.1f);
		rb2d.velocity += Vector2.up * jumpSpeed;
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
			// boost the player away from the wall
			x:moveSpeed * -wall.direction * 1.2f, 
			y:jumpSpeed
		);
		anim.SetTrigger("WallJump");
		StopWallTimeout();
	}

	void AirJump() {
		StopCoyoteTimeout();
		SoundManager.JumpSound();
		CameraShaker.TinyShake();
		EndShortHopWindow();
		InterruptMeteor();
		rb2d.velocity = new Vector2(
			x:rb2d.velocity.x, 
			y:jumpSpeed
		);
		ImpactDust();

		// refresh airdash
		if (airDashes < 1) {
			airDashes += 1;
			if (!dashCooldown) {
				anim.SetBool("RedWings", false);
			}
		}

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

		if (!(grounded || justLeftGround)) {
			if (airDashes < 1) {
				return;
			} else {
				airDashes--;
			}
		}

		StartCombatStanceCooldown();
		CameraShaker.MedShake();
		anim.SetTrigger("Dash");
	}

	public void StartDashAnimation(bool backwards) {
		preDashSpeed = Mathf.Abs(rb2d.velocity.x);

		// back dash animation always comes from the initial dash, where the speed boost has already been applied
		float newSpeed = (backwards ? preDashSpeed : preDashSpeed+dashSpeed);
		
		Vector2 targetVelocity = new Vector2(
			ForwardScalar() * newSpeed, 
			groundData.grounded ? 0 : Mathf.Max(rb2d.velocity.y, 0)
		);

		rb2d.velocity = targetVelocity.Rotate(groundData.normalRotation);

		if (perfectDashPossible && !earlyDashInput) {
			AlerterText.Alert("Recycling boost");
			perfectDashPossible = false;
			CancelInvoke("ClosePerfectDashWindow");
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
	    dashTimeout = StartCoroutine(RunDashCooldown(dashCooldownLength));
	}

	private void EndEarlyDashInput() {
		earlyDashInput = false;
	}

	public void StopDashAnimation() {
        UnFreeze();
        dashing = false;
        dashTimeout = StartCoroutine(RunDashCooldown(dashCooldownLength));
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
		anim.SetBool("JustFlipped", true);
		Invoke("EndFlipWindow", coyoteTime);
	}

	void EndFlipWindow() {
		anim.SetBool("JustFlipped", false);
	}

	IEnumerator RunDashCooldown(float seconds) {
        dashCooldown = true;
		anim.SetBool("RedWings", true);
        yield return new WaitForSecondsRealtime(seconds);
        EndDashCooldown();
    }

	void EndDashCooldown() {
		if (dashTimeout != null) {
			StopCoroutine(dashTimeout);
		}
		if (dashCooldown) {
			dashCooldown = false;
			if (grounded || airDashes>0) {
				FlashCyan();
				anim.SetBool("RedWings", false);
			}
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
		if (!groundData.onLedge) {
			StartCoroutine(SaveLastSafePos());
		}
		if (rb2d.velocity.y < -1.5f) {
			ImpactDust();
		}
		anim.SetBool("Grounded", true);
		if (inMeteor) {
			LandMeteor();
			return;
		}
		if (wall!=null && wall.direction==ForwardScalar()) {
			Flip();
		}
		if ((fallStart-transform.position.y > hardLandDistance) && !bufferedJump && rb2d.velocity.y>0) {
			rb2d.velocity = new Vector2(
				// the player can be falling backwards
				// don't multiply by HInput to be kinder to controller users
				(Mathf.Abs(rb2d.velocity.x) + (Mathf.Abs(impactSpeed / 4f))) * ForwardScalar(),
				impactSpeed
			);

			if (currentState != PlayerStates.DIVEKICK && !inMeteor) {
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
		if (attackGraph != null) {
			attackGraph.OnGroundHit();
		}
		if (bufferedJump) {
			GroundJump();
			CancelBufferedJump();
		}

		if (!dashCooldown) {
			anim.SetBool("RedWings", false);
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
		canFlipKick = false;
		rb2d.velocity = new Vector2(
			rb2d.velocity.x,
			jumpSpeed * 1.2f
		);
		anim.SetTrigger("UpSlash");
		ChangeAirspeed();
	}

	public void RefreshAirMovement() {
		canFlipKick = true;
		airJumps = unlocks.HasAbility(Ability.DoubleJump) ? 1 : 0;
		anim.ResetTrigger("Flail");
		panicJumpInputs = 0;
		airAttackTracker.Reset();
		if (airDashes<1 && dashCooldown) {
			FlashCyan();
		}
		anim.SetBool("RedWings", false);
		airDashes = 1;
		StopCoroutine(nameof(RunDashCooldown));
		EndDashCooldown();
	}

	public void OnLedgePop() {
		RefreshAirMovement();
	}

	IEnumerator SaveLastSafePos() {
		// if stunned, just wait a little while
		if (stunned || frozen) {
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(SaveLastSafePos());
			yield break;
		}

		GameObject currentGround = groundData.groundObject;
		if (!currentGround || currentGround.GetComponent<UnsafeGround>()) {
			yield break;
		}

		// offset, in case it's moving
		Vector3 currentOffset = transform.position - currentGround.transform.position;

		// wait, in case it's spikes or something
		yield return new WaitForSeconds(0.5f);

		lastSafeObject = currentGround;
		lastSafeOffset = currentOffset;
	}

	void StartEnvHurtAnimation() {
		if (currentHP <= 0) {
			return;
		}
		anim.SetTrigger("OnEnvDamage");
		LockInSpace();
		speedLimiter.enabled = false;
	}

	void FinishEnvHurtAnimation() {
		if (lastSafeObject) {
			GlobalController.MovePlayerTo(lastSafeObject.transform.position + (Vector3) lastSafeOffset);
		}
		UnLockInSpace();
		speedLimiter.enabled = true;
	}

	public override void OnGroundLeave() {
		grounded = false;
		airDashes = 1;
		justLeftGround = true;
		anim.SetBool("Grounded", false);
		coyoteTimeout = StartCoroutine(GroundLeaveTimeout());
	}

	IEnumerator GroundLeaveTimeout() {
		anim.SetBool("JustLeftGround", true);
		yield return new WaitForSecondsRealtime(coyoteTime);
		anim.SetBool("JustLeftGround", false);
		justLeftGround = false;
	}

	void StopCoyoteTimeout() {
		if (coyoteTimeout != null) {
			StopCoroutine(coyoteTimeout);
		}
		anim.SetBool("JustLeftGround", false);
		justLeftGround = false;
	}

	void InterruptAttack() {
		ResetAttackTriggers();
	}

	void InterruptMeteor() {
		if (anim == null) return;
		anim.SetBool("InMeteor", false);
		inMeteor = false;
	}

	public void ResetAttackTriggers() {
		if (anim == null) return;
		anim.ResetTrigger(Buttons.ATTACK);
		anim.ResetTrigger(Buttons.PUNCH);
		anim.ResetTrigger(Buttons.KICK);
		anim.ResetTrigger(Buttons.BLOCK);
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

		anim.SetBool("TouchingWall", touchingLastFrame);
	}

	void OnWallHit(WallCheckData wall) {
		EndDashCooldown();
		InterruptMeteor();
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
		//if the player just left the wall, they input the opposite direction for a walljump
		//so give them a split second to use a walljump when they're not technically touching the wall
		if (!grounded) {
			currentWallTimeout = StartCoroutine(WallLeaveTimeout());
		}
		ForceFlip();
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
		if (speedLimiter != null) speedLimiter.enabled = false;
		this.inMeteor = false;
		this.frozen = true;
	}

	public void UnFreeze() {
		this.frozen = false;
		if (anim != null) anim.speed = 1f;
		if (speedLimiter != null) speedLimiter.enabled = true;
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
		StartCombatCooldown();
		//if called while wallsliding
		anim.ResetTrigger("Meteor");
		SoundManager.ExplosionSound();
		if (currentEnergy > 0) {
			Instantiate(vaporExplosion, transform.position, Quaternion.identity);
		}
		CameraShaker.MedShake();
	}

	public void OnGrab(PlayerGrabber grabber) {
		this.playerGrabber = grabber;
	}

	public void OnGrabRelease(PlayerGrabber playerGrabber) {
		rb2d.velocity += playerGrabber.GetVelocity();
		this.playerGrabber = null;
	}

	public void Sparkle() {
		if (instantiatedSparkle == null) {
			instantiatedSparkle = (GameObject) Instantiate(sparkle, gunEyes.position, Quaternion.identity, gunEyes.transform).gameObject as GameObject;
		}
	}

	public void Shoot() {
		if (!unlocks.HasAbility(Ability.GunEyes) || frozen || !canShoot || stunned) {
			return;
		}
		if (InputManager.ButtonDown(Buttons.PROJECTILE) && CheckEnergy() >= gunCost) {
			Sparkle();
			SoundManager.ShootSound();
			if (grounded) BackwardDust();
			gun.Fire(
				forwardScalar: ForwardScalar(), 
				bulletPos: gunEyes
			);
			StartCombatCooldown();
			StartCombatStanceCooldown();
			LoseEnergy(gunCost);
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
			currentEnergy -= attack.GetDamage();
			if (currentEnergy < 0) {
				AlerterText.Alert("PARRY BREAK");
				Instantiate(shieldBreak, this.transform.position, shieldBreak.transform.rotation, null);
			} else {
				Parry(attack);
				return;
			}
		}

		CameraShaker.Shake(0.2f, 0.1f);
		StartCombatStanceCooldown();
		DamageBy(attack);
		CancelInvoke(nameof(StartParryWindow));

		if (this.currentHP == 0) return;
		
		if (isEnvDmg) return;

		StunFor(attack.stunLength);

		// asdi
		float actualSDIMultiplier = sdiMultiplier * (attack.gameObject.CompareTag(Tags.EnviroDamage) ? 0 : 1);
		rb2d.MovePosition(transform.position + ((Vector3) InputManager.MoveVector()*actualSDIMultiplier));

		if (attack.knockBack) {
			Vector2 kv = attack.GetKnockback();
			if (attack.knockbackAway) {
				kv = kv.magnitude * (transform.position - attack.transform.position).normalized; 
			}
			if (!IsFacing(attack.gameObject)) ForceFlip();
			rb2d.velocity = kv;
		}
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

	void DamageBy(Attack attack) {
		if (attack.damage == 0) return;

		Instantiate(selfHitmarker, this.transform.position, Quaternion.identity, null);
		SoundManager.PlayerHurtSound();
		currentHP -= attack.GetDamage();
		
		if (attack.instakill) {
			currentHP = 0;
		}

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
		if (options.gameJournalist) {
			AlerterText.Alert("<color=cyan>SECOND WIND</color>");
			SoundManager.HealSound();
			FullHeal();
			return;
		}

		AlerterText.AlertList(deathText);
		AlerterText.Alert($"<color=red>CAUSE OF DEATH:</color>");
		AlerterText.Alert($"<color=red>{fatalBlow.attackName}</color>");
		// if the animation gets interrupted or something, use this as a failsafe
		dead = true;
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
		
		deathEvent.Raise();
	}

	public void StartRespawning() {
		anim.SetTrigger("Respawn");
		EndCombatStanceCooldown();
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
		UnLockInSpace();
		EnableShooting();
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

	void DropThroughPlatforms(List<RaycastHit2D>platforms) {
		foreach (RaycastHit2D hit in platforms) {
			EdgeCollider2D platform = hit.collider.GetComponent<EdgeCollider2D>();
			if (platform == null) continue;
			platform.enabled = false;
			platformTimeout = StartCoroutine(EnableCollider(0.5f, platform));
		}

		rb2d.MovePosition(rb2d.position + Vector2.down*0.1f);
		rb2d.velocity = new Vector2(rb2d.velocity.x, -4f);
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
		if (exitCutsceneRoutine != null) StopCoroutine(exitCutsceneRoutine);
		InterruptEverything();
		Freeze();
		LockInSpace();
		if (rb2d != null) rb2d.velocity = Vector2.zero;
		DisableShooting();
		inCutscene = true;
		SetInvincible(invincible);
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
		if (exitCutsceneRoutine != null) StopCoroutine(exitCutsceneRoutine);
		if (gameObject.activeSelf) exitCutsceneRoutine = StartCoroutine(_ExitCutscene());
	}

	IEnumerator _ExitCutscene() {
		yield return new WaitForEndOfFrame();
		if (TransitionManager.sceneData != null && TransitionManager.sceneData.hidePlayer) {
				yield break;
		}

		UnFreeze();
		UnLockInSpace();
		EnableShooting();
		anim.speed = 1f;
		SetInvincible(false);
		inCutscene = false;
		anim.speed = 1f;
		rb2d.velocity = Vector2.zero;
		exitCutsceneRoutine = null;
	}

	public bool IsGrounded() {
		// scene load things
		if (groundData == null) return false;
		return groundData.grounded;
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
		if (frozen) return 0;
		return Mathf.Abs(rb2d.velocity.x) / speedLimiter.maxSpeedX;
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
		rb2d.velocity = Vector2.zero;
		StopCoroutine(nameof(SaveLastSafePos));
		if (e.returnPlayerToSafety) {
			LockInSpace();
			StartEnvHurtAnimation();
		}
	}

	public void ForceWalking() {
		//this.forcedWalking = true;
	}

	public void StopForcedWalking() {
		this.forcedWalking = false;
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

	IEnumerator InteractTimeout() {
		yield return new WaitForSecondsRealtime(0.5f);
		canInteract = true;
	}

	public override void Hide() {
		if (anim == null) {
			anim = GetComponent<Animator>();
		}
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
		anim.SetFloat("CombatStance", 1);
		CancelInvoke("EndCombatStanceCooldown");
		Invoke("EndCombatStanceCooldown", combatStanceCooldown);
	}

	public void EndCombatStanceCooldown() {
		anim.SetFloat("CombatStance", 0);
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

	public void OnBlockAnimationStart() {
		if (!grounded) {
			// rb2d.velocity = InputManager.LeftStick() * moveSpeed;
		}
	}

	public void StartParryWindow() {
		CancelInvoke(nameof(EndParryWindow));
		canParry = true;
		StartCombatCooldown();	
		Invoke(nameof(EndParryWindow), parryWindowLength);
	}

	public void EndParryWindow() {
		canParry = false;
		parryCount = 0;
		currentEnergy -= 1;
	}

	public void OnAttackLand(Attack attack) {
		if (attackGraph != null) {
			attackGraph.OnAttackLand();
		}
	}

	public void OnBoost(AcceleratorController accelerator) {
		if (playerGrabber != null) playerGrabber.ReleasePlayer();
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

	public bool InAttackStates() {
		int currentState = anim.GetInteger("SubState");
		return (currentState == 110) || (currentState == 210);
	}

	void UpdateAttackGraph() {
		if (attackGraph != null) attackGraph.Update();
	}

	public void EnterAttackGraph(PlayerAttackGraph graph, CombatNode startNode = null) {
		if (attackGraph == graph && startNode == null) return;
		attackGraph = graph;
		attackGraph.Initialize(anim, GetComponent<AttackBuffer>(), rb2d, airAttackTracker);
		attackGraph.EnterGraph(startNode);
	}

	public void ExitAttackGraph() {
		attackGraph = null;
	}

	public void DisableTriggers() {
		if (interaction == null) interaction = GetComponentInChildren<InteractAppendage>();
		interaction.GetComponent<BoxCollider2D>().enabled = false;
	}

	public void EnableTriggers() {
		if (interaction == null) return;
		interaction.GetComponent<BoxCollider2D>().enabled = true;
	}

	public void OnAttackNodeEnter() {
		if (InputBackwards()) ForceFlip();
	}
}
