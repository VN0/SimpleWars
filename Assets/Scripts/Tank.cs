using UnityEngine;

public class Tank : MonoBehaviour
{
	public float dryMass = 1;
	public float fuel = 5000;
	float fuelMass;
	Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		fuelMass = (rb.mass - dryMass) / fuel;
	}
	public bool GetFuel (float amount) {
		if (fuel > 0) {
			fuel -= amount;
			rb.mass -= fuelMass * amount;
			return true;
		} else {
			return false;
		}
	}
}