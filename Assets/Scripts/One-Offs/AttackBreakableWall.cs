using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(PersistentEnabled))]
public class AttackBreakableWall : MonoBehaviour {
	// this needs a Ground collider and also a hurtbox trigger on the hit/hurtboxes layer

	public GameObject breakPrefab;
	public AttackData attack;
	public AudioResource burstNoise;
	public bool sendAttackLandTrigger = true;

	bool burst = false;

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag(Tags.PlayerHitbox) && attack.name.Equals(other.GetComponent<PlayerAttack>().attackName)) {
			PlayerAttack a = other.GetComponent<PlayerAttack>();
			GlobalController.pc.OnAttackLand(a);
			burstNoise.Play();
			a.MakeHitmarker(a.transform);

			if (sendAttackLandTrigger) {
				GlobalController.pc.GetComponent<Animator>().SetTrigger("AttackLand");
				a.SelfKnockBack();
			}

			Hitstop.Run(0.2f);

			StartCoroutine(Burst());
		}
	}

	IEnumerator Burst() {
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<Collider2D>().enabled = false;
		Instantiate(breakPrefab, this.transform.position, this.transform.rotation, null);

		// then get neighbors
		// so there's no loopback
		burst = false;

		yield return new WaitForSecondsRealtime(0.05f);
		foreach (AttackBreakableWall w in GetNeighbors()) {
			w.StartCoroutine(w.Burst());
		}

		gameObject.SetActive(false);
	}

	List<AttackBreakableWall> GetNeighbors() {
		// get compass neighbors
		// assuming block size is .64 units
		ContactFilter2D filter = new ContactFilter2D();
		// might accidentally grab some other colliders, who cares
		Collider2D[] hits = new Collider2D[10];

		int layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
		filter.layerMask = layerMask;
		filter.useTriggers = false;

		Physics2D.OverlapCircle(this.transform.position, 0.32f, filter, hits);

		List<AttackBreakableWall> neighbors = new List<AttackBreakableWall>();
		for (int i=0; i<hits.Length; i++) {

			// array is initialized with a size, parts can be empty
			if (!hits[i]) {
				break;
			}

			AttackBreakableWall w = hits[i].GetComponent<AttackBreakableWall>();
			if (w && !w.burst) {
				neighbors.Add(w);
			}
		}

		return neighbors;
	}
}
