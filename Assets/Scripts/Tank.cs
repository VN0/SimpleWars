using UnityEngine;

public class Tank : MonoBehaviour
{
    public float dryMass = 1;
    public float capacity = 5000;
    [HideInInspector]
    public float fuel;
	float fuelMass;
	Rigidbody2D rb;

	void Awake ()
    {
		rb = GetComponent<Rigidbody2D> ();
        if (fuel == 0)
            fuel = capacity;
        else
            rb.mass = dryMass + fuel * fuelMass;
		fuelMass = (rb.mass - dryMass) / fuel;
	}

	public bool Consume (float amount)
    {
        if (fuel > 0) {
			fuel -= amount;
            rb.mass -= fuelMass * amount;
			return true;
		} else {
			return false;
		}
	}
}