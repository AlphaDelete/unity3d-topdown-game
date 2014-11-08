using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

	#region Properties
	private Animator anim;
	private SpriteRenderer sprite;
	private Humanoid player;

	private Transform sight;
	private float rotateAngle = 0;
	#endregion

	void Start() {
		sight = transform.Find("Sight");
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
		player = new Humanoid(this, anim, sprite, 4F);
	}

	#region OnFrameUpdate
	// Update each frame
	void Update() {
		player.SetAttack(
			Input.GetButtonDown ("Fire1")
		);
		player.SetGrabThrow(
			Input.GetButtonDown ("Fire2")
		);
		// Rotate Sight
		RotateSight(player.lookDirX, player.lookDirY);

		player.UpdateOnRange();
		player.GetMinAngleObj();
	}
	// Update each frame * deltaTime
	void FixedUpdate()
	{
		// Set Player movement
		player.SetMovement(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		);
	}
	// Use this for initialization
	void LateUpdate ()
	{
		player.SetLayerOrder();
	}
	#endregion

	#region TriggerEvents
	// Set up a list to keep track of targets

	// If a new enemy enters the trigger, add it to the list of targets
	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag("Pickup")) {
			GameObject go = other.gameObject;
			if(!player.targets.Contains(go)){
				player.targets.Add(go);
			}
		}
		// Test New Object
		GameObject range = other.gameObject;
		if(!player.ranges.Contains(new RangeObject(range))){
			player.ranges.Add(new RangeObject(range));
		}
	}

	// When an enemy exits the trigger, remove it from the list
	void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag("Pickup")) {
			GameObject go = other.gameObject;
			player.targets.Remove(go);
		}

		// Test New Object
		GameObject range = other.gameObject;
		player.ranges.Remove(new RangeObject(range));
	}
	#endregion
	
	void RotateSight(float x, float y) {
		if (player.walking) 
		{	
			// Rotate the Sight
			if (player.lookDirY <= 0 && player.lookDirX == 1 ) { // Right
				rotateAngle = 90;
				player.facingAngle = 0;
			} else if (player.lookDirY <= 0 && player.lookDirX == -1 ) { // Left
				rotateAngle = 270;
				player.facingAngle = 180;
			} else if (player.lookDirY == -1 && player.lookDirX == 0) { // Down
				rotateAngle = 0;
				player.facingAngle = 270;
			} else if (player.lookDirY == 1) { // Up
				rotateAngle = 180;
				player.facingAngle = 90;
			}

			sight.rotation = Quaternion.Euler(0, 0, rotateAngle);
		}
	}
}
