using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInterface : MonoBehaviour {

	public Transform effectPoint;
	Animator anim;
	ParticleSystem ps;

	public GameObject dust;
	public GameObject sparkle;

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

	public void EmitParticles(int particles) {
		ps.Emit(particles);
	}

	public void SpawnEffect(GameObject e) {
		Instantiate(e, Vector3.zero, Quaternion.identity, effectPoint.transform);
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

}
