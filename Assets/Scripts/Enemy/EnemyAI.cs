using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour {

	#region Properties
	// Enemy Class
	private Animator anim;
	private SpriteRenderer sprite;
	private Enemy enemyCtrl;
	// Enemy Healh
	private EnemyHealth enemyHealth;
	// Enemy IA 
	private Transform sight;
	private float rotateAngle = 0;
	#endregion

	#region Initialize
	void Start() {
		// Enemy Class
		sight = transform.Find("Sight");
		anim = GetComponent<Animator>();
		sprite = GetComponent<SpriteRenderer>();
		enemyCtrl = new Enemy(this, anim, sprite, 4F);
		// Enemy Health
		enemyHealth = GetComponent<EnemyHealth>();
	}
	#endregion

	#region OnFrameUpdate
	// Update each frame
	void Update() {
		// Get Enemy Life
		if (enemyHealth.GetLife() <= 0)
		{
			// Died
			callDestroy();
		}
	}

	// Use this for initialization
	void LateUpdate ()
	{
		enemyCtrl.SetLayerOrder();
	}
	#endregion

	#region TriggerEvents
	void OnCollisionEnter2D (Collision2D col)
	{
		// Check if it collided
		if(col.gameObject.tag == "Pickup")
		{
			// Take damage if is a jar and been thrown in the enemy
			if(col.gameObject.GetComponent< PickupItem >().beenThrow) {
				// DoDamage
				enemyHealth.TakeDamage(1);
			}
		}
	}
	#endregion
	
	#region Methods

	#region Destroy
	public void callDestroy()
	{
		// Start Destroy animation
		anim.SetBool("dying", true);
	}

	public void destroyObject()
	{
		// Destroy Object (This is set at the end of EnemyDie animation)
		Destroy(gameObject);
	}
	#endregion

	#region Sight
	void RotateSight(float x, float y) {
		if (enemyCtrl.walking) 
		{	
			// Rotate the Sight
			if (enemyCtrl.lookDirY <= 0 && enemyCtrl.lookDirX == 1 ) { // Right
				rotateAngle = 90;
				enemyCtrl.facingAngle = 0;
			} else if (enemyCtrl.lookDirY <= 0 && enemyCtrl.lookDirX == -1 ) { // Left
				rotateAngle = 270;
				enemyCtrl.facingAngle = 180;
			} else if (enemyCtrl.lookDirY == -1 && enemyCtrl.lookDirX == 0) { // Down
				rotateAngle = 0;
				enemyCtrl.facingAngle = 270;
			} else if (enemyCtrl.lookDirY == 1) { // Up
				rotateAngle = 180;
				enemyCtrl.facingAngle = 90;
			}
			
			sight.rotation = Quaternion.Euler(0, 0, rotateAngle);
		}
	}
	#endregion

	#endregion
}
