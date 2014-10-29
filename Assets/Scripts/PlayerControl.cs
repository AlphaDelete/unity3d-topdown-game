using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	public float Speed;
	public Animator Anim;

	private Vector3 Movement;
	private float timerAtk = 0.4f;
	private float attackTimer ;
	private int attacking = 0;
	private bool picking = false;
	private Transform pickedItem = null;

	private bool walking = false;
	private float playerLookX;
	private float playerLookY; 

	// Access camera to shake
	public GameObject playerCamera;	
	private CameraMovement moveCamera;


	void Start() {
		moveCamera = playerCamera.GetComponent<CameraMovement>();
	}

	#region OnFrameUpdate
	// Update each frame
	void Update() {
		setAttack();
		setGrabThrow();
	}
	// Update each frame * deltaTime
	void FixedUpdate()
	{
		// Set Player movement
		setMovement();
	}
	#endregion

	#region TriggerEvents
	// Check colisions each frame
	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("E");
		Transform parentCollider = other.gameObject.transform.parent;
		if (parentCollider.tag == "Pickup" && picking == false && pickedItem == null) 
		{
			pickedItem = parentCollider;
		}
	}
	void OnTriggerExit2D() {
		if (picking == false) {
			pickedItem = null;
		}
	}
	#endregion

	#region Movement
	void setMovement() {
		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		float moveVertical = Input.GetAxisRaw("Vertical");

		if ( moveHorizontal == 0 && moveVertical == 0) {
			Anim.SetBool("walking", false);
			walking = false;
		} else {
			Anim.SetBool("walking", true);
			Anim.SetFloat("xAxis", moveHorizontal);
			Anim.SetFloat("yAxis", moveVertical);
			// Save Last movement
			playerLookX = moveHorizontal;
			playerLookY = moveVertical;
			walking = true;
		}

		Movement = new Vector2(moveHorizontal, moveVertical);
		rigidbody2D.velocity = Movement.normalized * Speed;
	}
	#endregion

	#region Attack/Damage
	void setAttack() {
		if (Input.GetButtonDown ("Fire1") && attackTimer == 0 && picking == false) {
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

	#region Grab/Throw
	void setGrabThrow() {
		if (pickedItem != null ) {
			if (Input.GetButtonDown ("Fire2") && attacking == 0 && picking == false) {
				setGrab();
			} else if (Input.GetButtonDown ("Fire2") && picking == true ) {
				setThrow();
			}
		}
		Anim.SetBool("picking", picking);
	}

	void setGrab() {
		picking = true;
		pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Pickup";
		pickedItem.rigidbody2D.isKinematic = true;
		pickedItem.transform.parent = this.transform; 
		pickedItem.transform.localPosition = new Vector3(0, 0.6f, 0);
	}

	void setThrow() {
		if (!walking) {
			pickedItem.transform.parent = null;
			pickedItem.rigidbody2D.isKinematic = true;
			pickedItem.rigidbody2D.position = 
					new Vector2(
						transform.position.x + pickedItem.collider2D.bounds.size.x * playerLookX, 
						transform.position.y + pickedItem.collider2D.bounds.size.y * playerLookY
					);
			pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Objects";
			picking = false;
		}

	}
	#endregion
}