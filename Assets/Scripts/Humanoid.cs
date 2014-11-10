using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private GameObject _pickedItem;
	public GameObject pickedItem
	{
		// Return the value stored in a field.
		get { return _pickedItem; }
		// Store the value in the field.
		set { _pickedItem = value; }
	}
	private Transform _sightedItem;
	public Transform sightedItem
	{
		// Return the value stored in a field.
		get { return _sightedItem; }
		// Store the value in the field.
		set { _sightedItem = value; }
	}
	private RaycastHit2D _hit;
	public float facingAngle = 0;
	public List<RangeObject> ranges = new List<RangeObject>();
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
		_sightedItem = null;
		_timerAtk = 0.4f;
	}
	#endregion

	#region Methods

	#region Order
	public void SetLayerOrder() 
	{
		_sprite.sortingOrder = (int)(10 * (_obj.transform.position.y * -1));
	}
	#endregion

	#region Moviment
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
		_anim.SetInteger("attacking", _attacking);
	}
	#endregion

	#region GrabThrow
	public void SetGrabThrow(bool ButtonDown) {
		// Check of button input
		if (ButtonDown && _attacking == 0 && _picking == false) {
			SetGrab();
		} else if (ButtonDown && _picking == true ) {
			if (!_walking) {
				SetDrop();
			} else {
				SetThrow();
			}
		}
		_anim.SetBool("picking", _picking);
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
				_pickedItem.transform.parent = _obj.transform; 
				// Move item to the players head
				_pickedItem.transform.localPosition = new Vector3(0, 0.6f, 0);
				//Remove it on the range objects list
				//ranges.Remove(new RangeObject(_pickedItem));
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
			BoxCollider2D user = (BoxCollider2D)_obj.collider2D;

			// Get Collider Diference
			float colliderSizeX = (pick.center + new Vector2(pick.size.x * 0.5f, 0)).x + ((user.center * _lookDirX) + new Vector2(user.size.x * 0.5f, 0)).x;
			float colliderSizeY = ((pick.center * (-_lookDirY)) + new Vector2(0, pick.size.y * 0.5f)).y + ((user.center * _lookDirY) + new Vector2(0, user.size.y * 0.5f)).y;
			//Set New Position to drop item
			float newY, newX;

			if (_lookDirY == -1) {
				// Looking up: Use Player Collider Size + Y position
				newY = (colliderSizeY + 0.02f) * -1F;
			} else if (_lookDirY == 1) {
				// Looking down: Use item Collider Size + Y position
				newY = colliderSizeY + 0.02f;
			} else {
				// Adjust object to the player foot
				newY = (user.bounds.size.y - pick.bounds.size.y) * -1;
			}
			// Since X collider is in X = 0, the logis is simplier
			newX = (colliderSizeX + 0.02f) * _lookDirX;
			//Set New Position to drop item
			_pickedItem.rigidbody2D.position = 
				new Vector2(
					(_obj.transform.position.x + newX), 
					(_obj.transform.position.y + newY)
					);
			// Adjust Object Layer
			_pickedItem.gameObject.layer = 9;
			_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
			// Adjust Object Order
			_pickedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)((10 * ((_obj.transform.position.y + (_lookDirY*0.25f))  * -1)));
			//Put it on the range objects list
			//ranges.Add(new RangeObject(_pickedItem));
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
		_obj.StartCoroutine( boostThrow(0.2f,_pickedItem.transform) ); //Start the Coroutine called "Boost", and feed it the time we want it to boost us
		
		// Adjust Object Layer
		_pickedItem.gameObject.layer = 9;
		_pickedItem.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
		// Adjust Object Order
		_pickedItem.GetComponent<SpriteRenderer>().sortingOrder = (int)((10 * ((_obj.transform.position.y + (_lookDirY*0.25f))  * -1)));
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
		float tLookY = _lookDirY, tLookX = _lookDirX;
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

	#region RangeObjects
	
	public void UpdateOnRange() {
		if (ranges != null && ranges.Count != 0) {
			// Garbage remove in the list
			ranges.RemoveAll(item => item.RangeGameObject == null);
			// Foreach collided object in list
			foreach (RangeObject range in ranges)
			{
				if (range.RangeGameObject != null) {
					
					// Calculate the difference between the looker’s facing angle and the object’s angle.
					range.RangeAngle = SetAngle(range.RangeGameObject.transform);
					// Calculate the difference between the looker’s facing distance and the object’s distance.
					range.RangeDistance = SetDistance(range.RangeGameObject);
				}
			}
		}

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
						if (CameraMovement.isShaking < 2) _obj.StartCoroutine(CameraMovement.shakeCamera(0.3F, 0.015F));
					}	
				}
			}
		}

	}

	public RangeObject GetMinAngleObj(string tag){
		// Check all objects in the list
		RangeObject objReturn = null;
		if (ranges != null && ranges.Count != 0) {
			float minAngle = ranges[0].RangeAngle;
			int minIndex = 0;
			// Get the nearest angle object 
			for (int i = 0; i < ranges.Count; ++i) {
				if (ranges[i].RangeGameObject.tag == tag || tag == "")
				{
					if (ranges[i].RangeAngle < minAngle) {
						minAngle = ranges[i].RangeAngle;
						minIndex = i;
					}
				}
			}
			objReturn = ranges[minIndex];
		}
		// Return the object
		return objReturn;
	}

	private RangeObject GetMinDistanceObj(string tag){
		// Check all objects in the list
		RangeObject objReturn = null;
		if (ranges != null && ranges.Count != 0) {
			float minDistance = ranges[0].RangeDistance;
			int minIndex = 0;
			// Get the nearest object 
			for (int i = 0; i < ranges.Count; ++i) {
				if (ranges[i].RangeGameObject.tag == tag || tag == "")
				{
					if (ranges[i].RangeDistance < minDistance) {
						minDistance = ranges[i].RangeDistance;
						minIndex = i;
					}
				}
			}
			objReturn = ranges[minIndex];
		}
		// Return the distance
		return objReturn;
	}

	private float SetAngle(Transform range){
		float angle = 0;
		// Center of the “looker” is the origin (0, 0).
		Vector2 v = new Vector2(range.position.x - _obj.transform.position.x, range.position.y - _obj.transform.position.y);
		// For each nearby object, use atan2 to compute the angle from the looker “to” this object.
		float angleOfTarget = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
		angle =  Mathf.Abs((facingAngle - angleOfTarget + 180) % 360 - 180);

		return angle;
	}

	private float SetDistance(GameObject rangeObject){
		float distance = 0;
		BoxCollider2D range = (BoxCollider2D)rangeObject.collider2D;
		BoxCollider2D user = (BoxCollider2D)_obj.collider2D;

		// Get Collider Diference
		float colliderSizeX = (range.center + new Vector2(range.size.x * 0.5f, 0)).x + ((user.center * _lookDirX) + new Vector2(user.size.x * 0.5f, 0)).x;
		float colliderSizeY = ((range.center * -_lookDirY) + new Vector2(0, range.size.y * 0.5f)).y + ((user.center * _lookDirY) + new Vector2(0, user.size.y * 0.5f)).y;

		// Check distance
		if (_lookDirY <= 0 && _lookDirX == 1 ) { // Right
			distance = Mathf.Abs(Mathf.Abs(range.transform.position.x - _obj.collider2D.transform.position.x) - colliderSizeX);
		} else if (_lookDirY <= 0 && _lookDirX == -1 ) { // Left
			distance = Mathf.Abs(Mathf.Abs(range.transform.position.x - _obj.collider2D.transform.position.x) - colliderSizeX);
		} else if (_lookDirY == -1 && _lookDirX == 0) { // Down
			distance = Mathf.Abs(Mathf.Abs(range.transform.position.y - _obj.collider2D.transform.position.y) - colliderSizeY);
		} else if (_lookDirY == 1) { // Up
			distance = Mathf.Abs(Mathf.Abs(range.transform.position.y - _obj.collider2D.transform.position.y) - colliderSizeY);
		}
		return distance;
	}


	#endregion


	#endregion
}

