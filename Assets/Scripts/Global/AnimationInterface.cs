using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInterface : MonoBehaviour {

	Animator anim;
	ParticleSystem ps;

	public Transform effectPoint;
	public GameObject dust;
	public GameObject sparkle;

	public List<AudioClip> sounds;

	public List<Activatable> activatables;

	void Start() {
		anim = GetComponent<Animator>();
		ps = GetComponentInChildren<ParticleSystem>();
	}

	public void Dust() {
		SpawnEffect(dust);
	}

	public void Sparkle() {
		SpawnEffect(sparkle);
	}

	public void SpawnEffect(GameObject e) {
		Instantiate(e, Vector3.zero, Quaternion.identity, effectPoint.transform);
	}

	public void EmitParticles(int p) {
		ps.Emit(p);
	}

	public void GameFlag(string flag) {
		GlobalController.AddGameFlag(flag);
	}

	public void StopCameraFollow() {
		GlobalController.playerFollower.DisableFollowing();
	}

	public void StartCameraFollow() {
		GlobalController.playerFollower.EnableFollowing();
	}

	public void AnimationTrigger(string t) {
		anim.SetTrigger(t);
	}

	public void HidePlayer() {
		GlobalController.pc.LockInSpace();
		GlobalController.pc.Freeze();
		GlobalController.pc.DisableShooting();
	}

	public void ShowPlayer() {
		GlobalController.pc.UnLockInSpace();
		GlobalController.pc.UnFreeze();
		GlobalController.pc.EnableShooting();
	}

	public void LoadScene(string sceneName) {
		GlobalController.LoadScene(sceneName);
	}

	public void PlaySound(int soundIndex) {
		SoundManager.sm.a.PlayOneShot(this.sounds[soundIndex]);
	}

	public void Activate(int index) {
		activatables[index].Activate();
	}
}
