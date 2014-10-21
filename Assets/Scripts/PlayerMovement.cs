using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float Speed;
	public Animator Anim;
	private Vector3 Movement;

	void Update()
	{
		// Set Player movement
		SetMovement();
	}

	#region SetMovement
	void SetMovement() {
		float moveHorizontal = Input.GetAxisRaw("Horizontal");
		float moveVertical = Input.GetAxisRaw("Vertical");
		
		Movement = new Vector2(moveHorizontal, moveVertical);
		Movement.Normalize();
		rigidbody.velocity = Movement * Speed;
	}
	#endregion

	#region setAnimation
	void setAnimation(int animNumber) 
	{
		if(animNumber == Anim.GetInteger("animation") || animNumber == -Anim.GetInteger("animation"))
		{
			Anim.SetInteger("animation", animNumber);
		} else {
			Anim.SetInteger("animation", -animNumber);
		}
	}
	#endregion
}
