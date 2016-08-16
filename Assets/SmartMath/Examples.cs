using UnityEngine;
using System.Collections;
using SmartMath;

public class Examples : MonoBehaviour {
	
	public float result1;
	public float result2;
	public float result3;
	public float result4;
	public float result5;
	public float result6;
	
	void Update () 
	{
		result1 = Math.HeronsFormula (5, 2, 6, 7);
		result2 = Physic.Frequency (6);
		result3 = Mechanic.ResultantForce (500, 700);
		result4 = TechnicalDrawing.Tolerance1 (5, 3);
		result5 = MaterialsScience.KFactor (50, 70);
		result6 = General.BMI (70f, 1.83f);

	}
}