using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public bool facingRight = true;
    public bool canFlip = true;
    [HideInInspector] public bool movingRight = false;
    public bool frozen = false;
    public bool lockedInSpace = false;

    public bool stunned = false;
    public bool staggerable = false;
    public Coroutine unStunRoutine;

    public bool invincible = false;
    public bool envDmgSusceptible = true;

    public virtual void Flip() {
        if (!canFlip) {
            return;
        }
        ForceFlip();
    }

    public virtual void ForceFlip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        if (GetComponentInChildren<LookAtObject>() != null) {
            GetComponentInChildren<LookAtObject>().Flip();
        }
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }

    public void CheckFlip() {
        if (frozen || lockedInSpace) {
            return;
        }
        Rigidbody2D rb2d;
        if ((rb2d = GetComponent<Rigidbody2D>()) != null) {
            if (!facingRight && rb2d.velocity.x > 0 && movingRight)
            {
                Flip();
            }
            else if (facingRight && rb2d.velocity.x < 0 && !movingRight)
            {
                Flip();
            }
        }
    }

    public void LockInSpace() {
        Rigidbody2D rb2d;
        if ((rb2d = GetComponent<Rigidbody2D>()) != null) {
            rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            this.lockedInSpace = true;
        }
    }

    public virtual void UnLockInSpace() {
        Rigidbody2D rb2d;
        if ((rb2d = GetComponent<Rigidbody2D>()) != null) {
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            this.lockedInSpace = false;
        }
    }

    //returns the x-direction the entity is facing
    public int ForwardScalar() {
        return facingRight ? 1 : -1;
    }

    public Vector2 ForwardVector() {
        return new Vector2(
            ForwardScalar(),
            1
        );
    }

    public void StunFor(float seconds) {
		if (staggerable) {
			//if the enemy is already stunned, then restart the stun period
            CancelInvoke("UnStun");
            if (this.GetComponent<Animator>() != null) {
                Animator anim = GetComponent<Animator>();
                anim.SetTrigger("OnHit");
                anim.SetBool("Stunned", true);
            }
            Invoke("UnStun", seconds);
		}
	}

	virtual public void KnockBack(Vector2 kv) {
        Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
		if (staggerable && rb2d != null) {
			rb2d.velocity = kv;
		}
	}

	virtual protected void UnStun() {
		stunned = false;
        if (this.GetComponent<Animator>() != null) {
            Animator anim = GetComponent<Animator>();
		    anim.SetBool("Stunned", false);
        }
	}

    public virtual void OnHit(Attack a) {

    }

    public virtual void OnGroundHit(float impactSpeed) {

    }

    public virtual void OnGroundLeave() {
        
    }

    public virtual void Hide() {
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        if (spr != null) {
            spr.enabled = false;
        }
    }

    public virtual void Show() {
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        if (spr != null) {
            spr.enabled = true;
        }
    }

    public bool IsLookingAt(GameObject o) {
        float sign = o.transform.position.x - this.transform.position.x;
        return ((facingRight && sign>0) || (!facingRight && sign<0));
    }

    public bool IsFacing(GameObject other) {
        if (other == null) return false;
        return (facingRight && other.transform.position.x > this.transform.position.x) 
            || (!facingRight && other.transform.position.x < this.transform.position.x);
    }
}