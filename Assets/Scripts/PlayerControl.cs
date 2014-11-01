using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	#region Proprieties
	// Animation
	public Animator Anim;
	private SpriteRenderer sprite;

	// Movement
	public float Speed;
	private Vector3 Movement;
	private bool walking = false;
	private float playerLookX;
	private float playerLookY; 
	//Attack
	private float timerAtk = 0.4f;
	private float attackTimer ;
	private int attacking = 0;
	//Picking
	private bool picking = false;
	private Transform pickedItem = null;

	// Camera
	public GameObject playerCamera;	
	private CameraMovement moveCamera;
	#endregion

	void Start() {
		sprite = GetComponent<SpriteRenderer>();
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
	void LateUpdate () {
		//Adjust player layer
		sprite.sortingOrder = (int)(10 * (transform.position.y * -1));
	}
	#endregion

	#region TriggerEvents
	// Check colisions each frame
	void OnTriggerEnter2D(Collider2D other) {
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
				if (!walking) {
					setDrop();
				} else {
					setThrow();
				}
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

	void setDrop() {
		// Remove Item from player parent 
		pickedItem.transform.parent = null;
		pickedItem.rigidbody2D.isKinematic = true;

		//Set New Position to drop item
		float newY, newX;
		if (playerLookY == -1) {
			// Looking up: Use Player Collider Size + Y position
			newY = (transform.collider2D.bounds.size.y + 0.02f) * -1F;
		} else if (playerLookY == 1) {
			// Looking down: Use item Collider Size + Y position
			newY = pickedItem.collider2D.bounds.size.y + 0.02f;
		} else {
			// Adjust object to the player foot
			newY = (transform.collider2D.bounds.size.y - pickedItem.collider2D.bounds.size.y) * -1;
		}
		// Since X collider is in X = 0, the logis is simplier
		newX = (pickedItem.collider2D.bounds.size.x + 0.02f) * playerLookX;
		//Set New Position to drop item
		pickedItem.rigidbody2D.position = 
			new Vector2(
				(transform.position.x + newX), 
				(transform.position.y + newY)
			);

		// Adjust Object Layer
		pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		// Adjust Object Order
		pickedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)((10 * ((transform.position.y + (playerLookY*0.25f))  * -1)));
		// Set no item is picked
		pickedItem = null;
		picking = false;
	}

	void setThrow() {
		// Remove Item from player parent 
		pickedItem.transform.parent = null;
		pickedItem.rigidbody2D.isKinematic = false;
		// Disable collider for a brief moment to not collide with player
		pickedItem.collider2D.enabled = false;
		// Say to the item that it is been thrown
		pickedItem.GetComponent< PickupItem >().beenThrow = true;
		//Set New Position to drop item
		StartCoroutine( boostThrow(0.2f,pickedItem) ); //Start the Coroutine called "Boost", and feed it the time we want it to boost us

		// Adjust Object Layer
		pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		// Adjust Object Order
		pickedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)((10 * ((transform.position.y + (playerLookY*0.25f))  * -1)));

		// Set no item is picked
		pickedItem = null;
		picking = false;
	}

	IEnumerator boostThrow(float boostDur, Transform objectBoost) 
	{
		//create float to store the time this coroutine is operating
		float time = 0; 
		//Set New Position to drop item
		float newY = 0, newX = 0;
		// Look player look direction before throw
		float tLookY = playerLookY, tLookX = playerLookX;
		//we call this loop every frame while our custom boostDur is a higher value than the "time" variable in this coroutine
		while(boostDur > time) 
		{
			//Increase our "time" variable by the amount of time that it has been since the last update
			time += Time.deltaTime; 
			// Check player direction 
			if (tLookY == -1 && tLookX == 0) {
				newY = -20f; newX = 0;
			} else if (tLookY == 1 && tLookX == 0) {
				newY = 20f; newX = 0;
			} else if (tLookY == 0 && tLookX == -1) {
				newY = -3.5f; newX = -20;
			} else if (tLookY == 0 && tLookX == 1) {
				newY = -3.5f; newX = 20;
			} else {
				newY = 15f * tLookY; 
				newX = 15f * tLookX;
			}
			//set our rigidbody velocity to a custom velocity every frame, so that we get a steady boost direction
			objectBoost.rigidbody2D.velocity = new Vector2(newX, newY);
			// Enable collider after item is far from player
			objectBoost.collider2D.enabled = (time > 0.06f) ? true : false;
			//go to next frame
			yield return 0; 
		}
		//set our rigidbody velocity 0 to stop
		objectBoost.rigidbody2D.velocity = new Vector2(0, 0);
	}
	#endregion
}
