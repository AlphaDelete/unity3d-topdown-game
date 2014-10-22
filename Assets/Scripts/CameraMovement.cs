using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform target;
	public float distanceDamping = 3.0F;

	void Awake ()
	{
		/*
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
		*/
	}

	// Update is called once per frame
	void LateUpdate () 
	{
		if (target)
		{
			float wantedHeight = target.position.y;
			float wantedDistance = target.position.x;

			float currentHeight = transform.position.y;
			float currentDistance = transform.position.x;

			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, distanceDamping * Time.deltaTime);
			currentDistance = Mathf.Lerp (currentDistance, wantedDistance, distanceDamping * Time.deltaTime);

			this.transform.position = new Vector3 (currentDistance, currentHeight, -10);
		}
		
	}
}