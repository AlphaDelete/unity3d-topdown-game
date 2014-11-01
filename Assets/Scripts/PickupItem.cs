using UnityEngine;
using System.Collections;

public class PickupItem : MonoBehaviour {

	public bool beenThrow = false;

	void FixedUpdate()
	{
		// Set Player movement
		if(beenThrow && rigidbody2D.velocity.sqrMagnitude == 0f)
			StartCoroutine(Die());

	}

	void OnCollisionEnter2D (Collision2D col)
	{
		Debug.Log(col.gameObject.name);
		if(col.gameObject.name != "Player")
		{
			StartCoroutine(Die());
		}
	}

	private IEnumerator Die()
	{
		//PlayAnimation(GlobalSettings.animDeath1, WrapeMode.ClampForever);
		//yield return new WaitForSeconds(gameObject, GlobalSettings.animDeath1.length);
		yield return new WaitForSeconds (0.167f);
		Destroy(gameObject);
	}
}
