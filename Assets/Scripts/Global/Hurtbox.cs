using UnityEngine;

public class Hurtbox : MonoBehaviour {

	public GameObject parentObject;
	public GameObject hitEffect;
	public AudioClip hitSound;
	
	[Header("For Targeting Systems")]
	public bool overrideTargetPosition;


	void Start() {
		if (parentObject == null && GetComponentInParent<Entity>() != null) {
			parentObject = GetComponentInParent<Entity>().gameObject;
		}
	}

	public Entity GetParent() {
		if (parentObject == null) return null;
		return parentObject.GetComponent<Entity>();
	}

	public Transform GetTargetPosition() {
		if (overrideTargetPosition || parentObject==null) return this.transform;
		return parentObject.transform;
	}

	virtual public bool OnHit(Attack a) {
		PropagateHitEvent(a);
		if (a.hitmarker != null) a.MakeHitmarker(this.transform);
		if (hitEffect != null) Instantiate(hitEffect, this.transform.position, Quaternion.identity, null);
		if (hitSound != null) {
			SoundManager.WorldSound(hitSound);
		}
		return true;
	}

	virtual public void PropagateHitEvent(Attack attack) {
		if (parentObject != null) {
			parentObject.GetComponent<Entity>().OnHit(attack);
		}
	}
}
