using UnityEngine;
using System.Collections;

public class Humanoid
{
	#region Properties
	private MonoBehaviour _obj;
	// Animation
	private static Animator _anim;
	private static SpriteRenderer _sprite;
	// Movement
	private static float _speed;
	public static float speed
	{
		// Return the value stored in a field.
		get { return _speed; }
		// Store the value in the field.
		set { _speed = value; }
	}
	private Vector3 _movement;
	private bool _walking;
	public bool walking
	{
		// Return the value stored in a field.
		get { return _walking; }
		// Store the value in the field.
		set { _walking = value; }
	}
	private float _lookDirX;
	public float lookDirX
	{
		// Return the value stored in a field.
		get { return _lookDirX; }
		// Store the value in the field.
		set { _lookDirX = value; }
	}
	private float _lookDirY; 
	public float lookDirY
	{
		// Return the value stored in a field.
		get { return _lookDirY; }
		// Store the value in the field.
		set { _lookDirY = value; }
	}
	//Attack
	private float _timerAtk;
	private float _attackTimer ;
	private int _attacking;
	//Picking
	private bool _picking;
	public bool picking 
	{
		// Return the value stored in a field.
		get { return _picking; }
		// Store the value in the field.
		set { _picking = value; }
	}
	private Transform _pickedItem;
	public Transform pickedItem
	{
		// Return the value stored in a field.
		get { return _pickedItem; }
		// Store the value in the field.
		set { _pickedItem = value; }
	}
	#endregion

	#region Initial Instance
	// Needed instance
	protected static Humanoid instance;
	//This is the public reference that other classes will use
	public Humanoid (MonoBehaviour Obj,  Animator Anim, SpriteRenderer Sprt, float Spd)
	{
		_obj = Obj;
		_anim = Anim;
		_sprite = Sprt;
		_speed = Spd;
		_walking = false;
		_attacking = 0;
		_picking = false;
		_pickedItem = null;
		_timerAtk = 0.4f;
	}
	#endregion

	#region Movement
	public void SetMovement(float moveHorizontal, float moveVertical) {
		if ( moveHorizontal == 0 && moveVertical == 0) {
			_anim.SetBool("walking", false);
			_walking = false;
		} else {
			_anim.SetBool("walking", true);
			_anim.SetFloat("xAxis", moveHorizontal);
			_anim.SetFloat("yAxis", moveVertical);
			// Save Last movement
			_lookDirX = moveHorizontal;
			_lookDirY = moveVertical;
			_walking = true;
		}
		
		_movement = new Vector2(moveHorizontal, moveVertical);
		_obj.rigidbody2D.velocity = _movement.normalized * _speed;
	}
	#endregion
	
	#region Attack/Damage
	public void SetAttack(bool ButtonDown) {
		if (ButtonDown && _attackTimer == 0 && _picking == false) {
			// Set the attack anim
			_attacking = 1;
			_attackTimer = _timerAtk;
			// Set camera to shake (Second, Magnitude)
			if (CameraMovement.isShaking < 2) _obj.StartCoroutine(CameraMovement.shakeCamera(0.3F, 0.015F));
		}
		
		if(_attackTimer > 0)
			_attackTimer -= Time.deltaTime;
		
		if(_attackTimer < 0) {
			_attackTimer = 0;
			_attacking = 0;
		}
		
		_anim.SetInteger("attacking", _attacking);
	}
	#endregion
	
	#region Grab/Throw
	public void SetGrabThrow(bool ButtonDown) {
		if (_pickedItem != null ) {
			if (ButtonDown && _attacking == 0 && _picking == false) {
				SetGrab();
			} else if (ButtonDown && _picking == true ) {
				if (!_walking) {
					SetDrop();
				} else {
					SetThrow();
				}
			}
		}
		_anim.SetBool("picking", _picking);
	}
	
	private void SetGrab() {
		_picking = true;
		_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Pickup";
		_pickedItem.rigidbody2D.isKinematic = true;
		_pickedItem.transform.parent = _obj.transform; 
		_pickedItem.transform.localPosition = new Vector3(0, 0.6f, 0);
	}
	
	private void SetDrop() {
		// Remove Item from player parent 
		_pickedItem.transform.parent = null;
		_pickedItem.rigidbody2D.isKinematic = true;
		
		//Set New Position to drop item
		float newY, newX;
		if (_lookDirY == -1) {
			// Looking up: Use Player Collider Size + Y position
			newY = (_obj.transform.collider2D.bounds.size.y + 0.02f) * -1F;
		} else if (_lookDirY == 1) {
			// Looking down: Use item Collider Size + Y position
			newY = _pickedItem.collider2D.bounds.size.y + 0.02f;
		} else {
			// Adjust object to the player foot
			newY = (_obj.transform.collider2D.bounds.size.y - _pickedItem.collider2D.bounds.size.y) * -1;
		}
		// Since X collider is in X = 0, the logis is simplier
		newX = (_pickedItem.collider2D.bounds.size.x + 0.02f) * _lookDirX;
		//Set New Position to drop item
		_pickedItem.rigidbody2D.position = 
			new Vector2(
				(_obj.transform.position.x + newX), 
				(_obj.transform.position.y + newY)
				);
		
		// Adjust Object Layer
		_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		// Adjust Object Order
		_pickedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)((10 * ((_obj.transform.position.y + (_lookDirY*0.25f))  * -1)));
		// Set no item is picked
		_pickedItem = null;
		_picking = false;
	}
	
	private void SetThrow() {
		// Remove Item from player parent 
		_pickedItem.transform.parent = null;
		_pickedItem.rigidbody2D.isKinematic = false;
		// Disable collider for a brief moment to not collide with player
		_pickedItem.collider2D.enabled = false;
		// Say to the item that it is been thrown
		_pickedItem.GetComponent< PickupItem >().beenThrow = true;
		//Set New Position to drop item
		_obj.StartCoroutine( boostThrow(0.2f,_pickedItem) ); //Start the Coroutine called "Boost", and feed it the time we want it to boost us
		
		// Adjust Object Layer
		_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		// Adjust Object Order
		_pickedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)((10 * ((_obj.transform.position.y + (_lookDirY*0.25f))  * -1)));
		
		// Set no item is picked
		_pickedItem = null;
		_picking = false;
	}
	
	private IEnumerator boostThrow(float boostDur, Transform objectBoost) 
	{
		//create float to store the time this coroutine is operating
		float time = 0; 
		//Set New Position to drop item
		float newY = 0, newX = 0;
		// Look player look direction before throw
		float tLookY = _lookDirY, tLookX = _lookDirX;
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
	
	public void SetLayerOrder() 
	{
		_sprite.sortingOrder = (int)(10 * (_obj.transform.position.y * -1));
	}
	#endregion
}

