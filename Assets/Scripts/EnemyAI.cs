using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {

	#region Properties
	private Animator anim;
	private SpriteRenderer sprite;
	private Enemy enemy;
	
	private Transform sight;
	private float rotateAngle = 0;
	#endregion

	void Start() {
		sight = transform.Find("Sight");
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
		enemy = new Enemy(this, anim, sprite, 4F);
	}

	// Use this for initialization
	void LateUpdate ()
	{
		enemy.SetLayerOrder();
	}

	void OnCollisionEnter2D (Collision2D col)
	{
		// Check if it collided
		if(col.gameObject.tag == "Pickup")
		{
			// If is a jar and been thrown
			if(col.gameObject.GetComponent< PickupItem >().beenThrow) {
				callDestroy();
			}
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
		anim.SetBool("dying", true);
	}

	void RotateSight(float x, float y) {
		if (enemy.walking) 
		{	
			// Rotate the Sight
			if (enemy.lookDirY <= 0 && enemy.lookDirX == 1 ) { // Right
				rotateAngle = 90;
				enemy.facingAngle = 0;
			} else if (enemy.lookDirY <= 0 && enemy.lookDirX == -1 ) { // Left
				rotateAngle = 270;
				enemy.facingAngle = 180;
			} else if (enemy.lookDirY == -1 && enemy.lookDirX == 0) { // Down
				rotateAngle = 0;
				enemy.facingAngle = 270;
			} else if (enemy.lookDirY == 1) { // Up
				rotateAngle = 180;
				enemy.facingAngle = 90;
			}
			
			sight.rotation = Quaternion.Euler(0, 0, rotateAngle);
		}
	}
}
