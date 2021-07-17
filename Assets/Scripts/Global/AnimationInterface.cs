using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInterface : MonoBehaviour {

	Animator anim;
	ParticleSystem ps;

	public float effectDistance = 8f;
	public Transform effectPoint;
	public GameObject fallbackEffectPoint;
	public List<GameObject> effects;

	public List<AudioClip> sounds;

	public List<Activatable> activatables;

	public List<NPC> npcs;

	public List<ParticleSystem> particleSystems;

	void Start() {
		anim = GetComponent<Animator>();
		ps = GetComponentInChildren<ParticleSystem>();
	}

	public void SpawnEffect(int index) {
		Transform pos;
		if (effectPoint != null) {
			pos = effectPoint;
		} else {
			pos = fallbackEffectPoint.transform;
		}
		GameObject g = Instantiate(effects[index], pos.position, Quaternion.identity, pos.transform);
		g.transform.parent = null;
	}

	public void SpawnFollowingEffect(int index) {
		Transform pos = effectPoint;
		if (pos == null) {
			pos = fallbackEffectPoint.transform;
		}
		Instantiate(effects[index], pos.position, Quaternion.identity, pos.transform);
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

	public void ResetPlayerPosition() {
		GlobalController.pc.transform.position = Vector2.zero;
	}

	public void MoveToEffectPoint() {
		this.transform.position = effectPoint.transform.position;
	}

	virtual public void HidePlayer() {
		GlobalController.HidePlayer();
	}

	virtual public void ShowPlayer() {
		GlobalController.ShowPlayer();
	}

	public void Alert(string alertText) {
		AlerterText.Alert(alertText);
	}

	public void LoadScene(string sceneName) {
		GlobalController.LoadScene(sceneName);
	}

	public void PlaySound(int soundIndex) {
		if (Vector2.Distance(this.transform.position, GlobalController.pc.transform.position) < effectDistance) {
			SoundManager.PlaySound(this.sounds[soundIndex]);
		}
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
		GameObject point = effectPoint == null ? fallbackEffectPoint : effectPoint.gameObject;
		GlobalController.playerFollower.FollowTarget(point);
	}

	public void StopFollowingEffectPoint() {
		GlobalController.playerFollower.FollowPlayer();
	}

	public void CameraShake(float seconds) {
		if (Vector2.Distance(this.transform.position, GlobalController.pc.transform.position) < effectDistance) {
			CameraShaker.Shake(0.07f, seconds);
		}
	}

	public void EnterSlowMotion() {
		GlobalController.EnterSlowMotion();
	}

	public void ExitSlowMotion() {
		GlobalController.ExitSlowMotion();
	}

	public void SlowMotionFor(float seconds) {
		GlobalController.SlowMotionFor(seconds);
	}

	public void ShowTitle(string text) {
		string[] splitText = text.Split('/');
		GlobalController.ShowTitleText(splitText[0], splitText[1]);
	}

	public void RandomChoice(int numChoices) {
		string choice = Mathf.FloorToInt(Random.Range(0, numChoices+0.9f)).ToString();
		anim.SetTrigger(choice);
	}

	public void RandomFloatChoice(float max) {
		anim.SetFloat("RandomFloat", Random.Range(0f, max));
	}

	public void Deactivate() {
		this.gameObject.SetActive(false);
	}

	public void FireGun() {
		int forwardScalar = 1;
		if (GetComponent<Entity>() != null) {
			forwardScalar = GetComponent<Entity>().ForwardScalar();
		}
		GetComponent<Gun>().Fire(forwardScalar);
	}

	public void MovePlayerToEffectPoint() {
		Transform target = (effectPoint != null ? effectPoint : fallbackEffectPoint.transform);
		GlobalController.MovePlayerTo(target.position);
	}

	public void RunHitstop(float duration) {
		Hitstop.Run(duration);
	}

	public void PlayAudio(AudioResource audioResource) {
		audioResource.Play();
	}
}
