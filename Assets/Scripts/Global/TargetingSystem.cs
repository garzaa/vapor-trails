using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetingSystem : MonoBehaviour {

	public List<string> targetedTags;
	List<Transform> targetsInRange;

	public GameObject targetingUI;
	Animator targetAnim;

	List<Ability> playerUnlocks;
	
	bool onPlayer = false;

	void Start() {
		onPlayer = gameObject.GetComponentInParent<PlayerUnlocks>() != null;
		if (!onPlayer) return;
		playerUnlocks = gameObject.GetComponentInParent<PlayerUnlocks>().unlockedAbilities;
	}

	bool CanTarget() {
		return !onPlayer || playerUnlocks.Contains(Ability.GunEyes);
	}

	void OnEnable() {
		targetsInRange = new List<Transform>();
		InvokeRepeating("GarbageCollect", 0, 1);
		if (targetingUI != null) {
			targetAnim = targetingUI.GetComponent<Animator>();
		}
	}


	public Transform GetClosestTarget(Transform gunPos) {		
		float maxDistance = float.PositiveInfinity;
		Transform nearest = null;
		if (targetsInRange == null || targetsInRange.Count == 0) {
			return null;
		}
		foreach (Transform t in targetsInRange) {
			if (t != null && t.gameObject.activeSelf) {
				float currentDistance = Vector2.Distance(t.position, gunPos.position);
				if (currentDistance < maxDistance) {
					// then do a raycast to the target
					if (!CheckTargetRaycast(t)) {
						continue;
					}
					nearest = t;
					maxDistance = currentDistance;
				}
			}
		}
		
		if (nearest == null) {
			return null;
		}

		if (nearest.GetComponent<Hurtbox>() != null && !nearest.GetComponent<Hurtbox>().overrideTargetPosition) {
			return nearest.GetComponent<Hurtbox>().parentObject.transform;
		}

		return nearest;
	}

	void Update() {
		if (!onPlayer) return;
		if (targetingUI == null || !CanTarget()) {
			targetAnim.SetBool("Locked", false);
			return;
		}
		Transform closest = GetClosestTarget(this.transform);
		if (closest != null) {
			if (targetingUI.transform.position != closest.transform.position) {
				targetingUI.transform.position = Vector3.Lerp(targetingUI.transform.position, closest.transform.position, 0.5f);
			}
			if (onPlayer) targetAnim.SetBool("Locked", true);
		} else {
			if (onPlayer) targetAnim.SetBool("Locked", false);
		}
	}

	void OnTriggerEnter2D(Collider2D otherCol) {
		if (CanAttackTag(otherCol)) {
			targetsInRange.Add(otherCol.transform);
		}
	}

	void OnTriggerExit2D(Collider2D otherCol) {
		if (CanAttackTag(otherCol) && targetsInRange.Contains(otherCol.transform)) {
			targetsInRange.Remove(otherCol.transform);
		}
	}

	bool CanAttackTag(Collider2D other) {
		foreach (string goodTag in targetedTags) {
			if (other.CompareTag(goodTag)) {
				return true;
			}
		}
		return false;
	}

	//remove dead/null objects from targeting list, not always invoked on collider2d leaving
	void GarbageCollect() {
		if (targetsInRange.Count == 0) {
			return;
		}

		try {
			List<Transform> toRemove = new List<Transform>();
			foreach (Transform t in targetsInRange) {
				if (t == null) {
					toRemove.Add(t);
				}
			}

			foreach (Transform t in toRemove) {
				toRemove.Remove(t);
			}
		} catch (InvalidOperationException) {
			return;
		}
	}

	bool CheckTargetRaycast(Transform t) {
		int layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
		RaycastHit2D hit = Physics2D.Linecast(
			this.transform.position, 
			t.transform.position,
			layerMask
		);
		if (hit.transform != null) {
			return false;
		}
		else {
			return true;
		}
	}

	void OnDisable() {
		if (targetingUI == null) {
			return;
		}
		targetAnim.SetBool("Locked", false);
	}
}
