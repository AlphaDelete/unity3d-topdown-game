using UnityEngine;
using System.Collections;

public class Health
{
	#region Properties
	//Health
	private float _life;
	public float life 
	{
		// Return the value stored in a field.
		get { return _life; }
		// Store the value in the field.
		set { _life = value; }
	}
	#endregion

	#region Initialize
	public Health (float Life)
	{
		_life = Life;
	}
	#endregion
}