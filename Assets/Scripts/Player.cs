using UnityEngine;
using System.Collections;

public class Player : Humanoid
{
	//Picking
	private bool _picking;
	public bool picking 
	{
		// Return the value stored in a field.
		get { return _picking; }
		// Store the value in the field.
		set { _picking = value; }
	}
	private GameObject _pickedItem;
	public GameObject pickedItem
	{
		// Return the value stored in a field.
		get { return _pickedItem; }
		// Store the value in the field.
		set { _pickedItem = value; }
	}
	//Attack
	private float _timerAtk;
	private float _attackTimer ;
	private int _attacking;
	public int attacking 
	{
		// Return the value stored in a field.
		get { return _attacking; }
		// Store the value in the field.
		set { _attacking = value; }
	}

	public Player (MonoBehaviour Obj, Animator Anim, SpriteRenderer Sprt, float Spd) : base(Obj, Anim, Sprt, Spd)
	{
		// Vars specific for the player
		_picking = false;
		_pickedItem = null;
		_attacking = 0;
		_timerAtk = 0.4f;
	}

	#region Attack
	public void SetAttack(bool ButtonDown) {
		if (ButtonDown && _attackTimer == 0 && _picking == false) {
			// Set the attack anim
			_attacking = 1;
			_attackTimer = _timerAtk;
			// Search & Attack all range objects
			AttackRangeObj(0.25f);
			
		}
		
		if(_attackTimer > 0)
			_attackTimer -= Time.deltaTime;
		
		if(_attackTimer < 0) {
			_attackTimer = 0;
			_attacking = 0;
		}
		anim.SetInteger("attacking", _attacking);
	}

	public void AttackRangeObj(float attackRange){
		// Check all objects in the list
		if (ranges != null && ranges.Count != 0) {
			// Get the nearest angle object 
			for (int i = 0; i < ranges.Count; ++i) {
				if (ranges[i].RangeDistance < attackRange) {
					if (ranges[i].RangeGameObject.tag == "Pickup")
					{
						// Destroy Jar
						ranges[i].RangeGameObject.GetComponent< PickupItem >().callDestroy();
						// Set camera to shake (Second, Magnitude)
						if (CameraMovement.isShaking < 2) obj.StartCoroutine(CameraMovement.shakeCamera(0.3F, 0.015F));
					}
				}
			}
		}
	}
	#endregion

	#region GrabThrow
	public void SetGrabThrow(bool ButtonDown) {
		// Check of button input
		if (ButtonDown && _attacking == 0 && _picking == false) {
			SetGrab();
		} else if (ButtonDown && _picking == true ) {
			if (!walking) {
				SetDrop();
			} else {
				SetThrow();
			}
		}
		anim.SetBool("picking", _picking);
	}
	
	private void SetGrab() {
		// If there any itens in the target list
		if(ranges.Count > 0) 
		{
			RangeObject picking = GetMinAngleObj("Pickup");
			// Check if are some pickup itens in list
			_pickedItem = picking.RangeGameObject;
			if (_pickedItem != null && _pickedItem.tag == "Pickup") {
				// Set picking animation
				_picking = true;
				// Change picked item layer & configs to drag
				_pickedItem.gameObject.layer = 11;
				_pickedItem.rigidbody2D.isKinematic = true;
				_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Pickup";
				// Set player as parent of the item
				_pickedItem.transform.parent = obj.transform; 
				// Move item to the players head
				_pickedItem.transform.localPosition = new Vector3(0, 0.6f, 0);

			}
		}
	}
	
	private void SetDrop() {
		// Check if its near of some wall or object 
		float nearDistantObject = GetMinDistanceObj("") != null ? GetMinDistanceObj("").RangeDistance : 1;
		float nearAngleObject = GetMinDistanceObj("") != null ? GetMinDistanceObj("").RangeAngle : 0;
		string nearTagObject =  GetMinDistanceObj("") != null ? GetMinDistanceObj("").RangeGameObject.tag : "";
		bool canDrop = false;
		
		// Check if I can drop the item
		if (nearDistantObject > 0.25) {
			canDrop = true;
		} else if((nearAngleObject > 20 || nearAngleObject == 0) && nearTagObject == "Pickup") {
			canDrop = true;
		} else {
			canDrop = false;
		}
		
		if(canDrop) {
			// Remove Item from player parent 
			_pickedItem.transform.parent = null;
			_pickedItem.rigidbody2D.isKinematic = true;
			
			BoxCollider2D pick = (BoxCollider2D)_pickedItem.rigidbody2D.collider2D;
			BoxCollider2D user = (BoxCollider2D)obj.collider2D;
			
			// Get Collider Diference
			float colliderSizeX = (pick.center + new Vector2(pick.size.x * 0.5f, 0)).x + ((user.center * lookDirX) + new Vector2(user.size.x * 0.5f, 0)).x;
			float colliderSizeY = ((pick.center * (-lookDirY)) + new Vector2(0, pick.size.y * 0.5f)).y + ((user.center * lookDirY) + new Vector2(0, user.size.y * 0.5f)).y;
			//Set New Position to drop item
			float newY, newX;
			
			if (lookDirY == -1) {
				// Looking up: Use Player Collider Size + Y position
				newY = (colliderSizeY + 0.02f) * -1F;
			} else if (lookDirY == 1) {
				// Looking down: Use item Collider Size + Y position
				newY = colliderSizeY + 0.02f;
			} else {
				// Adjust object to the player foot
				newY = (user.bounds.size.y - pick.bounds.size.y) * -1;
			}
			// Since X collider is in X = 0, the logis is simplier
			newX = (colliderSizeX + 0.02f) * lookDirX;
			//Set New Position to drop item
			_pickedItem.rigidbody2D.position = 
				new Vector2(
					(obj.transform.position.x + newX), 
					(obj.transform.position.y + newY)
					);
			// Adjust Object Layer
			_pickedItem.gameObject.layer = 9;
			_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
			// Adjust Object Order
			_pickedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)((10 * ((obj.transform.position.y + (lookDirY*0.25f))  * -1)));
			// Set no item is picked
			_pickedItem = null;
			_picking = false;
		}
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
		obj.StartCoroutine( boostThrow(0.2f,_pickedItem.transform) ); //Start the Coroutine called "Boost", and feed it the time we want it to boost us
		
		// Adjust Object Layer
		_pickedItem.gameObject.layer = 9;
		_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		// Adjust Object Order
		_pickedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)((10 * ((obj.transform.position.y + (lookDirY*0.25f))  * -1)));
		//Remove it on the range objects list
		//ranges.Remove(new RangeObject(_pickedItem));
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
		float tLookY = lookDirY, tLookX = lookDirX;
		//we call this loop every frame while our custom boostDur is a higher value than the "time" variable in this coroutine
		while(boostDur > time) 
		{
			if (objectBoost) 
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
				
			}
			//go to next frame
			yield return 0; 
		}
		//set our rigidbody velocity 0 to stop
		objectBoost.rigidbody2D.velocity = new Vector2(0, 0);
	}
	#endregion

}

