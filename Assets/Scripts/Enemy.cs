using UnityEngine;
using System.Collections;

public class Enemy : Humanoid
{
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

	public Enemy (MonoBehaviour Obj, Animator Anim, SpriteRenderer Sprt, float Spd) : base(Obj, Anim, Sprt, Spd)
	{
		// Vars specific for the enemy
		_attacking = 0;
		_timerAtk = 0.4f;
	}

	#region Attack
	public void SetAttack(bool ButtonDown) {
		if (ButtonDown && _attackTimer == 0 ) {
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

}

