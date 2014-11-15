using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Humanoid
{
	#region Properties
	private MonoBehaviour _obj;
	protected MonoBehaviour obj
	{
		// Return the value stored in a field.
		get { return _obj; }
		// Store the value in the field.
		set { _obj = value; }
	}
	// Animation
	private Animator _anim;
	protected Animator anim
	{
		// Return the value stored in a field.
		get { return _anim; }
		// Store the value in the field.
		set { _anim = value; }
	}
	private SpriteRenderer _sprite;
	// Sight
	private Transform _sightedItem;
	protected Transform sightedItem
	{
		// Return the value stored in a field.
		get { return _sightedItem; }
		// Store the value in the field.
		set { _sightedItem = value; }
	}
	// Movement
	private float _speed;
	public float speed
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
	
	// facingAngle is set in Control / IA Script
	public float facingAngle = 0;
	// List for objects in range
	public List<RangeObject> ranges = new List<RangeObject>();
	#endregion

	#region Initialize
	//This is the public reference that other classes will use
	public Humanoid (MonoBehaviour Obj,  Animator Anim, SpriteRenderer Sprt, float Spd)
	{
		_obj = Obj;
		_anim = Anim;
		_sprite = Sprt;
		_speed = Spd;
		_walking = false;
		_sightedItem = null;

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

	protected RangeObject GetMinAngleObj(string tag){
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

	protected RangeObject GetMinDistanceObj(string tag){
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

	protected float SetAngle(Transform range){
		float angle = 0;
		// Center of the “looker” is the origin (0, 0).
		Vector2 v = new Vector2(range.position.x - _obj.transform.position.x, range.position.y - _obj.transform.position.y);
		// For each nearby object, use atan2 to compute the angle from the looker “to” this object.
		float angleOfTarget = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
		// Finally get the angle between the two objects
		angle =  Mathf.Abs((facingAngle - angleOfTarget + 180) % 360 - 180);

		return angle;
	}

	protected float SetDistance(GameObject rangeObject){
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

