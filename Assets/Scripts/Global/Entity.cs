using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool movingRight = false;
    public bool frozen = false;
    public bool lockedInSpace = false;
	public bool inHitstop = false;

    public void Flip() 
	{
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        //flip by scaling -1
    }

    public void Destroy() {
        Destroy(this.gameObject);
    }

    public void CheckFlip() {
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

    public void UnLockInSpace() {
        Rigidbody2D rb2d;
        if ((rb2d = GetComponent<Rigidbody2D>()) != null) {
            rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            this.lockedInSpace = false;
        }
    }

    //returns the x-direction the entity is facing
    public int GetForwardScalar() {
        return facingRight ? 1 : -1;
    }

    public virtual void OnHit(Attack a) {

    }
}