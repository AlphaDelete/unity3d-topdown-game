using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform target;
	public float distanceDamping = 3.0F;
	public int isShaking = 0;
	
	// Update is called once per frame
	void LateUpdate () 
	{
		followCamera(target);
	}

	private void followCamera(Transform target)
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
	
	public IEnumerator shakeCamera(float duration, float magnitude) {
		isShaking++;
		float elapsed = 0.0f;
		
		while (elapsed < duration) {

			Vector3 originalCamPos = Camera.main.transform.position;
			elapsed += Time.deltaTime;          
			
			float percentComplete = elapsed / duration;         
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;
			
			Camera.main.transform.position = new Vector3(x + originalCamPos.x, y + originalCamPos.y, originalCamPos.z); 
			
			yield return null;
		}
		isShaking--;
	}
}