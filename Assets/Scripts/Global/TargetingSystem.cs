using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetingSystem : MonoBehaviour {

	public List<string> targetedTags;
	List<Transform> targetsInRange;

	void Start() {
		targetsInRange = new List<Transform>();
		InvokeRepeating("GarbageCollect", 0, 1);
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
					nearest = t;
					maxDistance = currentDistance;
				}
			}
		}
		
		return nearest;
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
}
