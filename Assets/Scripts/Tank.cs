using UnityEngine;

public class Tank : MonoBehaviour
{
    public float dryMass = 1;
    public float capacity = 5000;
    public float fuel;
	float fuelMass;
	Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		fuelMass = (rb.mass - dryMass) / fuel;
        if (fuel == 0)
            fuel = capacity;
	}
	public bool Consume (float amount) {
		if (fuel > 0) {
			fuel -= amount;
			rb.mass -= fuelMass * amount;
			return true;
		} else {
			return false;
		}
	}
}