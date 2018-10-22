using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetingSystem : MonoBehaviour {

	public List<string> targetedTags;
	List<Transform> targetsInRange;

	public GameObject targetingUI;
	Animator targetAnim;

	void Start() {
		targetsInRange = new List<Transform>();
		InvokeRepeating("GarbageCollect", 0, 1);
		targetAnim = targetingUI.GetComponent<Animator>();
	}


	public Transform GetClosestTarget(Transform gunPos) {		
		float maxDistance = float.PositiveInfinity;
		Transform nearest = null;
		if (targetsInRange.Count == 0) {
			return null;
		}
		foreach (Transform t in targetsInRange) {
			if (t != null) {
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
		
		return nearest;
	}

	void Update() {
		Transform closest = GetClosestTarget(this.transform);
		if (closest != null) {
			if (targetingUI.transform.position != closest.transform.position) {
				targetingUI.transform.position = Vector3.Lerp(targetingUI.transform.position, closest.transform.position, 0.5f);
			}
			targetAnim.SetBool("Locked", true);
		} else {
			targetAnim.SetBool("Locked", false);
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
		RaycastHit2D hit = Physics2D.Raycast(this.transform.position, 
											t.transform.position - this.transform.position, 
											Vector3.Distance(t.transform.position, this.transform.position), 
											layerMask);
		if (hit.transform != null) {
			return false;
		}
		else {
			return true;
		}
	}
}
