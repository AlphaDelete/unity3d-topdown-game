using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	#region Properties
	private static GameObject _target;
	private static GameObject target
	{
		// Return the value stored in a field.
		get { return _target; }
		// Store the value in the field.
		set { _target = value; }
	}
	private static int _isShaking;
	public static int isShaking
	{
		// Return the value stored in a field.
		get { return _isShaking; }
		// Store the value in the field.
		set { _isShaking = value; }
	}
	private float _distanceDamping;
	//Here is a private reference only this class can access
	private static CameraMovement _instance;
	#endregion

	//This is the public reference that other classes will use
	public static CameraMovement instance
	{
		get
		{
			//If _instance hasn't been set yet, we grab it from the scene!
			//This will only happen the first time this reference is used.
			if(_instance == null)
				_instance = GameObject.FindObjectOfType<CameraMovement>();
			return _instance;
		}
	}

	#region Frames
	void Awake() 
	{
		_target = GameObject.FindGameObjectWithTag("Player");
		_distanceDamping = 3f;
		_isShaking = 0;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		// Set to follow the target
		followCamera(_target.transform);
	}
	#endregion

	#region Methods
	private void followCamera(Transform target)
	{
		if (target)
		{
			float wantedHeight = target.position.y;
			float wantedDistance = target.position.x;
			
			float currentHeight = transform.position.y;
			float currentDistance = transform.position.x;
			
			currentHeight = Mathf.Lerp (currentHeight, wantedHeight, _distanceDamping * Time.deltaTime);
			currentDistance = Mathf.Lerp (currentDistance, wantedDistance, _distanceDamping * Time.deltaTime);
			
			this.transform.position = new Vector3 (currentDistance, currentHeight, -10);
		}
	}
	
	public static IEnumerator shakeCamera(float duration, float magnitude) {
		float elapsed = 0.0f;
		while (elapsed < duration) {
			_isShaking++;
			Vector3 originalCamPos = Camera.main.transform.position;
			elapsed += Time.deltaTime;          
			
			float percentComplete = elapsed / duration;         
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;
			
			Camera.main.transform.position = new Vector3(x + originalCamPos.x, y + originalCamPos.y, originalCamPos.z); 
			_isShaking--;
			yield return null;
		}
	}
	#endregion
}