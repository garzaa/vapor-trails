using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInterface : MonoBehaviour {

	Animator anim;
	ParticleSystem ps;

	public Transform effectPoint;
	public List<GameObject> effects;

	public List<AudioClip> sounds;

	public List<Activatable> activatables;

	public List<NPC> npcs;

	void Start() {
		anim = GetComponent<Animator>();
		ps = GetComponentInChildren<ParticleSystem>();
	}

	public void SpawnEffect(int index) {
		Instantiate(effects[index], effectPoint.transform.position, Quaternion.identity, null);
	}

	public void EmitParticles(int p) {
		ps.Emit(p);
	}

	public void GameFlag(GameFlag flag) {
		GlobalController.AddGameFlag(flag);
	}

	public void StopCameraFollow() {
		GlobalController.playerFollower.DisableFollowing();
	}

	public void StartCameraFollow() {
		GlobalController.playerFollower.EnableFollowing();
	}

	public void SelfAnimationTrigger(string t) {
		anim.SetTrigger(t);
	}

	public void HidePlayer() {
		GlobalController.pc.LockInSpace();
		GlobalController.pc.Freeze();
		GlobalController.pc.DisableShooting();
		GlobalController.pc.Hide();
	}

	public void ShowPlayer() {
		GlobalController.pc.UnLockInSpace();
		GlobalController.pc.UnFreeze();
		GlobalController.pc.EnableShooting();
		GlobalController.pc.Show();
	}

	public void LoadScene(string sceneName) {
		GlobalController.LoadScene(sceneName);
	}

	public void PlaySound(int soundIndex) {
		SoundManager.sm.a.PlayOneShot(this.sounds[soundIndex]);
	}

	public void HitActivatable(int index) {
		activatables[index].Activate();
	}

	public void CutsceneCallback() {
		GlobalController.CutsceneCallback();
	}

	public void FlashWhite() {
		GlobalController.FlashWhite();
	}

	public void FollowEffectPoint() {
		GlobalController.playerFollower.FollowTarget(this.effectPoint.gameObject);
	}

	public void StopFollowingEffectPoint() {
		GlobalController.playerFollower.FollowPlayer();
	}

	public void CameraShake(float seconds) {
		CameraShaker.Shake(0.1f, seconds);
	}

	public void EnterSlowMotion() {
		GlobalController.EnterSlowMotion();
	}

	public static void ExitSlowMotion() {
		GlobalController.ExitSlowMotion();
	}
}
