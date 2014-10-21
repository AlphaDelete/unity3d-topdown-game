using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform target;
	public float distanceDamping = 3.0F;
	public float heightDamping = 1.0F;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void LateUpdate () {
		if (!target)
			return;
		
		float wantedHeight = target.position.y;
		float wantedDistance = target.position.x;

		float currentHeight = transform.position.y;
		float currentDistance = transform.position.x;
		
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		currentDistance = Mathf.Lerp (currentDistance, wantedDistance, distanceDamping * Time.deltaTime);
		
		this.transform.position = new Vector3 (currentDistance, currentHeight, -10f);
	}
}