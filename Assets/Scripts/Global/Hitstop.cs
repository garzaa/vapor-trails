using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitstop : MonoBehaviour{

	//statically calling instances
	public static Hitstop instance;

	void Awake() {
		instance = this;
	}

	public static void Run(float seconds, Entity e1, Entity e2) {
		instance.StartCoroutine(DoHitstop(seconds, e1, e2));
	}

	static IEnumerator DoHitstop(float seconds, Entity e1, Entity e2) {
		//store last velocities
		bool froze1 = false;
		bool froze2 = false;

		Rigidbody2D rb2d1 = e1.GetComponent<Rigidbody2D>(); 
		Rigidbody2D rb2d2 = e2.GetComponent<Rigidbody2D>();;
		Vector2 lastV1 = Vector2.zero, lastV2 = Vector2.zero;

		//physically lock if necessary
		//stop the animators if possible
		Animator e1Anim = e1.GetComponent<Animator>();
		Animator e2Anim = e2.GetComponent<Animator>();

		if (!e1.inHitstop && !e1.lockedInSpace && !e1.frozen) {
			if (rb2d1 != null) {
				lastV1 = rb2d1.velocity;
			}
			e1.LockInSpace();
			froze1 = true;
			e1.inHitstop = true;

			if (e1Anim != null) {
				e1Anim.speed = 0;
			}
		}	

		if (!e2.inHitstop && !e2.lockedInSpace && !e1.frozen) {
			if (rb2d2 != null) {
				lastV2 = rb2d2.velocity;
			}
			e2.LockInSpace();
			froze2 = true;
			e2.inHitstop = true;
			if (e2Anim != null) {
				e2Anim.speed = 0;
			}
		}

		yield return new WaitForSeconds(seconds);

		//then undo everything for each entity that was frozen
		//in reverse order
		//one of the entities could have died though, so check for that
		if (froze1) {
			if (e1 != null) {
				e1.UnLockInSpace();
				e1.inHitstop = false;
			}
			if (e1Anim != null) {
				e1Anim.speed = 1;
			}
			if (rb2d1 != null) {
				rb2d1.velocity = lastV1;
			}
		}

		if (froze2) {
			if (e2 != null) {
				e2.UnLockInSpace();
				e2.inHitstop = false;
			}
			if (e2Anim != null) {
				e2Anim.speed = 1;
			}
			if (rb2d2 != null) {
				rb2d2.velocity = lastV2;
			}
		}
	}
}