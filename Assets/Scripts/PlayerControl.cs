using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	
	public float Speed;
	public Animator Anim;
	private Vector3 Movement;

	static AnimatorStateInfo currentBaseState;

	private float timerAtk = 0.4f;
	private float attackTimer ;
	private int attacking = 0;

	void Update() {
		setAttack();
	}

	void FixedUpdate()
	{
		// Set Player movement
		setMovement();
	}

	#region Movement
	void setMovement() {
		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		float moveVertical = Input.GetAxisRaw("Vertical");

		if ( moveHorizontal == 0 && moveVertical == 0) {
			Anim.SetBool("walking", false);
		} else {
			Anim.SetBool("walking", true);
			Anim.SetFloat("xAxis", moveHorizontal);
			Anim.SetFloat("yAxis", moveVertical);
		}

		Movement = new Vector2(moveHorizontal, moveVertical);
		rigidbody.velocity = Movement.normalized * Speed;
	}
	#endregion

	#region Attack
	void setAttack() {
		if (Input.GetButtonDown ("Fire1") && attackTimer == 0) {
			// Set the attack anim
			attacking = 1;
			attackTimer = timerAtk;
		}

		if(attackTimer > 0)
			attackTimer -= Time.deltaTime;
		
		if(attackTimer < 0) {
			attackTimer = 0;
			attacking = 0;
		}

		Anim.SetInteger("attacking", attacking);
	}
	#endregion
}

