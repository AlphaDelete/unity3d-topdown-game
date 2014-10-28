using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	public float Speed;
	public Animator Anim;

	private Vector3 Movement;
	private float timerAtk = 0.4f;
	private float attackTimer ;
	private int attacking = 0;

	// Access camera to shake
	public GameObject playerCamera;	
	private CameraMovement moveCamera;

	void Start() {
		moveCamera = playerCamera.GetComponent<CameraMovement>();
	}

	void Update() {
		setAttack();
	}

	void FixedUpdate()
	{
		// Set Player movement
		setMovement();
	}

	void OnTriggerEnter(Collider other) {
		//Destroy(other.gameObject);
		Transform parentCollider = other.gameObject.transform.parent;
		if (parentCollider.tag == "Pickup") 
		{
			Debug.Log(parentCollider.tag);
			parentCollider.GetComponent<SpriteRenderer>().sortingLayerName = "Pickup";
			parentCollider.transform.parent = this.transform; 
			parentCollider.transform.localPosition = new Vector3(0, 0.6f, 0);
		}

		/*
		 	//trow
			parentCollider.transform.parent = null;
			parentCollider.rigidbody.AddForce (transform.forward * 8000);
		*/

	}

	#region Movement
	void setMovement() {
		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		float moveVertical = Input.GetAxisRaw("Vertical");

		if ( moveHorizontal == 0 && moveVertical == 0) {
			Anim.SetBool("walking", false);
		} else {
			Anim.SetBool("walking", true);
			Anim.SetFloat("xAxis", moveHorizontal);
			Anim.SetFloat("yAxis", moveVertical);
		}

		Movement = new Vector2(moveHorizontal, moveVertical);
		rigidbody.velocity = Movement.normalized * Speed;
	}
	#endregion

	#region Attack
	void setAttack() {
		if (Input.GetButtonDown ("Fire1") && attackTimer == 0) {
			// Set the attack anim
			attacking = 1;
			attackTimer = timerAtk;
			// Set camera to shake (Second, Magnitude)
			if (moveCamera.isShaking < 2) StartCoroutine(moveCamera.shakeCamera(0.3F, 0.015F));
		}

		if(attackTimer > 0)
			attackTimer -= Time.deltaTime;
		
		if(attackTimer < 0) {
			attackTimer = 0;
			attacking = 0;
		}

		Anim.SetInteger("attacking", attacking);
	}
	#endregion
}

