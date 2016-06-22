using UnityEngine;

public class Engine : PartFunction {
	
	public float force = 200f;
	public float turn = 60f;
	public float consumption = 1;
	bool active;
    Tank tank;
    SpriteRenderer fire;

	void Start() {
        fire = transform.GetChild(0).GetComponent<SpriteRenderer>();
        try {
            tank = transform.GetComponentInParent<Tank>();
            active = tank.GetFuel(consumption);
        } catch {
            Destroy(GetComponent<Engine>());
            Destroy(fire);
        }
	}

	void Update() {
		if (Input.GetKey (KeyCode.W) && active) {
			fire.enabled = true;
		}
		if (Input.GetKeyUp (KeyCode.W)) {
			fire.enabled = false;
		}
	}
	void FixedUpdate () {
		if (Input.GetKey(KeyCode.W) && active) {
			try {
				active = tank.GetFuel (consumption);
			} catch {
				active = false;
			}
			GetComponent<Rigidbody2D>().AddForce (transform.up * force);
			if (Input.GetKey(KeyCode.A)) {
				GetComponent<Rigidbody2D>().AddTorque(turn);
			}
			if (Input.GetKey(KeyCode.D)) {
				GetComponent<Rigidbody2D>().AddTorque(-turn);
			}
		}
	}
}
