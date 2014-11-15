using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour {

	// Animation
	public Animator Anim;
	public bool beenThrow = false;
	public bool destroy = false;

	void Start() {
		//Adjust pot layer
		GetComponent<SpriteRenderer>().sortingOrder = (int)(10 * (transform.position.y * -1));
	}

	void FixedUpdate()
	{
		// Check if the pot is been throw
		if(beenThrow && rigidbody2D.velocity.sqrMagnitude == 0f)
			callDestroy();
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		// Check if it collided
		if(col.gameObject.tag == "Pickup")
		{
			callDestroy();
		}
	}

	public void destroyObject()
	{
		// Destroy Object (This is set at the end of PotDestroy animation)
		Destroy(gameObject);
	}


	public void callDestroy()
	{
		// Shake screen
		StartCoroutine(CameraMovement.shakeCamera(0.3F, 0.015F));
		// Start Destroy animation
		Anim.SetBool("destroy", true);
	}

}
