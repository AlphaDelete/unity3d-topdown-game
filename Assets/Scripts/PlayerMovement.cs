using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float Speed;
	public Animator Anim;
	private Vector3 Movement;

	void FixedUpdate()
	{
		// Set Player movement
		SetMovement();

	}

	#region SetMovement
	void SetMovement() {
		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		float moveVertical = Input.GetAxisRaw("Vertical");

		setAnimation(moveHorizontal, moveVertical);

		Movement = new Vector2(moveHorizontal, moveVertical);
		rigidbody.velocity = Movement.normalized * Speed;
	}
	#endregion

	#region setAnimation
	void setAnimation(float xAxis, float yAxis) 
	{
		if ( xAxis == 0 && yAxis == 0) {
			Anim.SetBool("walking", false);
		}
		else {
			Anim.SetBool("walking", true);
			Anim.SetFloat("xAxis", xAxis);
			Anim.SetFloat("yAxis", yAxis);
		}
	}
	#endregion
}
