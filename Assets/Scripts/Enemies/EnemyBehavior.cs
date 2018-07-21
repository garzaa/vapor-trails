using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {

	[HideInInspector] public GameObject playerObject;
	[HideInInspector] public Enemy mainController;
	[HideInInspector] public Rigidbody2D rb2d;
	[HideInInspector] public Animator anim;

	//distance to the player at which to stop moving towards them
	public float minSeekThreshold = .2f;
	public float maxSeekThreshold = 5f; //or ~275px

	public float playerDistance;

	void Start() {
		mainController = this.gameObject.GetComponent<Enemy>();
		playerObject = GameObject.Find("Player");
		rb2d = this.GetComponent<Rigidbody2D>();
		anim = this.GetComponent<Animator>();
	} 

	void Update() {
		playerDistance = Mathf.Abs(Vector2.Distance(this.transform.position, playerObject.transform.position));
	}

	public virtual void Move(){}

	public virtual void OnGroundHit() {

	}

	public virtual void OnGroundLeave() {
		
	}
}