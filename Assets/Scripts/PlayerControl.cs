using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

	#region Properties
	private Animator Anim;
	private SpriteRenderer Sprite;
	private Humanoid Player;
	#endregion

	void Start() {
		Anim = GetComponent<Animator>();
		Sprite = GetComponent<SpriteRenderer>();
		Player = new Humanoid(this, Anim, Sprite, 4F);
	}

	#region OnFrameUpdate
	// Update each frame
	void Update() {
		Player.setAttack(
			Input.GetButtonDown ("Fire1")
		);
		Player.setGrabThrow(
			Input.GetButtonDown ("Fire2")
		);
	}
	// Update each frame * deltaTime
	void FixedUpdate()
	{
		// Set Player movement
		Player.setMovement(
			Input.GetAxisRaw("Horizontal"),
			Input.GetAxisRaw("Vertical")
		);
	}
	// Use this for initialization
	void LateUpdate ()
	{
		Player.setLayerOrder();
	}
	#endregion

	#region TriggerEvents
	// Check colisions each frame
	void OnTriggerEnter2D(Collider2D other) {
		Transform parentCollider = other.gameObject.transform.parent;
		if (parentCollider.tag == "Pickup" && Player.picking == false && Player.pickedItem == null) 
		{
			Player.pickedItem = parentCollider;
		}
	}
	void OnTriggerExit2D() {
		if (Player.picking == false) {
			Player.pickedItem = null;
		}
	}
	#endregion
}
