using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public AudioClip hit;
	public AudioClip explosion;
	public AudioClip shoot;
	public AudioClip jump;
	public AudioClip playerHurt;
	public AudioClip smallJump;
	public AudioClip supercruise;
	public AudioClip dash;
	public AudioClip swing;
	public AudioClip hardland;
	public AudioClip die;
	public AudioClip interact;
	public AudioClip footfall;
	public AudioClip heal;
	public AudioClip itemGet;
	public List<AudioClip> voices;

	public static SoundManager sm;
	public AudioSource a;

	void Start() {
		sm = this;
		a = GetComponent<AudioSource>();
	}

	public static void HitSound() {
		sm.a.PlayOneShot(sm.hit);
	}

	public static void ShootSound() {
		sm.a.PlayOneShot(sm.shoot);
	}

	public static void PlayerHurtSound() {
		sm.a.PlayOneShot(sm.playerHurt);
	}

	public static void PlayerDieSound() {
		sm.a.PlayOneShot(sm.die);
	}

	public static void JumpSound() {
		sm.a.PlayOneShot(sm.jump);
	}

	public static void InteractSound() {
		sm.a.PlayOneShot(sm.interact);
	}

	public static void SmallJumpSound() {
		sm.a.PlayOneShot(sm.smallJump);
	}

	public static void ExplosionSound() {
		sm.a.PlayOneShot(sm.explosion);
	}

	public static void DashSound() {
		sm.a.PlayOneShot(sm.dash);
	}

	public static void VoiceSound(int index) {
		sm.a.PlayOneShot(sm.voices[index]);
	}

	public static void SwingSound() {
		sm.a.PlayOneShot(sm.swing);
	}

	public static void HardLandSound() {
		sm.a.PlayOneShot(sm.hardland);
	}

	public static void FootFallSound() {
		sm.a.PlayOneShot(sm.footfall);
	}

	public static void HealSound() {
		sm.a.PlayOneShot(sm.heal);
	}

	public static void ItemGetSound() {
		sm.a.PlayOneShot(sm.itemGet);
	}
}
