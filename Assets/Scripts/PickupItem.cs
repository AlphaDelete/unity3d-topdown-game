﻿using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour {

	// Animation
	public Animator Anim;
	public bool beenThrow = false;
	public bool destroy = false;
	// Camera
	private CameraMovement moveCamera;

	void Start() {
		//Adjust pot layer
		GetComponent<SpriteRenderer>().sortingOrder = (int)(10 * (transform.position.y * -1));
		// Find camera script
		moveCamera = Camera.main.GetComponent<CameraMovement>();
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
		if(col.gameObject.name != "Player")
		{
			callDestroy();
		}
	}

	public void destroyObject()
	{
		// Destroy Object (This is set at the end of PotDestroy animation)
		Destroy(gameObject);
	}

	private void callDestroy()
	{
		// Shake screen
		StartCoroutine(moveCamera.shakeCamera(0.3F, 0.015F));
		// Start Destroy animation
		Anim.SetBool("destroy", true);
	}

}
