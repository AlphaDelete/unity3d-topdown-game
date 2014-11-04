using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	#region Properties
	private Animator anim;
	private SpriteRenderer sprite;
	private Humanoid player;

	private Transform sight;
	private float facingAngle = 0;
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
	// Check colisions each frame
	void OnTriggerEnter2D(Collider2D other) {
		Transform target = other.gameObject.transform;

		Vector3 v = transform.position - target.position;
		float angleOfTarget = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
		float anglediff = (facingAngle - angleOfTarget + 180) % 360 - 180;

		if (target.tag == "Pickup" && player.picking == false && player.pickedItem == null) 
		{
			player.pickedItem = target;
		}
		Debug.Log (target.name + "Angle: " + angleOfTarget);
	}
	void OnTriggerExit2D() {
		if (player.picking == false) {
			player.pickedItem = null;
		}
	}
	#endregion
	
	void RotateSight(float x, float y) {
		if (player.walking) 
		{	
			// Rotate the Sight
			if (player.lookDirY <= 0 && player.lookDirX == 1 ) {
				facingAngle = 90;
			} else if (player.lookDirY <= 0 && player.lookDirX == -1 ) {
				facingAngle = 270;
			} else if (player.lookDirY == -1 && player.lookDirX == 0) {
				facingAngle = 0;
			} else if (player.lookDirY == 1) {
				facingAngle = 180;
			}

			sight.rotation = Quaternion.Euler(0, 0, facingAngle);
		}
	}
}
