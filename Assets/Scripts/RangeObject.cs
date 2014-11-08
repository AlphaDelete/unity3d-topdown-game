using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeObject : IEquatable<RangeObject>
{
	public GameObject RangeGameObject { get; set; }
	public float RangeDistance { get; set; }
	public float RangeAngle { get; set; }
	public int RangeId { get; set; }

	public RangeObject (GameObject Obj)
	{
		RangeGameObject = Obj;
		RangeId = RangeGameObject.GetInstanceID();
	}
	
	public override string ToString()
	{
		return  "ID: " + RangeId + 
				"   Name: " + RangeGameObject +
				"   Distance: " + RangeDistance +
				"   Angle: " + RangeAngle;
	}

	public override int GetHashCode()
	{
		return RangeId;
	}

	public override bool Equals(object obj)
	{
		if (obj == null) return false;
		RangeObject objAsPart = obj as RangeObject;
		if (objAsPart == null) return false;
		else return Equals(objAsPart);
	}

	public bool Equals(RangeObject other)
	{
		if (other == null) return false;
		return (this.RangeId.Equals(other.RangeId));
	}

}

