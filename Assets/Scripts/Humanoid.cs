using UnityEngine;
using System.Collections;

public class Humanoid
{
	#region Properties
	private MonoBehaviour _Obj;
	// Animation
	private static Animator _Anim;
	private static SpriteRenderer _Sprite;
	// Movement
	private static float _Speed;
	public static float Speed
	{
		// Return the value stored in a field.
		get { return _Speed; }
		// Store the value in the field.
		set { _Speed = value; }
	}
	private Vector3 _Movement;
	private bool _Walking;
	private float _LookDirX;
	private float _LookDirY; 
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
		_Obj = Obj;
		_Anim = Anim;
		_Sprite = Sprt;
		_Speed = Spd;
		_Walking = false;
		_attacking = 0;
		_picking = false;
		_pickedItem = null;
		_timerAtk = 0.4f;
	}
	#endregion

	#region Movement
	public void setMovement(float moveHorizontal, float moveVertical) {
		if ( moveHorizontal == 0 && moveVertical == 0) {
			_Anim.SetBool("walking", false);
			_Walking = false;
		} else {
			_Anim.SetBool("walking", true);
			_Anim.SetFloat("xAxis", moveHorizontal);
			_Anim.SetFloat("yAxis", moveVertical);
			// Save Last movement
			_LookDirX = moveHorizontal;
			_LookDirY = moveVertical;
			_Walking = true;
		}
		
		_Movement = new Vector2(moveHorizontal, moveVertical);
		_Obj.rigidbody2D.velocity = _Movement.normalized * _Speed;
	}
	#endregion
	
	#region Attack/Damage
	public void setAttack(bool ButtonDown) {
		if (ButtonDown && _attackTimer == 0 && _picking == false) {
			// Set the attack anim
			_attacking = 1;
			_attackTimer = _timerAtk;
			// Set camera to shake (Second, Magnitude)
			if (CameraMovement.isShaking < 2) _Obj.StartCoroutine(CameraMovement.shakeCamera(0.3F, 0.015F));
		}
		
		if(_attackTimer > 0)
			_attackTimer -= Time.deltaTime;
		
		if(_attackTimer < 0) {
			_attackTimer = 0;
			_attacking = 0;
		}
		
		_Anim.SetInteger("attacking", _attacking);
	}
	#endregion
	
	#region Grab/Throw
	public void setGrabThrow(bool ButtonDown) {
		if (_pickedItem != null ) {
			if (ButtonDown && _attacking == 0 && _picking == false) {
				setGrab();
			} else if (ButtonDown && _picking == true ) {
				if (!_Walking) {
					setDrop();
				} else {
					setThrow();
				}
			}
		}
		_Anim.SetBool("picking", _picking);
	}
	
	private void setGrab() {
		_picking = true;
		_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Pickup";
		_pickedItem.rigidbody2D.isKinematic = true;
		_pickedItem.transform.parent = _Obj.transform; 
		_pickedItem.transform.localPosition = new Vector3(0, 0.6f, 0);
	}
	
	private void setDrop() {
		// Remove Item from player parent 
		_pickedItem.transform.parent = null;
		_pickedItem.rigidbody2D.isKinematic = true;
		
		//Set New Position to drop item
		float newY, newX;
		if (_LookDirY == -1) {
			// Looking up: Use Player Collider Size + Y position
			newY = (_Obj.transform.collider2D.bounds.size.y + 0.02f) * -1F;
		} else if (_LookDirY == 1) {
			// Looking down: Use item Collider Size + Y position
			newY = _pickedItem.collider2D.bounds.size.y + 0.02f;
		} else {
			// Adjust object to the player foot
			newY = (_Obj.transform.collider2D.bounds.size.y - _pickedItem.collider2D.bounds.size.y) * -1;
		}
		// Since X collider is in X = 0, the logis is simplier
		newX = (_pickedItem.collider2D.bounds.size.x + 0.02f) * _LookDirX;
		//Set New Position to drop item
		_pickedItem.rigidbody2D.position = 
			new Vector2(
				(_Obj.transform.position.x + newX), 
				(_Obj.transform.position.y + newY)
				);
		
		// Adjust Object Layer
		_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		// Adjust Object Order
		_pickedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)((10 * ((_Obj.transform.position.y + (_LookDirY*0.25f))  * -1)));
		// Set no item is picked
		_pickedItem = null;
		_picking = false;
	}
	
	private void setThrow() {
		// Remove Item from player parent 
		_pickedItem.transform.parent = null;
		_pickedItem.rigidbody2D.isKinematic = false;
		// Disable collider for a brief moment to not collide with player
		_pickedItem.collider2D.enabled = false;
		// Say to the item that it is been thrown
		_pickedItem.GetComponent< PickupItem >().beenThrow = true;
		//Set New Position to drop item
		_Obj.StartCoroutine( boostThrow(0.2f,_pickedItem) ); //Start the Coroutine called "Boost", and feed it the time we want it to boost us
		
		// Adjust Object Layer
		_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		// Adjust Object Order
		_pickedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)((10 * ((_Obj.transform.position.y + (_LookDirY*0.25f))  * -1)));
		
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
		float tLookY = _LookDirY, tLookX = _LookDirX;
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
	
	public void setLayerOrder() 
	{
		_Sprite.sortingOrder = (int)(10 * (_Obj.transform.position.y * -1));
	}
	#endregion
}

