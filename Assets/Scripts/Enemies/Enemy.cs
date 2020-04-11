using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Enemy : Entity {

	[HideInInspector] public Rigidbody2D rb2d;

	public int hp;
	[HideInInspector]
	public int totalHP;
	public int moveForce;
	public int maxSpeed;
	public float diStrength = 0.5f;
	public float diScaleMagnitude = 0.1f;
	private int selfJuggleChain;

	public GameObject playerObject;

	[HideInInspector] public Animator anim;
	[HideInInspector] public bool hasAnimator;

	[HideInInspector] public EnemyBehavior[] behaviors;

	Material whiteMaterial;
	Material defaultMaterial;

	bool dead = false;
	Renderer mainChildRenderer;
	bool fakeDamage = false;

	[HideInInspector] public SpriteRenderer spr;
	List<SpriteRenderer> spriteRenderers;

	public AudioClip hitSound;

	public bool burstOnDeath = false;
	public Transform burstEffect;

	GameObject bossResources;
	[SerializeField] bool dropBossResources;

	public bool IsStunned() {
		return stunned;
	}

	override protected void OnEnable() {
		base.OnEnable();
		fakeDamage = hp < 0;
		totalHP = hp;
		rb2d = this.GetComponent<Rigidbody2D>();
		playerObject = GameObject.Find("Player");
		if ((anim = this.GetComponent<Animator>()) != null) {
			this.hasAnimator = true;
			anim.logWarnings = false;
		}
		behaviors = this.GetComponents<EnemyBehavior>();

		spr = this.GetComponent<SpriteRenderer>();
		
		whiteMaterial = Resources.Load<Material>("Shaders/WhiteFlash");
        // vile, but easier than redoing the entire lady of the lake boss fight
		spriteRenderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(includeInactive:true))
			.Where(x => x.GetComponent<IgnoreWhiteFlash>() == null).ToList();
		if (spr != null) {
				defaultMaterial = spr.material;
		} else {
				defaultMaterial = spriteRenderers[0].material;
		}
		mainChildRenderer = GetComponentInChildren<Renderer>();

		if (dropBossResources) {
			bossResources = Resources.Load<GameObject>("Effects/BossResources");
		}
	}

	override public void KnockBack(Vector2 kv) {
		selfJuggleChain++;
		kv += Random.insideUnitCircle * diStrength * (selfJuggleChain*diScaleMagnitude);
		base.KnockBack(kv);
	}

	virtual public void DamageFor(int dmg) {
		CombatMusic.EnterCombat();
		if (fakeDamage) return;
		OnDamage();
		this.hp -= dmg;
		if (this.hp <= 0 && !dead) {
			Die();
		} else {
			WhiteSprite();
		}
	}

	public override void OnHit(Attack attack) {
		if (attack.GetComponent<PlayerAttack>() != null) {
			PlayerAttack a = attack.GetComponent<PlayerAttack>();
			if (a.hitstopLength > 0 && this.hp > attack.GetDamage()) {
				Hitstop.Run(a.hitstopLength);
			}
		}
		DamageFor(attack.GetDamage());
		if (this.hitSound != null && mainChildRenderer.isVisible) {
			SoundManager.PlaySound(this.hitSound);
		}
		StunFor(attack.GetStunLength());
		if (attack.knockBack) {
		KnockBack(attack.GetKnockback());
		}
	}

	protected virtual void Die(){
		CloseHurtboxes();
		this.frozen = true;
		this.dead = true;
		Hitstop.Run(.1f);
		if (dropBossResources) {
			for (int i=0; i<1; i++) {
				Instantiate(bossResources, this.transform.position, Quaternion.identity, null);
			}
		}
		if (this.GetComponent<Animator>() != null && !burstOnDeath) {
			this.GetComponent<Animator>().SetTrigger("Die");
		} else {
			if (burstEffect != null) {
				Burst();
			} else {
				Destroy(this.gameObject);
			}
		}
	}

	// for each added behavior, call it
	virtual protected void Update() {
		if (!stunned) {
			foreach (EnemyBehavior eb in this.behaviors) {
				eb.Move();
			}
		}
	}

	override protected void UnStun() {
		selfJuggleChain = 0;
		base.UnStun();
	}

	//on death, remove damage dealing even though it'll live a little bit while the dying animation finishes
	public void CloseHurtboxes() {
		foreach (Transform child in transform) {
			if (child.gameObject.tag.Equals(Tags.EnemyHurtbox)) {
				child.GetComponent<Collider2D>().enabled = false;
			}
		}
	}

	public void WhiteSprite() {
		if (spriteRenderers == null) {
			return;
		}
		foreach (SpriteRenderer x in spriteRenderers) {
			x.material = whiteMaterial;
		}
		if (spr != null) {
        	spr.material = whiteMaterial;
		}
		StartCoroutine(normalSprite());
    }

	IEnumerator normalSprite() {
		yield return new WaitForSecondsRealtime(.1f);
		spriteRenderers.ForEach(x => {
			x.material = defaultMaterial;
		});
		if (spr != null) {
        	spr.material = defaultMaterial;
		}
		if (anim != null) {
			anim.SetBool("WhiteSprite", false);
		}
	}

	public virtual void OnDamage() {
		anim.SetTrigger("Hurt");
	}

	public void Burst() {
		Instantiate(burstEffect, this.transform.position, Quaternion.identity);
		Destroy();
	}

	public override void OnGroundHit(float impactSpeed) {
		base.OnGroundHit(impactSpeed);
		anim.SetBool("Grounded", true);
		foreach (EnemyBehavior eb in this.behaviors) {
			eb.OnGroundHit();
		}
	}

	public override void OnGroundLeave() {
		anim.SetBool("Grounded", false);
		foreach (EnemyBehavior eb in this.behaviors) {
			eb.OnGroundLeave();
		}
	}
}