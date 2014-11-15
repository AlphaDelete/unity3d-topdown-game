using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

	#region Properties
	// Player CLass
	private Animator anim;
	private SpriteRenderer sprite;
	private Player playerCtrl;
	// Player Control
	private Transform sight;
	private float rotateAngle = 0;
	#endregion

	#region Initialize
	void Start() {
		sight = transform.Find("Sight");
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
		playerCtrl = new Player(this, anim, sprite, 4F);
	}
	#endregion

	#region OnFrameUpdate
	// Update each frame
	void Update() {
		playerCtrl.SetAttack(
			Input.GetButtonDown ("Fire1")
		);
		playerCtrl.SetGrabThrow(
			Input.GetButtonDown ("Fire2")
		);
		// Rotate Sight
		RotateSight(playerCtrl.lookDirX, playerCtrl.lookDirY);

		playerCtrl.UpdateOnRange();
	}
	// Update each frame * deltaTime
	void FixedUpdate()
	{
		// Set Player movement
		playerCtrl.SetMovement(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		);
	}
	// Use this for initialization
	void LateUpdate ()
	{
		playerCtrl.SetLayerOrder();
	}
	#endregion

	#region TriggerEvents
	// Set up a list to keep track of targets
	// If a new enemy enters the trigger, add it to the list of targets
	void OnTriggerEnter2D(Collider2D other){
		// Handle Objects List
		GameObject range = other.gameObject;
		if(other.gameObject.tag != "Sight") {
			if(!playerCtrl.ranges.Contains(new RangeObject(range))){
				playerCtrl.ranges.Add(new RangeObject(range));
			}
		}
	}

	// When an enemy exits the trigger, remove it from the list
	void OnTriggerExit2D(Collider2D other) {
		// Handle Objects List
		if(other.gameObject.tag != "Sight") {
			GameObject range = other.gameObject;
			playerCtrl.ranges.Remove(new RangeObject(range));
		}
	}
	#endregion

	#region Methods

	#region Sight
	void RotateSight(float x, float y) {
		if (playerCtrl.walking) 
		{	
			// Rotate the Sight
			if (playerCtrl.lookDirY <= 0 && playerCtrl.lookDirX == 1 ) { // Right
				rotateAngle = 90;
				playerCtrl.facingAngle = 0;
			} else if (playerCtrl.lookDirY <= 0 && playerCtrl.lookDirX == -1 ) { // Left
				rotateAngle = 270;
				playerCtrl.facingAngle = 180;
			} else if (playerCtrl.lookDirY == -1 && playerCtrl.lookDirX == 0) { // Down
				rotateAngle = 0;
				playerCtrl.facingAngle = 270;
			} else if (playerCtrl.lookDirY == 1) { // Up
				rotateAngle = 180;
				playerCtrl.facingAngle = 90;
			}

			sight.rotation = Quaternion.Euler(0, 0, rotateAngle);
		}
	}
	#endregion

	#endregion
}
