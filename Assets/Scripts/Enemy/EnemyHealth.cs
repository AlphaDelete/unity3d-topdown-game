using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

	#region Properties
	//Health
	private float spawnLife;
	private Health enemyHealth;
	#endregion

	#region Initialize
	// Use this for initialization
	void Start () {
		spawnLife = 2;
		enemyHealth = new Health(spawnLife);
	}
	#endregion

	#region OnFrameUpdate
	// Update is called once per frame
	void Update () {
	}
	#endregion

	#region Methods

	public void TakeDamage (float Damage) {
		enemyHealth.life -= Damage;
	}

	public float GetLife () {
		return enemyHealth.life;
	}

	#endregion
}
