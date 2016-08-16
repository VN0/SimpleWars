using UnityEngine;
using System.Collections;
using SmartMath;

public class ConvertingExample : MonoBehaviour {

	public double length;
	public double weight;
	public double temperature;
	public double time;

void Update() 
{

		length = Converting.Length.YardToDm (5);  //it returns 45.72(5 yard is 45.72 dm)
		weight = Converting.Weight.OuncesToPound (10); //it returns 0.625(10 ounces is 0.625 pounds)
		temperature = Converting.Temperature.CelsiusToKelvin (30); // it returns 303.15(30 celsius degree is 303.15 kelvin degree)
		time = Converting.Times.HourToMonth (21); // it returns 0.02876712 (so 21 hour is 0.02876712 month)
	}
}